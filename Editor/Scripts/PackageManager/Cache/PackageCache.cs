#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MegaPint.Editor.Scripts.DevMode;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.PackageManager.Utility;
using UnityEditor.PackageManager;

[assembly: InternalsVisibleTo("tiogiras.megapint.Editor.Tests")]

namespace MegaPint.Editor.Scripts.PackageManager.Cache
{

/// <summary> Sores all information about the installed and non installed MegaPint packages </summary>
internal static class PackageCache
{
    /// <summary> Called when the cache was refreshed </summary>
    public static Action onCacheRefreshed;

    public static Action <float> onCacheProgressChanged;
    public static Action <string> onCacheProcessChanged;

    private static float s_currentProgress;

    public static Action onCacheStartRefreshing;

    private static readonly Dictionary <PackageKey, CachedPackage> s_cache = new();

    /// <summary> <see cref="PackageInfo" /> of the MegaPint basePackage </summary>
    public static PackageInfo BasePackage {get; private set;}

    /// <summary> Newest Version of the MegaPint basePackage </summary>
    public static string NewestBasePackageVersion {get; private set;}

    public static bool WasInitialized {get; private set;}

    #region Public Methods

    /// <summary> If the package can be removed or if packages are dependant on it </summary>
    /// <param name="key"> Key to the targeted package </param>
    /// <param name="dependencies"> Dependencies that point to the package </param>
    /// <returns> True when no dependencies point to the package </returns>
    public static bool CanBeRemoved(PackageKey key, out List <PackageKey> dependencies)
    {
        return s_cache[key].CanBeRemoved(out dependencies);
    }

    /// <summary> Current version of the package </summary>
    /// <param name="key"> Key to the targeted package </param>
    /// <returns> Current version </returns>
    public static string CurrentVersion(PackageKey key)
    {
        return s_cache[key].CurrentVersion;
    }

    /// <summary> Get a <see cref="CachedPackage" /> by the corresponding <see cref="PackageKey" /> </summary>
    /// <param name="key"> Key to the targeted package </param>
    /// <returns> Found <see cref="CachedPackage" /> for the given key </returns>
    public static CachedPackage Get(PackageKey key)
    {
        return s_cache[key];
    }

    /// <summary> Get all <see cref="CachedPackage" /> </summary>
    /// <returns> All packages not regarding installation state </returns>
    public static List <CachedPackage> GetAllMpPackages()
    {
        return s_cache.Values.ToList();
    }

    /// <summary> Get all <see cref="CachedPackage" /> and <see cref="CachedVariation" /> that are currently installed </summary>
    /// <param name="packages"> Found installed <see cref="CachedPackage" /> </param>
    /// <param name="variations"> Found installed <see cref="CachedVariation" /> </param>
    public static void GetInstalledMpPackages(out List <CachedPackage> packages, out List <CachedVariation> variations)
    {
        packages = new List <CachedPackage>();
        variations = new List <CachedVariation>();

        foreach (CachedPackage cachedPackage in s_cache.Values.Where(cachedPackage => cachedPackage.IsInstalled))
        {
            if (string.IsNullOrEmpty(cachedPackage.CurrentVariationHash))
            {
                packages.Add(cachedPackage);

                continue;
            }

            variations.Add(cachedPackage.CurrentVariation);
        }
    }

    /// <summary> Installation state of the targeted package </summary>
    /// <param name="key"> Key to the targeted package </param>
    /// <returns> If the package is currently installed </returns>
    public static bool IsInstalled(PackageKey key)
    {
        return s_cache[key].IsInstalled;
    }

    /// <summary> Check if the current variation of a package is the given variation </summary>
    /// <param name="key"> Key to the targeted package </param>
    /// <param name="gitHash"> Hash of the variation that is to be checked </param>
    /// <returns> If the current installed variation equals the given hash </returns>
    public static bool IsVariation(PackageKey key, string gitHash)
    {
        return s_cache[key].CurrentVariationHash.Equals(gitHash);
    }

