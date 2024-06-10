#if UNITY_EDITOR
using System;
using MegaPint.Editor.Scripts.GUI.Utility;
using MegaPint.Editor.Scripts.PackageManager;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.Windows.BaseWindowContent;
using MegaPint.Editor.Scripts.Windows.DevMode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;
using Toggle = MegaPint.Editor.Scripts.Windows.DevMode.Toggle;

namespace MegaPint.Editor.Scripts.Windows
{

internal class BaseWindow : EditorWindowBase
{
    private const float MaxDevModeTimer = 50;
    private const int MaxDevModeClickCount = 10;

    public static Action onRightPaneInitialization;
    public static Action onRightPaneClose;

    private readonly string _contentPath = Constants.BasePackage.UserInterface.BaseWindow;

    private VisualTreeAsset _baseWindow;
    private Button _btnDevCenter;

    private Button _btnDevMode;
    private Button _btnInfos;
    private Button _btnPackages;
    private Button _btnSettings;
    private Button _btnUpdate;

    private bool _callbacksRegistered;

    private int _currentDevModeClickCount;
    private float _currentDevModeTimer;
    private InfosTab _infos;

    private PackagesTab _packages;

    private VisualElement _root;
    private SettingsTab _settings;

    private VisualElement _updateBasePackage;

    private Label _versionNumber;

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
        ContextMenu.TryOpen <PackageManager>(false, "Package Manager");
    }

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "MegaPint";
        
        minSize = new Vector2(700, 350);

        if (!SaveValues.BasePackage.ApplyPSBaseWindow)
            return this;

        this.CenterOnMainWin(900, 450);
        SaveValues.BasePackage.ApplyPSBaseWindow = false;

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

        _btnPackages.clicked -= SwitchToPackages; // TODO NULLREFS
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

    /// <summary> Open the development center </summary>
    private static void OnDevCenter()
    {
        ContextMenu.TryOpen <Center>(false);
    }

    /// <summary> Update the BasePackage </summary>
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

    /// <summary> Collect the specified visual references </summary>
    /// <param name="root"> RootVisualElement </param>
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

        _btnDevCenter.style.display = SaveValues.BasePackage.DevMode ? DisplayStyle.Flex : DisplayStyle.None;

        _versionNumber.text = SaveValues.BasePackage.DevMode ? "Development" : $"v{PackageCache.BasePackage.version}";
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

    /// <summary> Try to open the development center </summary>
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

    /// <summary> Prepare for refreshing the <see cref="PackageCache" /> </summary>
    private void StartCacheRefresh()
    {
        _packages?.ResetVariables();
        _settings?.ResetVariables();
        _infos?.ResetVariables();
        GUIUtility.DisplaySplashScreen(_root, () => {CreateGUIContent(_root);});
    }

    /// <summary> Switch to the info tab </summary>
    private void SwitchToInfos()
    {
        _packages.Hide();
        _settings.Hide();
        _infos.Show();
    }

    /// <summary> Switch to the packages tab </summary>
    private void SwitchToPackages()
    {
        _packages.Show();
        _settings.Hide();
        _infos.Hide();
    }

    /// <summary> Switch to the settings tab </summary>
    private void SwitchToSettings()
    {
        _packages.Hide();
        _settings.Show();
        _infos.Hide();
    }

    #endregion
}

}
#endif
