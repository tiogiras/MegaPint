#if UNITY_EDITOR
using Editor.Scripts.PackageManager;
using Editor.Scripts.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Windows
{

public class MegaPintDevMode : MegaPintEditorWindowBase
{
    private MegaPintSettingsBase _settings => MegaPintSettings.instance.GetSetting("General");

    #region Public Methods

    public override MegaPintEditorWindowBase ShowWindow()
    {
        titleContent.text = "Developer Mode";

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return "User Interface/MegaPintDevMode";
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = rootVisualElement;

        VisualElement content = _baseWindow.Instantiate();

        _btnOn = content.Q <Button>("BTN_On");
        _btnOff = content.Q <Button>("BTN_Off");

        UpdateButtonStyles();

        RegisterCallbacks();

        root.Add(content);
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

        _devModeValue = _settings.GetValue("devMode", false);

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
        _settings.SetValue("devMode", _devModeValue);

        UpdateButtonStyles();
        
        MegaPintPackageManager.UpdateAll();
    }

    private void ToggleOn()
    {
        _devModeValue = true;
        _settings.SetValue("devMode", _devModeValue);

        UpdateButtonStyles();

        MegaPintPackageManager.UpdateAll();
    }

    private void UpdateButtonStyles()
    {
        _btnOn.style.backgroundColor = _devModeValue ? s_onColor : s_offColor;
        _btnOff.style.backgroundColor = _devModeValue ? s_offColor : s_onColor;
    }

    #endregion

    #region Private

    /// <summary> Loaded uxml references </summary>
    private VisualTreeAsset _baseWindow;

    private bool _devModeValue;

    private static readonly Color s_onColor = new(.8196078431372549f, 0f, .4470588235294118f);
    private static readonly Color s_offColor = new(.34f, .34f, .34f);

    #endregion

    #region Visual References

    private Button _btnOn;
    private Button _btnOff;

    #endregion
}

}
#endif
