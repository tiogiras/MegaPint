#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Editor.Scripts.PackageManager;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.PackageManager.Packages;
using NUnit.Framework;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TestTools;

namespace Editor.Scripts.Tests
{

/// <summary> Unit tests regarding the <see cref="DataCache"/> </summary>
internal class DataCacheTests : MonoBehaviour
{
    #region Tests
    
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

    [Test] [Order(0)]
    public void PackageData()
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
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(packageData.repository), $"Package[{key}] missing [repository]!");

            TestsUtility.Validate(ref valid, !ValidateDependencies(packageData.dependencies, key));
            TestsUtility.Validate(ref valid, !ValidateVariations(packageData.variations, key));
        }

        Assert.IsTrue(valid);
    }

    [UnityTest] [Order(1)]
    public IEnumerator PackageInfo()
    {
        Task <List <PackageInfo>> task = MegaPintPackageManager.GetInstalledPackages();

        while (!task.IsCompleted)
            yield return null;

        List <PackageInfo> packages = task.Result;

        var valid = true;

        foreach (PackageInfo packageInfo in packages)
            TestsUtility.Validate(ref valid, !ValidatePackageInfo(packageInfo));

        Assert.IsTrue(valid);
    }

    #endregion

    #region Private Methods

    private static bool ValidateDependencies(List <Dependency> dependencies, PackageKey parent, string variation = "")
    {
        if (dependencies is not {Count: > 0})
            return true;

        var valid = true;

        var variationStr = string.IsNullOrEmpty(variation) ? "" : $"Variation[{variation}]";

        foreach (Dependency dependency in dependencies)
        {
            TestsUtility.Validate(
                ref valid,
                dependency.key == PackageKey.Undefined,
                $"Package[{parent}] {variationStr} Dependency is missing PackageKey!");

            TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(dependency.name),
                $"Package[{parent}] {variationStr} Dependency[{dependency.key}] is missing a name!");
        }

        return valid;
    }

    private static bool ValidatePackageInfo(PackageInfo packageInfo)
    {
        var name = packageInfo.name;

        PackageData packageData = DataCache.PackageData(name);

        if (packageInfo.name.Equals(DataCache.BasePackageName))
            return true;

        if (packageData == null)
            return false;

        var valid = true;

        if (!TestsUtility.Validate(ref valid, string.IsNullOrEmpty(name), "PackageInfo is missing a name!"))
            TestsUtility.Validate(ref valid, !name!.Equals(packageData.name), $"PackageInfo[{name}] non-equal [name]!");

        TestsUtility.Validate(ref valid, string.IsNullOrEmpty(packageInfo.displayName), $"PackageInfo[{name}] missing [displayName]!");

        TestsUtility.Validate(ref valid, packageInfo.author == null, $"PackageInfo[{name}] missing [author]!");

        if (!TestsUtility.Validate(ref valid, string.IsNullOrEmpty(packageInfo.version), $"PackageInfo[{name}] missing [version]!"))
            TestsUtility.Validate(ref valid, !packageInfo.version.Equals(packageData.version), $"PackageInfo[{name}] non-equal [version]!");

        TestsUtility.Validate(ref valid, !packageInfo.category.Equals("Tools"), $"PackageInfo[{name}] incorrect [category]!");

        if (!TestsUtility.Validate(ref valid, string.IsNullOrEmpty(packageInfo.description), $"PackageInfo[{name}] missing [description]!"))
            TestsUtility.Validate(
                ref valid,
                !packageInfo.description.Equals(packageData.description),
                $"PackageInfo[{name}] non-equal [description]!");

        if (TestsUtility.Validate(ref valid, packageInfo.repository == null, $"PackageInfo[{name}] missing [repository]!"))
            return valid;

        if (!TestsUtility.Validate(ref valid, string.IsNullOrEmpty(packageInfo.repository!.url), $"PackageInfo[{name}] missing [repository.url]!"))
            TestsUtility.Validate(
                ref valid,
                !packageInfo.repository.url.Equals(packageData.repository),
                $"PackageInfo[{name}] non-equal [repository.url]!");

        return valid;
    }

    private static bool ValidateVariations(List <Variation> variations, PackageKey parent)
    {
        if (variations is not {Count: > 0})
            return true;

        var valid = true;

        foreach (Variation variation in variations)
        {
            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(variation.name), $"Package[{parent}] Variation is missing a name");

            TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(variation.version),
                $"Package[{parent}] Variation[{variation.name}] missing [version]!");

            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(variation.tag), $"Package[{parent}] Variation[{variation.name}] missing [tag]!");

            TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(variation.devBranch),
                $"Package[{parent}] Variation[{variation.name}] missing [devBranch]!");

            TestsUtility.Validate(ref valid, !ValidateDependencies(variation.dependencies, parent, variation.name));
        }

        return valid;
    }

    #endregion
}

}
#endif
