#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System;
using MegaPint.Editor.Scripts.GUI.Utility;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.Tests.Utility;
using NUnit.Framework;

namespace MegaPint.Editor.Scripts.Tests
{

/// <summary> Unit regarding the <see cref="DataCache" /> data of the basePackage </summary>
internal class DataCacheTest
{
    #region Tests

    [Test]
    public void BasePackageDevBranch()
    {
        Assert.IsFalse(
            string.IsNullOrEmpty(DataCache.BasePackageDevBranch),
            "Cache is missing the [BasePackageDevBranch]!");
    }

    [Test]
    public void BasePackageName()
    {
        Assert.IsFalse(string.IsNullOrEmpty(DataCache.BasePackageName), "Cache is missing the [BasePackageName]!");
    }

    [Test]
    public void PackagesData()
    {
        var isValid = true;

        foreach (PackageData package in DataCache.AllPackages)
        {
            TestsUtility.ValidatePackageData(ref isValid, package);

            TestsUtility.ValidateDependenciesData(ref isValid, package.dependencies, package.key);
            TestsUtility.ValidateVariationsData(ref isValid, package.variations, package.key);
            TestsUtility.ValidateSamplesData(ref isValid, package.samples, package.key);
        }

        Assert.IsTrue(isValid);
    }
    
    [Test] [Order(2)]
    public void Versions()
    {
        foreach (PackageKey key in (PackageKey[])Enum.GetValues(typeof(PackageKey)))
        {
            if (VersionUtility.Prefixes.ContainsKey(key))
                continue;

            Assert.Fail($"The PackageKey [{key}] was not defined in the version prefix dictionary");
        }
        
        Assert.Pass();
    }

    #endregion
}

}
#endif
#endif
