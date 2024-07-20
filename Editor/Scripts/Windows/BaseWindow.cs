#if UNITY_EDITOR
using System;
using MegaPint.Editor.Scripts.GUI.Utility;
using MegaPint.Editor.Scripts.PackageManager;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.Windows.BaseWindowContent;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Windows
{

internal class BaseWindow : EditorWindowBase
{
    public static Action onOpen;
    public static Action onClose;
    public static Action<string> onOpenWithLink;
    public static Action<string> onSwitchTab;

    public static Action <string> onPackageItemSelected;
    public static Action <string> onPackageItemTabSelected;
    public static Action <string> onInfoItemSelected;
    public static Action <string> onSettingItemSelected;
    
    private const float MaxDevModeTimer = 50;
    private const int MaxDevModeClickCount = 10;

    public static Action onRightPaneInitialization;
    public static Action onRightPaneClose;

    public static string openingLink;

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
    private bool _executeLinkOnGUICreation;
    private bool _guiContentCreated;

    private bool _hasPackages;
    private InfosTab _infos;

    private Label _noPackagesInstalled;

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

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "MegaPint";

        minSize = new Vector2(700, 350);

        if (!string.IsNullOrEmpty(openingLink))
        {
            if (_guiContentCreated)
                OpenWithLink();
            else 
                _executeLinkOnGUICreation = true;
        }
        
        onOpen?.Invoke();

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
        _btnDevCenter.clicked += ContextMenu.BasePackage.OpenDevCenter;
        _btnUpdate.clicked += UpdateBasePackage;

        _callbacksRegistered = true;
    }

    protected override void UnRegisterCallbacks()
    {
        onClose?.Invoke();
        
        if (!_callbacksRegistered)
            return;

        PackageCache.onCacheStartRefreshing -= StartCacheRefresh;

        if (_btnPackages == null)
        {
            _callbacksRegistered = false;

            return;
        }

        _btnPackages.clicked -= SwitchToPackages;
        _btnSettings.clicked -= SwitchToSettings;
        _btnInfos.clicked -= SwitchToInfos;

        onRightPaneClose?.Invoke();

        _btnDevMode.clicked -= OnDevMode;
        _btnDevCenter.clicked -= ContextMenu.BasePackage.OpenDevCenter;
        _btnUpdate.clicked -= UpdateBasePackage;

        _callbacksRegistered = false;
    }

    #endregion

    #region Private Methods

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

        _noPackagesInstalled = root.Q <Label>("NoPackagesInstalled");
    }

    private void CreateGUIContent(VisualElement root)
    {
        _guiContentCreated = true;

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

        _noPackagesInstalled.parent.ActivateLinks(
            link =>
            {
                if (link.linkID.Equals("PackageManager"))
                    ContextMenu.BasePackage.OpenPackageManager();
            });

        _hasPackages = PackageCache.HasPackagesInstalled();

        if (_executeLinkOnGUICreation)
        {
            if (!string.IsNullOrEmpty(openingLink))
                OpenWithLink();

            _executeLinkOnGUICreation = false;
        }
        else
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
        ContextMenu.BasePackage.OpenDevModeToggle();
    }

    // TODO commenting
    private void OpenWithLink()
    {
        onOpenWithLink?.Invoke(openingLink);
        
        var parts = openingLink.Split("/");

        switch (parts[0])
        {
            case "Packages":
                SwitchToPackages();

                DisplayContent.startTab = parts[2] switch
                                          {
                                              "Info" => DisplayContent.Tab.Info,
                                              "Settings" => DisplayContent.Tab.Settings,
                                              "Guides" => DisplayContent.Tab.Guides,
                                              "Help" => DisplayContent.Tab.Help,
                                              var _ => DisplayContent.Tab.Info
                                          };

                rootVisualElement.schedule.Execute(
                    () => {_packages.ShowByLink(parts[1]);});

                break;

            case "Info":
                SwitchToInfos();

                rootVisualElement.schedule.Execute(
                    () => {_infos.ShowByLink(parts[1..]);});

                break;

            case "Settings":
                SwitchToSettings();

                rootVisualElement.schedule.Execute(
                    () => {_settings.ShowByLink(parts[1..]);});

                break;
        }

        openingLink = "";
    }

    /// <summary> Prepare for refreshing the <see cref="PackageCache" /> </summary>
    private void StartCacheRefresh()
    {
        _guiContentCreated = false;

        _packages?.ResetVariables();
        _settings?.ResetVariables();
        _infos?.ResetVariables();
        GUIUtility.DisplaySplashScreen(_root, () => {CreateGUIContent(_root);});
    }

    /// <summary> Switch to the info tab </summary>
    private void SwitchToInfos()
    {
        onSwitchTab?.Invoke("Infos");
        
        _packages.Hide();
        _settings.Hide();
        _infos.Show();

        _noPackagesInstalled.style.display = DisplayStyle.None;

        GUIUtility.ToggleActiveButtonInGroup(2, _btnPackages, _btnSettings, _btnInfos);
    }

    /// <summary> Switch to the packages tab </summary>
    private void SwitchToPackages()
    {
        onSwitchTab?.Invoke("Packages");
        
        _packages.Show();
        _settings.Hide();
        _infos.Hide();

        _noPackagesInstalled.style.display = _hasPackages ? DisplayStyle.None : DisplayStyle.Flex;

        GUIUtility.ToggleActiveButtonInGroup(0, _btnPackages, _btnSettings, _btnInfos);
    }

    /// <summary> Switch to the settings tab </summary>
    private void SwitchToSettings()
    {
        onSwitchTab?.Invoke("Settings");
        
        _packages.Hide();
        _settings.Show();
        _infos.Hide();

        _noPackagesInstalled.style.display = DisplayStyle.None;

        GUIUtility.ToggleActiveButtonInGroup(1, _btnPackages, _btnSettings, _btnInfos);
    }

    #endregion
}

}
#endif
