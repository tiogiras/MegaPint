#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using MegaPint.Editor.Scripts.Tests.Utility;
using NUnit.Framework;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.Tests
{

/// <summary> Unit tests regarding the general structure and settings of the package </summary>
internal class PackageTests
{
    /*private static bool s_initialized;*/

    #region Tests

    /*[UnityTest] [Order(0)]
    public IEnumerator InitializePackageCache()
    {
        Task <bool> task = TestsUtility.CheckCacheInitialization();

        yield return task.AsIEnumeratorReturnNull();

        s_initialized = task.Result;
        Assert.IsTrue(task.Result);
    }*/

    [Test] [Order(1)]
    public void PackageStructure()
    {
        /*if (!s_initialized)
            Assert.Fail("FAILED ===> Missing packageCache initialization!");*/

        // TestsUtility.CheckStructure(PackageKey.AlphaButton); // TODO Change to Base Package Structure
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

        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.BasePackage.UserInterface.Gallery);

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

        Assert.IsTrue(isValid);
    }

    #endregion
}

}
#endif
#endif
