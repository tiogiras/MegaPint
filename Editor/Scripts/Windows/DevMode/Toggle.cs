#if UNITY_EDITOR
using MegaPint.Editor.Scripts.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Windows.DevMode
{

/// <summary> Editor window to toggle on/off the development mode </summary>
public class Toggle : EditorWindowBase
{
    private VisualTreeAsset _baseWindow;
    private Button _btnOff;
    private Button _btnOn;

    private bool _devModeValue;

    #region Public Methods

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "Development Mode";

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return Constants.BasePackage.UserInterface.DevModeToggle;
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = rootVisualElement;

        VisualElement content = GUIUtility.Instantiate(_baseWindow);

        content.style.flexGrow = 1f;
        content.style.flexShrink = 1f;

        root.Add(content);

        _btnOn = content.Q <Button>("BTN_On");
        _btnOff = content.Q <Button>("BTN_Off");

        UpdateButtonStyles();

        RegisterCallbacks();
    }

    protected override bool LoadResources()
    {
        _baseWindow = Resources.Load <VisualTreeAsset>(BasePath());

        return _baseWindow != null;
    }

    protected override bool LoadSettings()
    {
        if (!base.LoadSettings())
            return false;

        _devModeValue = SaveValues.BasePackage.DevMode;

        return true;
    }

    protected override void RegisterCallbacks()
    {
        _btnOn.clicked += ToggleOn;
        _btnOff.clicked += ToggleOff;
    }

    protected override void UnRegisterCallbacks()
    {
        _btnOn.clicked -= ToggleOn;
        _btnOff.clicked -= ToggleOff;
    }

    #endregion

    #region Private Methods

    /// <summary> Turn off development mode </summary>
    private void ToggleOff()
    {
        _devModeValue = false;
        SaveValues.BasePackage.DevMode = _devModeValue;

        UpdateButtonStyles();

        MegaPintPackageManager.UpdateAll();
    }

    /// <summary> Turn on development mode </summary>
    private void ToggleOn()
    {
        _devModeValue = true;
        SaveValues.BasePackage.DevMode = _devModeValue;

        UpdateButtonStyles();

        MegaPintPackageManager.UpdateAll();
    }

    /// <summary> Update the state of the buttons </summary>
    private void UpdateButtonStyles()
    {
        GUIUtility.ToggleActiveButtonInGroup(_devModeValue ? 0 : 1, _btnOn, _btnOff);
    }

    #endregion
}

}
#endif