    /// <summary> If the MegaPint basePackage needs update </summary>
    /// <returns> True if update is available </returns>
    public static bool NeedsBasePackageUpdate()
    {
        return !BasePackage.version.Equals(NewestBasePackageVersion);
    }

    /// <summary> If the package needs an update </summary>
    /// <param name="key"> Key to the targeted package </param>
    /// <returns> True if update is available </returns>
    public static bool NeedsUpdate(PackageKey key)
    {
        return !s_cache[key].IsNewestVersion;
    }

    /// <summary> If the variation of the package needs an update </summary>
    /// <param name="key"> Key to the targeted package </param>
    /// <param name="variationName"> Name of the targeted variation </param>
    /// <returns> True if update is available </returns>
    public static bool NeedsVariationUpdate(PackageKey key, string variationName)
    {
        CachedVariation variation = s_cache[key].Variations.First(variation => variation.name.Equals(variationName));

        return !variation.isNewestVersion;
    }

    /// <summary> Refresh the cache </summary>
    public static void Refresh()
    {
        Initialize();
    }

    private static void SetProcess(string process)
    {
        onCacheProcessChanged?.Invoke(process);
    }

    #endregion

    #region Private Methods

    private static void GetInstalledPackageNames(List <PackageInfo> packages, out List <string> packageNames)
    {
        packageNames = new List <string>();

        if (packages is not {Count: > 0})
            return;

        foreach (PackageInfo package in packages)
        {
            packageNames.Add(package.name);

            if (!package.name.Equals(DataCache.BasePackageName))
                continue;

            BasePackage = package;
            DevLog.Log("BasePackage detected");

            NewestBasePackageVersion = GitExtension.LatestGitTag(BasePackage.repository.url)[1..];
        }
    }

    private static void IncreaseProgressBy(float delta)
    {
        s_currentProgress += delta;
        onCacheProgressChanged?.Invoke(s_currentProgress);
    }

    private static async void Initialize()
    {
        onCacheStartRefreshing?.Invoke();

        SetProgressTo(0);

        SetProcess("Reading Data Cache");
        PackageData[] mpPackages = DataCache.AllPackages;
        IncreaseProgressBy(.15f);

        SetProcess("Reading Unity's PackageInfo");
        List <PackageInfo> installedPackages = await MegaPintPackageManager.GetInstalledPackages();
        IncreaseProgressBy(.3f);

        SetProcess("Collecting Packages");
        GetInstalledPackageNames(installedPackages, out List <string> installedPackagesNames);
        IncreaseProgressBy(.05f);

        s_cache.Clear();

        Dictionary <PackageKey, List <PackageKey>> allDependencies = new();

        var step = .3f / mpPackages.Length;

        foreach (PackageData packageData in mpPackages)
        {
            SetProcess($"Creating Cache: {packageData.displayName}");

            var package = new CachedPackage(
                packageData,
                installedPackagesNames.Contains(packageData.name)
                    ? installedPackages[installedPackagesNames.IndexOf(packageData.name)]
                    : null,
                out List <Dependency> dependencies);

            if (dependencies is {Count: > 0})
            {
                foreach (Dependency dependency in dependencies)
                {
                    allDependencies.TryAdd(dependency.key, new List <PackageKey>());
                    allDependencies[dependency.key].Add(package.Key);
                }
            }

            s_cache.Add(package.Key, package);

            IncreaseProgressBy(step);
        }

        SetProgressTo(.8f);

        step = .2f / allDependencies.Count;

        foreach (KeyValuePair <PackageKey, List <PackageKey>> valuePair in allDependencies)
        {
            SetProcess($"Registering Dependencies: {valuePair.Key}");

            CachedPackage cachedPackage = s_cache[valuePair.Key];
            cachedPackage.RegisterDependencies(valuePair.Value);

            IncreaseProgressBy(step);
        }

        SetProcess("Finalizing Cache");
        SetProgressTo(1f);

        WasInitialized = true;

        onCacheRefreshed?.Invoke();
    }

    private static void SetProgressTo(float progress)
    {
        s_currentProgress = progress;
        onCacheProgressChanged?.Invoke(s_currentProgress);
    }

    #endregion
}

}
#endif
