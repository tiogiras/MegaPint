#if UNITY_EDITOR
using MegaPint.Editor.Scripts.GUI.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Windows.DevMode
{

/// <summary> Editor window to access various development mode utility </summary>
internal class Center : EditorWindowBase
{
    private VisualTreeAsset _baseWindow;
    private Button _btnInterfaceOverview;
    private Button _btnReloadDomain;
    private Button _btnRepaint;

    private Button _btnToggle;

    #region Public Methods

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "Dev Center";

        minSize = new Vector2(350, 250);
        maxSize = minSize;

        if (!SaveValues.BasePackage.ApplyPSDevCenter)
            return this;

        this.CenterOnMainWin();
        SaveValues.BasePackage.ApplyPSDevCenter = false;

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return Constants.BasePackage.UserInterface.DevModeCenter;
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = GUIUtility.Instantiate(_baseWindow);
        root.style.flexGrow = 1f;
        root.style.flexShrink = 1f;

        rootVisualElement.Add(root);

        _btnToggle = root.Q <Button>("BTN_Toggle");
        _btnInterfaceOverview = root.Q <Button>("BTN_InterfaceOverview");
        _btnRepaint = root.Q <Button>("BTN_Repaint");
        _btnReloadDomain = root.Q <Button>("BTN_ReloadDomain");

        RegisterCallbacks();
    }

    protected override bool LoadResources()
    {
        _baseWindow = Resources.Load <VisualTreeAsset>(BasePath());

        return _baseWindow != null;
    }

    protected override void RegisterCallbacks()
    {
        _btnToggle.clicked += OnToggle;
        _btnInterfaceOverview.clicked += OnInterfaceOverview;
        _btnRepaint.clicked += OnRepaint;
        _btnReloadDomain.clicked += OnReloadDomain;
    }

    protected override void UnRegisterCallbacks()
    {
        _btnToggle.clicked -= OnToggle;
        _btnInterfaceOverview.clicked -= OnInterfaceOverview;
        _btnRepaint.clicked -= OnRepaint;
        _btnReloadDomain.clicked -= OnReloadDomain;
    }

    #endregion

    #region Private Methods

    /// <summary> Open InterfaceOverview </summary>
    private static void OnInterfaceOverview()
    {
        ContextMenu.TryOpen <InterfaceOverview>(false);
    }

    /// <summary> Force reload the domain </summary>
    private static void OnReloadDomain()
    {
        EditorUtility.RequestScriptReload();
    }

    /// <summary> Call Force Repaint </summary>
    private static void OnRepaint()
    {
        GUIUtility.ForceRepaint();
    }

    /// <summary> Open Toggle </summary>
    private static void OnToggle()
    {
        ContextMenu.TryOpen <Toggle>(true);
    }

    #endregion
}

}
#endif
