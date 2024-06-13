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
internal class DataCacheTests
{
    // private static bool ValidatePackageInfo(PackageInfo packageInfo)
    // {
    //     var name = packageInfo.name;
    //
    //     PackageData packageData = DataCache.PackageData(name);
    //
    //     if (packageInfo.name.Equals(DataCache.BasePackageName))
    //         return true;
    //
    //     if (packageData == null)
    //         return false;
    //
    //     var valid = true;
    //
    //     if (!TestsUtility.Validate(ref valid, string.IsNullOrEmpty(name), "PackageInfo is missing a name!"))
    //         TestsUtility.Validate(ref valid, !name!.Equals(packageData.name), $"PackageInfo[{name}] non-equal [name]!");
    //
    //     TestsUtility.Validate(
    //         ref valid,
    //         string.IsNullOrEmpty(packageInfo.displayName),
    //         $"PackageInfo[{name}] missing [displayName]!");
    //
    //     TestsUtility.Validate(ref valid, packageInfo.author == null, $"PackageInfo[{name}] missing [author]!");
    //
    //     if (!TestsUtility.Validate(
    //             ref valid,
    //             string.IsNullOrEmpty(packageInfo.version),
    //             $"PackageInfo[{name}] missing [version]!"))
    //     {
    //         TestsUtility.Validate(
    //             ref valid,
    //             !packageInfo.version.Equals(packageData.version),
    //             $"PackageInfo[{name}] non-equal [version]!");
    //     }
    //
    //     TestsUtility.Validate(
    //         ref valid,
    //         !packageInfo.category.Equals("Tools"),
    //         $"PackageInfo[{name}] incorrect [category]!");
    //
    //     if (!TestsUtility.Validate(
    //             ref valid,
    //             string.IsNullOrEmpty(packageInfo.description),
    //             $"PackageInfo[{name}] missing [description]!"))
    //     {
    //         TestsUtility.Validate(
    //             ref valid,
    //             !packageInfo.description.Equals(packageData.description),
    //             $"PackageInfo[{name}] non-equal [description]!");
    //     }
    //
    //     if (TestsUtility.Validate(
    //             ref valid,
    //             packageInfo.repository == null,
    //             $"PackageInfo[{name}] missing [repository]!"))
    //         return valid;
    //
    //     if (!TestsUtility.Validate(
    //             ref valid,
    //             string.IsNullOrEmpty(packageInfo.repository!.url),
    //             $"PackageInfo[{name}] missing [repository.url]!"))
    //     {
    //         TestsUtility.Validate(
    //             ref valid,
    //             !packageInfo.repository.url.Equals(packageData.repository),
    //             $"PackageInfo[{name}] non-equal [repository.url]!");
    //     }
    //
    //     return valid;
    // }
}

}
#endif
#endif
