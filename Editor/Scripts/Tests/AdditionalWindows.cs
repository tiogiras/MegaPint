#if UNITY_EDITOR
using MegaPint.Editor.Scripts.Tests.Utility;
using MegaPint.Editor.Scripts.Windows;
using MegaPint.Editor.Scripts.Windows.DevMode;
using NUnit.Framework;

namespace MegaPint.Editor.Scripts.Tests
{

/// <summary> Unit tests regarding the menuItems of the package </summary>
internal class AdditionalWindows
{
    #region Tests

    [Test]
    public void DevModeCenter()
    {
        TestsUtility.ValidateEditorWindow <Center>();
    }

    [Test]
    public void DevModeToggle()
    {
        TestsUtility.ValidateEditorWindow <Toggle>(true);
    }

    [Test]
    public void FirstSteps()
    {
        TestsUtility.ValidateEditorWindow <FirstSteps>(true);
    }

    [Test]
    public void Gallery()
    {
        TestsUtility.ValidateEditorWindow <Gallery>();
    }

    [Test]
    public void InterfaceOverview()
    {
        TestsUtility.ValidateEditorWindow <InterfaceOverview>();
    }

    #endregion
}

}
#endif
