using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.PackageManager.Packages;
using UnityEditor.PackageManager;

namespace Editor.Scripts.PackageManager
{

public static class Tests
{
    public static bool HasKey(PackageKey key)
    {
        return DataCache.PackageData(key).key != PackageKey.Undefined;
    }
    
    public static bool HasMpVersion(PackageKey key)
    {
        return !string.IsNullOrEmpty(DataCache.PackageData(key).reqMpVersion);
    }

    public static string Name(PackageKey key)
    {
        return DataCache.PackageData(key).name;
    }
    
    public static string DisplayName(PackageKey key)
    {
        return DataCache.PackageData(key).displayName;
    }
    
    public static string Description(PackageKey key)
    {
        return DataCache.PackageData(key).description;
    }
    
    public static string Version(PackageKey key)
    {
        return DataCache.PackageData(key).version;
    }
    
    public static bool HasUnityVersion(PackageKey key)
    {
        return !string.IsNullOrEmpty(DataCache.PackageData(key).unityVersion);
    }
    
    public static bool HasLastUpdateVersion(PackageKey key)
    {
        return !string.IsNullOrEmpty(DataCache.PackageData(key).lastUpdate);
    }

    public static bool HasDependencies(PackageKey key)
    {
        return DataCache.PackageData(key).dependencies is {Count: > 0};
    }
    
    public static bool HasDependencyNames(PackageKey key, out string error)
    {
        error = "";

        List <Dependency> list = DataCache.PackageData(key).dependencies;

        for (var i = 0; i < list.Count; i++)
        {
            Dependency dependency = list[i];

            if (!string.IsNullOrEmpty(dependency.name))
                continue;

            error = $"[{key}, PackageData]: Variation at the index {i} has missing name";

            return false;
        }

        return true;
    }
    
    public static bool HasCorrectDependencyKeys(PackageKey key, out string error)
    {
        error = "";

        List <Dependency> list = DataCache.PackageData(key).dependencies;

        for (var i = 0; i < list.Count; i++)
        {
            Dependency dependency = list[i];

            if (dependency.key == key)
                continue;

            error = $"[{key}, PackageData]: Variation at the index {i} has incorrect key";

            return false;
        }

        return true;
    }
    
    public static bool HasVariations(PackageKey key)
    {
        return DataCache.PackageData(key).variations is {Count: > 0};
    }
    
    public static bool HasVariationNames(PackageKey key, out string error)
    {
        error = "";

        List <Dependency> list = DataCache.PackageData(key).dependencies;

        for (var i = 0; i < list.Count; i++)
        {
            Dependency dependency = list[i];

            if (dependency.key == key)
                continue;

            error = $"[{key}, PackageData]: Variation at the index {i} has incorrect key";

            return false;
        }

        return true;
    }
    
    public static bool HasCorrectVariations(PackageKey key, out string error)
    {
        error = "";

        List <Variation> list = DataCache.PackageData(key).variations;

        for (var i = 0; i < list.Count; i++)
        {
            Variation variation = list[i];

            if (string.IsNullOrEmpty(variation.name))
            {
                error = $"[{key}, PackageData]: Variation at the index {i} has incorrect key";
                return false;
            }
            
            
        }

        return true;
    }
    
    public static bool HasVariationTags(PackageKey key, out string error)
    {
        error = "";

        List <Dependency> list = DataCache.PackageData(key).dependencies;

        for (var i = 0; i < list.Count; i++)
        {
            Dependency dependency = list[i];

            if (dependency.key == key)
                continue;

            error = $"[{key}, PackageData]: Variation at the index {i} has incorrect key";

            return false;
        }

        return true;
    }
    
    public static bool HasVariationDevBranches(PackageKey key, out string error)
    {
        error = "";

        List <Dependency> list = DataCache.PackageData(key).dependencies;

        for (var i = 0; i < list.Count; i++)
        {
            Dependency dependency = list[i];

            if (dependency.key == key)
                continue;

            error = $"[{key}, PackageData]: Variation at the index {i} has incorrect key";

            return false;
        }

        return true;
    }

    public static PackageInfo BasePackage()
    {
        return PackageCache.BasePackage;
    }

    public static async Task <PackageInfo> GetInfo(PackageKey key)
    {
        List <PackageInfo> packages = await MegaPintPackageManager.GetInstalledPackages();

        var name = DataCache.PackageData(key).name;
        return packages.FirstOrDefault(packageInfo => packageInfo.name.Equals(name));
    }
}

}
