#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.IO;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.Tests.Utility;
using NUnit.Framework;
using UnityEngine.Device;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.Tests
{

/// <summary> Unit tests regarding the general structure and settings of the package </summary>
internal class PackageTests
{
    #region Tests

    [Test]
    public void PackageStructure()
    {
        var isValid = true;

        var path = Application.dataPath[..^7];

        path = Path.Combine(path, "Packages", "com.tiogiras.megapint");

        TestsUtility.ValidateFiles(
            ref isValid,
            path,
            new[] {"LICENSE", "package.json", "README.md"},
            new[] {"Editor.meta", "LICENSE.meta", "package.json.meta", "README.md.meta"},
            out var requiredFiles,
            out var _);

        if (requiredFiles[0])
            TestsUtility.CheckLicenseFile(ref isValid, Path.Combine(path, "LICENSE"));

        if (requiredFiles[1])
            TestsUtility.CheckPackageJson(ref isValid, Path.Combine(path, "package.json"), PackageKey.Undefined);

        if (requiredFiles[2])
            TestsUtility.CheckReadMe(ref isValid, Path.Combine(path, "README.md"));

        TestsUtility.ValidateDirectories(
            ref isValid,
            path,
            new[] {"Editor"},
            new[] {".git"},
            out var requiredDirectories,
            out var _);

        if (!requiredDirectories[0])
            Assert.Fail();

        path = Path.Combine(path, "Editor");

        TestsUtility.ValidateFiles(
            ref isValid,
            path,
            null,
            new[] {"Additional Namespaces.meta", "Resources.meta", "Scripts.meta"},
            out var _,
            out var _);

        TestsUtility.ValidateDirectories(
            ref isValid,
            path,
            new[] {"Additional Namespaces", "Resources", "Scripts"},
            null,
            out requiredDirectories,
            out var _);

        if (requiredDirectories[1])
        {
            TestsUtility.ValidateNamingOfFilesInFolderAndSubFolders(
                ref isValid,
                Path.Combine(path, "Resources"),
                "Info Content",
                "Settings Content");
        }

        if (requiredDirectories[2])
            TestsUtility.ValidateNamingOfFilesInFolderAndSubFolders(ref isValid, Path.Combine(path, "Scripts"));

        Assert.IsTrue(isValid);
    }

    [Test] [Order(1)]
    public void Resources()
    {
        var isValid = true;

        TestsUtility.ValidateResource <VisualTreeAsset>(
            ref isValid,
            Constants.BasePackage.UserInterface.PackageManager);

        TestsUtility.ValidateResource <VisualTreeAsset>(
            ref isValid,
            Constants.BasePackage.UserInterface.PackageManagerDependencyItem);

        TestsUtility.ValidateResource <VisualTreeAsset>(
            ref isValid,
            Constants.BasePackage.UserInterface.PackageManagerPackageItem);

        TestsUtility.ValidateResource <VisualTreeAsset>(
            ref isValid,
            Constants.BasePackage.UserInterface.PackageManagerVariationItem);

        TestsUtility.ValidateResource <VisualTreeAsset>(
            ref isValid,
            Constants.BasePackage.UserInterface.PackageManagerSampleItem);

        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.BasePackage.UserInterface.Gallery);
        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.BasePackage.UserInterface.VersionCompatibility);
        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.BasePackage.UserInterface.CompatibilityItem);

        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.BasePackage.UserInterface.SplashScreen);
        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.BasePackage.UserInterface.Tooltip);

        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.BasePackage.UserInterface.BaseWindow);

        TestsUtility.ValidateResource <VisualTreeAsset>(
            ref isValid,
            Constants.BasePackage.UserInterface.BaseWindowInfoItem);

        TestsUtility.ValidateResource <VisualTreeAsset>(
            ref isValid,
            Constants.BasePackage.UserInterface.BaseWindowPackageItem);

        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.BasePackage.UserInterface.DevModeCenter);
        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.BasePackage.UserInterface.DevModeToggle);

        TestsUtility.ValidateResource <VisualTreeAsset>(
            ref isValid,
            Constants.BasePackage.UserInterface.InterfaceOverview);

        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.BasePackage.UserInterface.FirstSteps);
        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.BasePackage.UserInterface.Spinner);

        Assert.IsTrue(isValid);
    }

    #endregion
}

}
#endif
#endif
