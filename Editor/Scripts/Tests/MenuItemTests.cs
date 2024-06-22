#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using MegaPint.Editor.Scripts.Tests.Utility;
using MegaPint.Editor.Scripts.Windows;
using NUnit.Framework;

namespace MegaPint.Editor.Scripts.Tests
{

/// <summary> Unit tests regarding the menuItems of the package </summary>
internal class MenuItemTests
{
    #region Tests

    [Test]
    public void BaseWindow()
    {
        TestsUtility.ValidateMenuItemLink(Constants.BasePackage.Links.BaseWindow, typeof(BaseWindow));
    }

    [Test]
    public void PackageManager()
    {
        TestsUtility.ValidateMenuItemLink(Constants.BasePackage.Links.PackageManager, typeof(Windows.PackageManager));
    }

    [Test]
    public void Shortcuts()
    {
        TestsUtility.ValidateMenuItemLink(Constants.BasePackage.Links.Shortcuts, false, "Shortcuts");
    }

    #endregion
}

}
#endif
#endif
