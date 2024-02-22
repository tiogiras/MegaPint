#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.PackageManager.Packages;
using NUnit.Framework;
using UnityEngine;

namespace Editor.Scripts.Tests
{

public class DataCacheTests : MonoBehaviour
{
    [Test]
    public void BasePackage()
    {
        var valid = true;

        if (string.IsNullOrEmpty(DataCache.BasePackageName))
        {
            Debug.Log("Cache is missing the [BasePackageName]!");
            valid = false;
        }
        
        if (string.IsNullOrEmpty(DataCache.BasePackageDevBranch))
        {
            Debug.Log("Cache is missing the [BasePackageDevBranch]!");
            valid = false;
        }

        Assert.IsTrue(valid);
    }

    [Test]
    public void Packages()
    {
        var valid = true;
        
        foreach (PackageData packageData in DataCache.AllPackages)
        {
            PackageKey key = packageData.key;
            TestsUtility.Validate(ref valid, key == PackageKey.Undefined, "PackageKey is missing!");
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(packageData.reqMpVersion), $"Package[{key}] missing [reqMpVersion]!");
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(packageData.version), $"Package[{key}] missing [version]!");
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(packageData.unityVersion), $"Package[{key}] missing [unityVersion]!");
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(packageData.lastUpdate), $"Package[{key}] missing [lastUpdate]!");
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(packageData.name), $"Package[{key}] missing [name]!");
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(packageData.displayName), $"Package[{key}] missing [displayName]!");
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(packageData.description), $"Package[{key}] missing [description]!");

            TestsUtility.Validate(ref valid, !ValidateDependencies(packageData.dependencies, key));
            TestsUtility.Validate(ref valid, !ValidateVariations(packageData.variations, key));
        }
        
        Assert.IsTrue(valid);
    }

    private static bool ValidateVariations(List <Variation> variations, PackageKey parent)
    {
        if (variations is not {Count: > 0})
            return true;
        
        var valid = true;

        foreach (Variation variation in variations)
        {
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(variation.name), $"Package[{parent}] Variation is missing a name");
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(variation.version), $"Package[{parent}] Variation[{variation.name}] missing [version]!");
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(variation.tag), $"Package[{parent}] Variation[{variation.name}] missing [tag]!");
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(variation.devBranch), $"Package[{parent}] Variation[{variation.name}] missing [devBranch]!");
            
            TestsUtility.Validate(ref valid, !ValidateDependencies(variation.dependencies, parent, variation.name));
        }

        return valid;
    }
    
    private static bool ValidateDependencies(List <Dependency> dependencies, PackageKey parent, string variation = "")
    {
        if (dependencies is not {Count: > 0})
            return true;
        
        var valid = true;

        var variationStr = string.IsNullOrEmpty(variation) ? "" : $"Variation[{variation}]";
        
        foreach (Dependency dependency in dependencies)
        {
            TestsUtility.Validate(ref valid, dependency.key == PackageKey.Undefined, $"Package[{parent}] {variationStr} Dependency is missing PackageKey!");
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(dependency.name), $"Package[{parent}] {variationStr} Dependency[{dependency.key}] is missing a name!");
        }

        return valid;
    }
}

}
#endif
