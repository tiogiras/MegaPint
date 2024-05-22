#if UNITY_EDITOR
using System;
using System.IO;
using Editor.Scripts.PackageManager;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.Settings;
using Editor.Scripts.Windows.BaseWindowContent;
using Editor.Scripts.Windows.DevMode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = Editor.Scripts.GUI.GUIUtility;
using Toggle = Editor.Scripts.Windows.DevMode.Toggle;

namespace Editor.Scripts.Windows
{

internal class BaseWindow : MegaPintEditorWindowBase
{
    private const float MaxDevModeTimer = 50;
    private const int MaxDevModeClickCount = 10;

    public static Action onRightPaneInitialization;
    public static Action onRightPaneClose;

    private static bool _DevMode => MegaPintSettings.instance.GetSetting("General").GetValue("devMode", false);

    private readonly string _contentPath = Path.Combine(Constants.BasePackage.PathWindows, "Base Window");

    private VisualTreeAsset _baseWindow;

    private Button _btnDevMode;
    private Button _btnDevCenter;
    private Button _btnPackages;
    private Button _btnSettings;
    private Button _btnInfos;
    private Button _btnUpdate;

    private int _currentDevModeClickCount;
    private float _currentDevModeTimer;

    private PackagesTab _packages;
    private SettingsTab _settings;
    private InfosTab _infos;

    private VisualElement _root;
    
    private VisualElement _updateBasePackage;

    private Label _versionNumber;

    private bool _callbacksRegistered;

    #region Unity Event Functions

    private void OnGUI()
    {
        DisplayContent.onRightPaneGUI?.Invoke(rootVisualElement.Q <VisualElement>("RightPane"));

        if (_currentDevModeTimer > 0)
            _currentDevModeTimer -= Time.deltaTime;
        else
        {
            _currentDevModeTimer = 0;
            _currentDevModeClickCount = 0;
        }
    }

    #endregion

    #region Public Methods

    public static void OnOpenPackageManager()
    {
        ContextMenu.TryOpen <MegaPintPackageManagerWindow>(false, "Package Manager");
    }

    public override MegaPintEditorWindowBase ShowWindow()
    {
        titleContent.text = "MegaPint";

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return _contentPath;
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        _root = rootVisualElement;

        PackageCache.onCacheStartRefreshing += StartCacheRefresh;

        if (!PackageCache.WasInitialized)
            PackageCache.Refresh();
        else
            CreateGUIContent(_root);
    }

    protected override bool LoadResources()
    {
        _baseWindow = Resources.Load <VisualTreeAsset>(BasePath());

        return _baseWindow != null;
    }

    protected override void RegisterCallbacks()
    {
        _btnPackages.clicked += SwitchToPackages;
        _btnSettings.clicked += SwitchToSettings;
        _btnInfos.clicked += SwitchToInfos;

        _btnDevMode.clicked += OnDevMode;
        _btnDevCenter.clicked += OnDevCenter;
        _btnUpdate.clicked += UpdateBasePackage;

        _callbacksRegistered = true;
    }

    protected override void UnRegisterCallbacks()
    {
        if (!_callbacksRegistered)
            return;
        
        PackageCache.onCacheStartRefreshing -= StartCacheRefresh;

        _btnPackages.clicked -= SwitchToPackages;
        _btnSettings.clicked -= SwitchToSettings;
        _btnInfos.clicked -= SwitchToInfos;

        onRightPaneClose?.Invoke();

        _btnDevMode.clicked -= OnDevMode;
        _btnDevCenter.clicked -= OnDevCenter;
        _btnUpdate.clicked -= UpdateBasePackage;

        _callbacksRegistered = false;
    }

    #endregion

    #region Private Methods

    private static void UpdateBasePackage()
    {
        if (!EditorUtility.DisplayDialog(
                "Update MegaPint",
                $"Are you sure you want to upgrade\nMegaPint v{PackageCache.NewestBasePackageVersion}?",
                "Yes",
                "Abort"))
            return;

#pragma warning disable CS4014
        MegaPintPackageManager.UpdateBasePackage();
#pragma warning restore CS4014
    }

    private void CollectVisualReferences(VisualElement root)
    {
        _btnPackages = root.Q <Button>("BTN_Packages");
        _btnSettings = root.Q <Button>("BTN_Settings");
        _btnInfos = root.Q <Button>("BTN_Infos");

        _btnDevMode = root.Q <Button>("BTN_DevMode");
        _btnDevCenter = root.Q <Button>("BTN_DevCenter");
        _versionNumber = root.Q <Label>("VersionNumber");

        _updateBasePackage = root.Q <VisualElement>("UpdateBasePackage");
        _btnUpdate = _updateBasePackage.Q <Button>("BTN_Update");
    }

    private void CreateGUIContent(VisualElement root)
    {
        root.Clear();
        
        VisualElement content = GUIUtility.Instantiate(_baseWindow, root);

        CollectVisualReferences(content);

        RegisterCallbacks();

        _btnDevCenter.style.display = _DevMode ? DisplayStyle.Flex : DisplayStyle.None;
        
        _versionNumber.text = _DevMode ? "Development" : $"v{PackageCache.BasePackage.version}";
        _versionNumber.style.display = DisplayStyle.Flex;

        _packages = new PackagesTab(content);
        _settings = new SettingsTab(content);
        _infos = new InfosTab(content);

        SwitchToPackages();

        if (!PackageCache.NeedsBasePackageUpdate())
        {
            _updateBasePackage.style.display = DisplayStyle.None;

            return;
        }

        _updateBasePackage.style.display = DisplayStyle.Flex;
    }

    private void OnDevMode()
    {
        if (_currentDevModeClickCount == 0)
            _currentDevModeTimer = MaxDevModeTimer;

        _currentDevModeClickCount++;

        if (_currentDevModeClickCount < MaxDevModeClickCount)
            return;

        _currentDevModeClickCount = 0;
        ContextMenu.TryOpen <Toggle>(true);
    }
    
    private static void OnDevCenter()
    {
        ContextMenu.TryOpen <Center>(false);
    }

    private void StartCacheRefresh()
    {
        _packages?.ResetVariables();
        _settings?.ResetVariables();
        _infos?.ResetVariables();
        GUIUtility.DisplaySplashScreen(_root, () => {CreateGUIContent(_root);});
    }

    private void SwitchToPackages()
    {
        _packages.Show();
        _settings.Hide();
        _infos.Hide();
    }

    private void SwitchToSettings()
    {
        _packages.Hide();
        _settings.Show();
        _infos.Hide();
    }
    
    private void SwitchToInfos()
    {
        _packages.Hide();
        _settings.Hide();
        _infos.Show();
    }

    #endregion
}

}
#endif
