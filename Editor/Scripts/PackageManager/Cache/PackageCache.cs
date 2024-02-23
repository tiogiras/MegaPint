#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.DevModeUtil;
using Editor.Scripts.PackageManager.Packages;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Editor.Scripts.PackageManager.Cache
{

internal class PackageCache
{
    public static Action onCacheRefreshed;
    
    public static PackageInfo BasePackage {get; private set;}
    
    private static readonly Dictionary <PackageKey, CachedPackage> s_cache = new();

    private static void GetInstalledPackageNames(List<PackageInfo> packages, out List <string> packageNames)
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
            DevLog.Log(typeof(PackageCache), "BasePackage detected");
        }
    }
    
    private static async void Initialize()
    {
        IEnumerable <PackageData> mpPackages = DataCache.AllPackages;
        List <PackageInfo> installedPackages = await MegaPintPackageManager.GetInstalledPackages();

        GetInstalledPackageNames(installedPackages, out List <string> installedPackagesNames);
        
        s_cache.Clear();

        Dictionary <PackageKey, List <Dependency>> allDependencies = new();

        foreach (PackageData packageData in mpPackages)
        {
            var package = new CachedPackage(
                packageData,
                installedPackagesNames.Contains(packageData.name) ? installedPackages[installedPackagesNames.IndexOf(packageData.name)] : null,
                out List <Dependency> dependencies);

            if (dependencies is {Count: > 0})
            {
                foreach (Dependency dependency in dependencies)
                {
                    allDependencies.TryAdd(dependency.key, new List <Dependency>());
                    allDependencies[dependency.key].Add(dependency);
                }
            }
            
            s_cache.Add(package.Key, package);
        }

        foreach (KeyValuePair<PackageKey,List<Dependency>> valuePair in allDependencies)
        {
            CachedPackage cachedPackage = s_cache[valuePair.Key];
            cachedPackage.RegisterDependencies(valuePair.Value);
        }

        onCacheRefreshed?.Invoke();
    }

    public static CachedPackage Get(PackageKey key) => s_cache[key];

    public static bool CanBeRemoved(PackageKey key, out List <Dependency> dependencies)
    {
        return s_cache[key].CanBeRemoved(out dependencies);
    }

    public static string CurrentVersion(PackageKey key)
    {
        return s_cache[key].CurrentVersion;
    }
    
    public static bool IsInstalled(PackageKey key)
    {
        return s_cache[key].IsInstalled;
    }
    
    public static bool NeedsUpdate(PackageKey key)
    {
        return !s_cache[key].IsNewestVersion;
    }
    
    public static bool IsVariation(PackageKey key, string gitHash)
    {
        return s_cache[key].CurrentVariationHash.Equals(gitHash);
    }
    
    public static bool NeedsVariationUpdate(PackageKey key, string variationName)
    {
        CachedVariation variation = s_cache[key].Variations.First(variation => variation.name.Equals(variationName));
        return !variation.isNewestVersion;
    }
    
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

    public static List <CachedPackage> GetAllMpPackages()
    {
        return s_cache.Values.ToList();
    }

    public static void Refresh()
    {
        Debug.Log("Refreshing");
        Initialize();
    }
}

}
#endif
