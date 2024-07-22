#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.PackageManager.Utility;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace MegaPint.Editor.Scripts.PackageManager
{

internal static class MegaPintPackageManager
{
    /// <summary> RefreshRate while loading packages </summary>
    public const int RefreshRate = 10;

    /// <summary> Called while requesting packages </summary>
    public static Action onRefreshingPackages;

    /// <summary> Called after successfully calling an action regarding the packageManager  </summary>
    public static Action onSuccess;

    /// <summary> Called after failing an action regarding the packageManager </summary>
    public static Action <string> onFailure;

    #region Public Methods

    /// <summary> Add and Embed a package </summary>
    /// <param name="package"> <see cref="CachedPackage" /> to be added </param>
    /// <param name="suppressCacheRefresh">
    ///     If true the <see cref="PackageCache" /> will not be refreshed after finishing the
    ///     task
    /// </param>
    public static async Task AddEmbedded(CachedPackage package, bool suppressCacheRefresh = false)
    {
        await AddEmbedded(PackageManagerUtility.GetPackageUrl(package), package.Dependencies, suppressCacheRefresh);
    }

    /// <summary> Add and Embed a variation </summary>
    /// <param name="variation"> <see cref="CachedVariation" /> to be added </param>
    /// <param name="suppressCacheRefresh">
    ///     If true the <see cref="PackageCache" /> will not be refreshed after finishing the
    ///     task
    /// </param>
    public static async Task AddEmbedded(CachedVariation variation, bool suppressCacheRefresh = false)
    {
        await AddEmbedded(PackageManagerUtility.GetPackageUrl(variation), variation.dependencies, suppressCacheRefresh);
    }

    /// <summary> Get all installed packages </summary>
    /// <returns> All <see cref="UnityEditor.PackageManager.PackageInfo" /> that are currently installed </returns>
    public static async Task <List <PackageInfo>> GetInstalledPackages()
    {
        ListRequest request = Client.List();

        while (!request.IsCompleted)
        {
            await Task.Delay(RefreshRate);

            onRefreshingPackages?.Invoke();
        }

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);

        return request.Result.Where(packageInfo => packageInfo.name.ToLower().Contains("megapint")).ToList();
    }

    /// <summary> Install all packages </summary>
    public static void InstallAll()
    {
        ImportBulk(PackageCache.GetAllMpPackages());
        
        PackageCache.Refresh();
    }

    /// <summary>
    ///     due to restrictions in the unity engine that makes it impossible to import multiple packages without reloading the scripts
    ///     in between and therefore canceling the import. Importing multiple packages is handled via adding the packages to the manifest and unity will
    ///     detect the changes automatically
    /// </summary>
    /// <param name="packages"> Packages to add </param>
    private static void ImportBulk(List <CachedPackage> packages)
    {
        if (packages is not {Count: > 0})
            return;
        
        var path = Application.dataPath[..^7];
        path = Path.Combine(path, "Packages", "manifest.json");

        var manifestText = File.ReadAllText(path);

        const string Divider = "\"dependencies\": {";

        var parts = manifestText.Split(Divider);

        var part1 = $"{parts[0]}{Divider}";
        var part2 = parts[1];

        foreach (CachedPackage package in packages)
        {
            var name = $"\"{package.Name}\":";

            if (part2.Contains(name))
                continue;

            var url = $"\"{PackageManagerUtility.GetPackageUrl(package)}\"";

            part2 = $"{name}{url},{part2}";
        }

        var newManifest = $"{part1}{part2}";

        File.WriteAllText(path, newManifest);
    }

    /// <summary> Remove a package </summary>
    /// <param name="packageName"> Name of the package to be removed </param>
    /// <param name="suppressCacheRefresh">
    ///     If true the <see cref="PackageCache" /> will not be refreshed after finishing the
    ///     task
    /// </param>
    public static async Task Remove(string packageName, bool suppressCacheRefresh = false)
    {
        RemoveRequest request = Client.Remove(packageName);

        while (!request.IsCompleted)
            await Task.Delay(RefreshRate);

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);
        else
        {
            onSuccess?.Invoke();

            if (!suppressCacheRefresh)
                PackageCache.Refresh();
        }
    }

    /// <summary> Update all packages to their currently specified version </summary>
    public static async void UpdateAll()
    {
        var version = PackageCache.BasePackage.version;

        PackageCache.GetInstalledMpPackages(out List <CachedPackage> packages, out List <CachedVariation> variations);

        if (packages.Count > 0)
        {
            foreach (CachedPackage package in packages)
                await AddEmbedded(package);
        }

        if (variations.Count > 0)
        {
            foreach (CachedVariation variation in variations)
                await AddEmbedded(variation);
        }

        var url = SaveValues.BasePackage.DevMode
            ? $"{PackageCache.BasePackage.repository.url}#{DataCache.BasePackageDevBranch}"
            : $"{PackageCache.BasePackage.repository.url}#v{version}";

        await AddEmbedded(url);

        PackageCache.Refresh();
    }

    /// <summary> Update the basePackage to it's newest possible version </summary>
    public static async Task UpdateBasePackage()
    {
        var url = SaveValues.BasePackage.DevMode
            ? $"{PackageCache.BasePackage.repository.url}#{DataCache.BasePackageDevBranch}"
            : $"{PackageCache.BasePackage.repository.url}#v{PackageCache.NewestBasePackageVersion}";

        await AddEmbedded(url);
        PackageCache.Refresh();
    }

    #endregion

    #region Private Methods

    private static async Task <bool> Add(string packageUrl)
    {
        AddRequest request = Client.Add(packageUrl);

        while (!request.IsCompleted)
            await Task.Delay(RefreshRate);

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);

        return request.Status == StatusCode.Success;
    }

    private static async Task AddEmbedded(string gitUrl, List <Dependency> dependencies, bool suppressCacheRefresh)
    {
        var requestingDomainReload = false;
        
        if (dependencies != null)
        {
            if (dependencies.Count >= 3)
            {
                ImportBulk(PackageCache.GetRange(dependencies.Select(dependency => dependency.key)));
                Debug.Log("Imported in bulk"); // TODO remove
                
                requestingDomainReload = true;
            }
            else
            {
                foreach (CachedPackage cachedPackage in dependencies.Select(dependency => PackageCache.Get(dependency.key)))
                {
                    await AddEmbedded(
                        PackageManagerUtility.GetPackageUrl(cachedPackage),
                        cachedPackage.Dependencies,
                        suppressCacheRefresh);

                    await Task.Delay(250);
                }
            }
        }
        
        Debug.Log("Delaying..."); // TODO remove
        
        await Task.Delay(250);
        
        Debug.Log("Adding..."); // TODO remove
        await AddEmbedded(gitUrl);

        Debug.Log("Added... Reloading"); // TODO remove
        
        onSuccess?.Invoke();

        if (!suppressCacheRefresh)
            PackageCache.Refresh();
        
        if (requestingDomainReload)
            EditorUtility.RequestScriptReload();
    }

    private static async Task AddEmbedded(string packageUrl)
    {
        if (!await Add(packageUrl))
            return;

        try
        {
            await Embed(packageUrl);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private static async Task Embed(string packageName)
    {
        EmbedRequest request = Client.Embed(packageName);

        while (!request.IsCompleted)
            await Task.Delay(RefreshRate);

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);
    }

    #endregion
}

}
#endif
