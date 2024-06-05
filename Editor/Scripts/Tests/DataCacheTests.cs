#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.PackageManager;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.Tests.Utility;
using NUnit.Framework;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TestTools;

namespace MegaPint.Editor.Scripts.Tests
{

/// <summary> Unit tests regarding the <see cref="DataCache" /> </summary>
internal class DataCacheTests
{
    #region Tests

    /// <summary> Validate the base package </summary>
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

    /// <summary> Validate the package data </summary>
    [Test] [Order(0)]
    public void PackageData()
    {
        var valid = true;

        foreach (PackageData packageData in DataCache.AllPackages)
        {
            PackageKey key = packageData.key;
            TestsUtility.Validate(ref valid, key == PackageKey.Undefined, "PackageKey is missing!");

            TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(packageData.reqMpVersion),
                $"Package[{key}] missing [reqMpVersion]!");

            TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(packageData.version),
                $"Package[{key}] missing [version]!");

            TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(packageData.unityVersion),
                $"Package[{key}] missing [unityVersion]!");

            TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(packageData.lastUpdate),
                $"Package[{key}] missing [lastUpdate]!");

            TestsUtility.Validate(ref valid, string.IsNullOrEmpty(packageData.name), $"Package[{key}] missing [name]!");

            TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(packageData.displayName),
                $"Package[{key}] missing [displayName]!");

            TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(packageData.description),
                $"Package[{key}] missing [description]!");

            TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(packageData.repository),
                $"Package[{key}] missing [repository]!");

            TestsUtility.Validate(ref valid, !ValidateDependencies(packageData.dependencies, key));
            TestsUtility.Validate(ref valid, !ValidateVariations(packageData.variations, key));
        }

        Assert.IsTrue(valid);
    }

    /// <summary> Validate the package data </summary>
    /// <returns> IEnumerator </returns>
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

    /// <summary> Validate package dependencies </summary>
    /// <param name="dependencies"> Dependencies of the package </param>
    /// <param name="parent"> Key of the parent package </param>
    /// <param name="variation"> Targeted variation of the package </param>
    /// <returns> If the dependencies are valid </returns>
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

    /// <summary> Validate the a package info </summary>
    /// <param name="packageInfo"> Targeted package info </param>
    /// <returns> If the packageInfo is valid </returns>
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

        TestsUtility.Validate(
            ref valid,
            string.IsNullOrEmpty(packageInfo.displayName),
            $"PackageInfo[{name}] missing [displayName]!");

        TestsUtility.Validate(ref valid, packageInfo.author == null, $"PackageInfo[{name}] missing [author]!");

        if (!TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(packageInfo.version),
                $"PackageInfo[{name}] missing [version]!"))
        {
            TestsUtility.Validate(
                ref valid,
                !packageInfo.version.Equals(packageData.version),
                $"PackageInfo[{name}] non-equal [version]!");
        }

        TestsUtility.Validate(
            ref valid,
            !packageInfo.category.Equals("Tools"),
            $"PackageInfo[{name}] incorrect [category]!");

        if (!TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(packageInfo.description),
                $"PackageInfo[{name}] missing [description]!"))
        {
            TestsUtility.Validate(
                ref valid,
                !packageInfo.description.Equals(packageData.description),
                $"PackageInfo[{name}] non-equal [description]!");
        }

        if (TestsUtility.Validate(
                ref valid,
                packageInfo.repository == null,
                $"PackageInfo[{name}] missing [repository]!"))
            return valid;

        if (!TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(packageInfo.repository!.url),
                $"PackageInfo[{name}] missing [repository.url]!"))
        {
            TestsUtility.Validate(
                ref valid,
                !packageInfo.repository.url.Equals(packageData.repository),
                $"PackageInfo[{name}] non-equal [repository.url]!");
        }

        return valid;
    }

    /// <summary> Validate variations of a package </summary>
    /// <param name="variations"> Targeted variations </param>
    /// <param name="parent"> Key of the parent package </param>
    /// <returns> If the variations are valid </returns>
    private static bool ValidateVariations(List <Variation> variations, PackageKey parent)
    {
        if (variations is not {Count: > 0})
            return true;

        var valid = true;

        foreach (Variation variation in variations)
        {
            TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(variation.name),
                $"Package[{parent}] Variation is missing a name");

            TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(variation.version),
                $"Package[{parent}] Variation[{variation.name}] missing [version]!");

            TestsUtility.Validate(
                ref valid,
                string.IsNullOrEmpty(variation.tag),
                $"Package[{parent}] Variation[{variation.name}] missing [tag]!");

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
#endif
