#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.PackageManager;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.PackageManager.Packages;
using Editor.Scripts.PackageManager.Utility;
using Editor.Scripts.Settings;
using Editor.Scripts.Settings.BaseSettings;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Windows
{
    internal class MegaPintBaseWindow : MegaPintEditorWindowBase
    {
        #region Const

        private const string PackageItem = "User Interface/MegaPintBasePackageItem";
        private const string SettingItem = "User Interface/MegaPintBaseSettingItem";

        private const string RightPaneContentBase = "xxx/User Interface/DisplayContent";

        #endregion

        #region Visual References

        private VisualElement _rightPane;
        
        private Label _loading;
        private Label _devMode;
        private Label _versionNumber;
        
        private ListView _packagesList;
        private ListView _settingsList;

        private ToolbarSearchField _searchField;
        
        private Button _btnPackages;
        private Button _btnSettings;
        private Button _btnOpenPackageManager;
        private Button _btnDevMode;
        
        #endregion

        #region Private

        /// <summary> Loaded uxml references </summary>
        private VisualTreeAsset _baseWindow;
        private VisualTreeAsset _packageItem;
        private VisualTreeAsset _settingItem;
        
        private List<CachedPackage> _displayedPackages;

        private VisualElement _currentVisualElementPackages;
        private List<VisualElement> _visualElementsPackages;
        
        private VisualElement _currentVisualElementSettings;
        private List<VisualElement> _visualElementsSettings;

        private List<MegaPintBaseSettingsData.Setting> _openSettings;
        private List<MegaPintBaseSettingsData.Setting> _displayedSettings;
        private List<MegaPintBaseSettingsData.Setting> _allSettings;

        private int _currentLoadingLabelProgress;

        private bool _inSearch;

        private const float MaxDevModeTimer = 50;
        private float _currentDevModeTimer;

        private const int MaxDevModeClickCount = 10;
        private int _currentDevModeClickCount;

        #endregion

        public static Action onRightPaneInitialization;
        public static Action onRightPaneClose;

        #region Override Methods

        protected override string BasePath() => "User Interface/MegaPintBaseWindow";
        
        public override MegaPintEditorWindowBase ShowWindow()
        {
            titleContent.text = "MegaPint";
            return this;
        }

        private static bool _DevMode => MegaPintSettings.instance.GetSetting("General").GetValue("devMode", false);

        protected override void CreateGUI()
        {
            base.CreateGUI();

            VisualElement root = rootVisualElement;

            VisualElement content = _baseWindow.Instantiate();

            #region References

            _btnOpenPackageManager = content.Q<Button>("OpenImporter");
            
            _rightPane = content.Q<VisualElement>("RightPane");

            _btnPackages = content.Q<Button>("BTN_Packages");
            _btnSettings = content.Q<Button>("BTN_Settings");

            _packagesList = content.Q<ListView>("PackagesList");
            _settingsList = content.Q<ListView>("SettingsList");
            
            _searchField = content.Q<ToolbarSearchField>("SearchField");
            _loading = content.Q<Label>("Loading");

            _btnDevMode = content.Q <Button>("BTN_DevMode");
            _devMode = content.Q <Label>("DevMode");
            _versionNumber = content.Q <Label>("VersionNumber");

            #endregion
            
            RegisterCallbacks();

            #region Packages List

            _packagesList.makeItem = () => _packageItem.Instantiate();
            
            _packagesList.bindItem = (element, i) =>
            {
                _visualElementsPackages ??= new List<VisualElement>();
                _visualElementsPackages.Add(element);
                
                var packageNameLabel = element.Q<Label>("PackageName");
                packageNameLabel.text = _displayedPackages[i].DisplayName;
                packageNameLabel.style.borderLeftWidth = 0;
            };
            
            _packagesList.destroyItem = element =>
            {
                _visualElementsPackages ??= new List<VisualElement>();
                
                if (_visualElementsPackages.Contains(element))
                    _visualElementsPackages.Remove(element);
                
                element.Clear();
            };

            #endregion

            #region Settings List

            _settingsList.makeItem = () => _settingItem.Instantiate();
            
            _settingsList.bindItem = (element, i) =>
            {
                _openSettings ??= new List<MegaPintBaseSettingsData.Setting>();
                
                _visualElementsSettings ??= new List<VisualElement>();

                if (!_visualElementsSettings.Contains(element))
                    _visualElementsSettings.Add(element);

                var setting = _displayedSettings[i];

                var nameLabel = element.Q<Label>("Name");
                nameLabel.text = setting.settingName;
                nameLabel.style.borderLeftWidth = _currentVisualElementSettings == element 
                    ? 2.5f
                    : 0;
                
                element.style.marginLeft = _inSearch 
                    ? 0
                    : setting.intendLevel * 10;

                element.Q<VisualElement>("Open").style.display = _openSettings.Contains(setting) 
                    ? DisplayStyle.Flex 
                    : DisplayStyle.None;
                
                element.Q<VisualElement>("Closed").style.display = setting.subSettings is { Count: > 0 }
                    ? _openSettings.Contains(setting) 
                        ? DisplayStyle.None 
                        : DisplayStyle.Flex
                    : DisplayStyle.None;
            };
            
            _settingsList.destroyItem = element =>
            {
                _visualElementsSettings ??= new List<VisualElement>();

                if (_visualElementsSettings.Contains(element))
                    _visualElementsSettings.Remove(element);

                element.Clear();
            };

            #endregion
            
            _loading.style.display = DisplayStyle.Flex;
            _packagesList.style.display = DisplayStyle.None;
            _settingsList.style.display = DisplayStyle.None;

            _versionNumber.style.display = DisplayStyle.None;
            _devMode.style.display = _DevMode ? DisplayStyle.Flex : DisplayStyle.None;

            PackageCache.Refresh();

            root.Add(content);
        }

        private void OnGUI()
        {
            DisplayContent.onRightPaneGUI?.Invoke(rootVisualElement.Q<VisualElement>("RightPane"));
            
            if (_currentDevModeTimer > 0)
                _currentDevModeTimer -= Time.deltaTime;
            else
            {
                _currentDevModeTimer = 0;
                _currentDevModeClickCount = 0;
            }
        }

        protected override bool LoadResources()
        {
            _baseWindow = Resources.Load<VisualTreeAsset>(BasePath());
            _packageItem = Resources.Load<VisualTreeAsset>(PackageItem);
            _settingItem = Resources.Load<VisualTreeAsset>(SettingItem);

            return _baseWindow != null && _packageItem != null && _settingItem != null;
        }

        protected override void RegisterCallbacks()
        {
            MegaPintPackageManager.onRefreshingPackages += _onLoadingPackages;
            PackageCache.onCacheRefreshed += _onPackagesLoaded;

            _searchField.RegisterValueChangedCallback(OnSearchFieldChange);
            
            _packagesList.selectedIndicesChanged += OnUpdateRightPane;
            _settingsList.selectedIndicesChanged += OnRefreshSettings;

            _btnPackages.clicked += PackageCache.Refresh;
            _btnSettings.clicked += OnUpdateSettings;
            _btnOpenPackageManager.clicked += OnOpenPackageManager;

            _btnDevMode.clicked += OnDevMode;
        }

        protected override void UnRegisterCallbacks()
        {
            MegaPintPackageManager.onRefreshingPackages -= _onLoadingPackages;
            PackageCache.onCacheRefreshed -= _onPackagesLoaded;

            _searchField.UnregisterValueChangedCallback(OnSearchFieldChange);
            
            _packagesList.selectedIndicesChanged -= OnUpdateRightPane;
            _settingsList.selectedIndicesChanged -= OnRefreshSettings;
            
            _btnPackages.clicked -= PackageCache.Refresh;
            _btnSettings.clicked -= OnUpdateSettings;
            _btnOpenPackageManager.clicked -= OnOpenPackageManager;
            
            onRightPaneClose?.Invoke();
            
            _btnDevMode.clicked -= OnDevMode;
        }

        #endregion

        #region Callback Methods

        private void OnDevMode()
        {
            if (_currentDevModeClickCount == 0)
                _currentDevModeTimer = MaxDevModeTimer;

            _currentDevModeClickCount++;
            
            if (_currentDevModeClickCount < MaxDevModeClickCount)
                return;

            _currentDevModeClickCount = 0;
            ContextMenu.TryOpen<MegaPintDevMode>(true);
        }
        
        private Action _onLoadingPackages => () =>
        {
            _loading.style.display = DisplayStyle.Flex;
            _packagesList.style.display = DisplayStyle.None;
            
            PackageManagerUtility.UpdateLoadingLabel(
                _loading, 
                _currentLoadingLabelProgress, 
                30, 
                out _currentLoadingLabelProgress);
        };

        private Action _onPackagesLoaded => () =>
        {
            _loading.style.display = DisplayStyle.None;
            _packagesList.style.display = DisplayStyle.Flex;

            _versionNumber.text = _DevMode ? "Development" : $"v{PackageCache.BasePackage.version}";
            _versionNumber.style.display = DisplayStyle.Flex;
            
            _currentLoadingLabelProgress = 0;
            
            SetDisplayedPackages(_searchField.value);
        };

        private void OnUpdateRightPane(IEnumerable<int> _)
        {
            ClearRightPane();

            if (_packagesList.style.display == DisplayStyle.Flex)
            {
                if (_packagesList.selectedItem == null)
                    return;

                if (_currentVisualElementPackages != null)
                    _currentVisualElementPackages.style.borderLeftWidth = 0;

                var visualElement = _visualElementsPackages[_packagesList.selectedIndex].Q<Label>("PackageName");
                visualElement.style.borderLeftWidth = 2.5f;
                _currentVisualElementPackages = visualElement;

                PackageKey currentPackageKey = ((CachedPackage)_packagesList.selectedItem).Key;
                var contentPath = RightPaneContentBase.Replace("xxx", currentPackageKey.ToString());
                TemplateContainer content = Resources.Load<VisualTreeAsset>(contentPath).Instantiate();
            
                DisplayContent.DisplayRightPane(currentPackageKey, content);
            
                _rightPane.Add(content);
            }

            if (_settingsList.style.display == DisplayStyle.Flex)
            {
                if (_settingsList.selectedItem == null)
                    return;

                var currentSettingKey = ((MegaPintBaseSettingsData.Setting)_settingsList.selectedItem).settingKey;
                MegaPintBaseSettingsDisplay.Display(_rightPane, currentSettingKey);
            }
            
            onRightPaneInitialization?.Invoke();
        }
        
        public static void OnOpenPackageManager() => ContextMenu.TryOpen<MegaPintPackageManagerWindow>(true, "Package Manager");

        private void OnSearchFieldChange(ChangeEvent<string> evt)
        {
            if (_packagesList.style.display == DisplayStyle.Flex)
                SetDisplayedPackages(_searchField.value);

            if (_settingsList.style.display == DisplayStyle.Flex)
                SetDisplayedSettings(_searchField.value);
        }

        private void OnUpdateSettings()
        {
            _displayedSettings = MegaPintBaseSettingsData.Settings;
            _openSettings = new List<MegaPintBaseSettingsData.Setting>();
            _visualElementsSettings = new List<VisualElement>();

            _inSearch = false;
            
            _settingsList.itemsSource = _displayedSettings;
            _settingsList.RefreshItems();
            
            SwitchState(1);
            
            if (_allSettings != null) 
                return;
            
            _allSettings = new List<MegaPintBaseSettingsData.Setting>();
                
            foreach (var setting in MegaPintBaseSettingsData.Settings)
            {
                _allSettings.AddRange(GetAllSettings(setting));
            }
        }

        private void OnRefreshSettings(IEnumerable<int> _)
        {
            if (_settingsList.selectedItem == null)
                return;

            var castedItem = (MegaPintBaseSettingsData.Setting)_settingsList.selectedItem;

            if (_currentVisualElementSettings != null)
            {
                _currentVisualElementSettings.Q<Label>("Name").style.borderLeftWidth = 0;
                _currentVisualElementSettings = null;
            }

            if (castedItem.subSettings is { Count: > 0 })
            {
                _displayedSettings = new List<MegaPintBaseSettingsData.Setting>();

                _openSettings ??= new List<MegaPintBaseSettingsData.Setting>();

                _visualElementsSettings = new List<VisualElement>();
                
                if (_openSettings.Contains(castedItem))
                    _openSettings.Remove(castedItem);
                else
                    _openSettings.Add(castedItem);
                
                foreach (var setting in MegaPintBaseSettingsData.Settings)
                {
                    AddSetting(setting);
                }

                _settingsList.itemsSource = _displayedSettings;
                _settingsList.RefreshItems();
            }
            else
            {
                _currentVisualElementSettings = _visualElementsSettings[_settingsList.selectedIndex];
                _currentVisualElementSettings.Q<Label>("Name").style.borderLeftWidth = 2.5f;

                OnUpdateRightPane(_); 
            }
            
            _settingsList.ClearSelection();
        }
        
        #endregion

        #region Internal Methods

        private void AddSetting(MegaPintBaseSettingsData.Setting setting)
        {
            _displayedSettings.Add(setting);

            if (setting.subSettings is not {Count: > 0})
                return;
            
            if (!_openSettings.Contains(setting))
                return;

            foreach (var subSetting in setting.subSettings)
            {
                AddSetting(subSetting);
            }
        }
        
        private void ClearRightPane()
        {
            onRightPaneClose?.Invoke();
            _rightPane.Clear();
        }

        private void SetDisplayedPackages(string searchString)
        {
            _loading.style.display = DisplayStyle.None;
            SwitchState(0);

            List <CachedPackage> allPackages = PackageCache.GetAllMpPackages();
            
            _displayedPackages = searchString.Equals("") ? 
                allPackages.Where(package => package.IsInstalled).ToList() :
                allPackages.Where(package => package.IsInstalled)
                           .Where(package => package.DisplayName.ToLower().Contains(searchString.ToLower())).ToList();
            
            _displayedPackages.Sort();

            _packagesList.itemsSource = _displayedPackages;
            _packagesList.RefreshItems();
        }
        
        private void SetDisplayedSettings(string searchString)
        {
            _displayedSettings = searchString.Equals("")
                ? MegaPintBaseSettingsData.Settings
                : _allSettings.Where(setting => setting.settingName.ToLower().Contains(searchString.ToLower())).ToList();

            if (!searchString.Equals(""))
            {
                _inSearch = true;
                _displayedSettings.Sort();
            }
            else
                _inSearch = false;

            _openSettings = new List<MegaPintBaseSettingsData.Setting>();
            _visualElementsSettings = new List<VisualElement>();
            
            _settingsList.ClearSelection();
            _currentVisualElementSettings = null;

            _settingsList.itemsSource = _displayedSettings;
            _settingsList.RefreshItems();
        }

        private List<MegaPintBaseSettingsData.Setting> GetAllSettings(MegaPintBaseSettingsData.Setting setting)
        {
            var result = new List<MegaPintBaseSettingsData.Setting>();
            var isCategory = setting.subSettings is { Count: > 0 };
            
            if (!isCategory)
                result.Add(setting);
            else
            {
                foreach (var subSetting in setting.subSettings)
                {
                    result.AddRange(GetAllSettings(subSetting));
                }
            }
            
            return result;
        }

        private void SwitchState(int page)
        {
            _packagesList.ClearSelection();
            _settingsList.ClearSelection();
            
            _packagesList.style.display = page == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            _settingsList.style.display = page == 1 ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        #endregion
    }
}
#endif