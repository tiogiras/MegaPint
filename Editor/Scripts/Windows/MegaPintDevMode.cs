#if UNITY_EDITOR
using Editor.Scripts.GUI;
using Editor.Scripts.PackageManager;
using Editor.Scripts.Settings;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = Editor.Scripts.GUI.GUIUtility;

namespace Editor.Scripts.Windows
{

public class MegaPintDevMode : MegaPintEditorWindowBase
{
    private static Color s_onColor;
    private static Color s_offColor;

    private static MegaPintSettingsBase _Settings =>
        MegaPintSettings.instance.GetSetting("General");

    private VisualTreeAsset _baseWindow;
    private Button _btnOff;
    private Button _btnOn;

    private bool _devModeValue;
    
    #region Public Methods

    public override MegaPintEditorWindowBase ShowWindow()
    {
        titleContent.text = "Development Mode";

        return this;
    }

    #endregion
    
    #region Protected Methods

    protected override string BasePath()
    {
        return "MegaPint/User Interface/Windows/Dev Mode";
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = rootVisualElement;

        VisualElement content = _baseWindow.Instantiate();
        GUIUtility.ApplyTheme(content);

        root.Add(content);

        _btnOn = content.Q <Button>("BTN_On");
        _btnOff = content.Q <Button>("BTN_Off");

        s_onColor = RootElement.Colors.Primary;
        s_offColor = RootElement.Colors.Button;

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

        _devModeValue = _Settings.GetValue("devMode", false);

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

    private void ToggleOff()
    {
        _devModeValue = false;
        _Settings.SetValue("devMode", _devModeValue);

        UpdateButtonStyles();

        MegaPintPackageManager.UpdateAll();
    }

    private void ToggleOn()
    {
        _devModeValue = true;
        _Settings.SetValue("devMode", _devModeValue);

        UpdateButtonStyles();

        MegaPintPackageManager.UpdateAll();
    }

    private void UpdateButtonStyles()
    {
        _btnOn.style.backgroundColor = _devModeValue ? s_onColor : s_offColor;
        _btnOff.style.backgroundColor = _devModeValue ? s_offColor : s_onColor;
    }

    #endregion
}

}
#endif
