#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.PackageManager;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.PackageManager.Packages;
using Editor.Scripts.Settings;
using Editor.Scripts.Settings.BaseSettings;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = Editor.Scripts.GUI.GUIUtility;

namespace Editor.Scripts.Windows
{
    internal class MegaPintBaseWindow : MegaPintEditorWindowBase
    {
        #region Const

        private const string PackageItem = "MegaPint/User Interface/Windows/Base Window/Package Item";
        private const string SettingItem = "MegaPint/User Interface/Windows/Base Window/Setting Item";

        private const string RightPaneContentBase = "xxx/User Interface/Display Content";

        #endregion

        #region Visual References
        
        private VisualElement _root;

        private VisualElement _rightPane;
        private VisualElement _updateBasePackage;
        
        private Label _versionNumber;
        
        private ListView _packagesList;
        private ListView _settingsList;

        private ToolbarSearchField _searchField;
        
        private Button _btnPackages;
        private Button _btnSettings;
        private Button _btnDevMode;
        private Button _btnUpdate;

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

        private bool _inSearch;

        private const float MaxDevModeTimer = 50;
        private float _currentDevModeTimer;

        private const int MaxDevModeClickCount = 10;
        private int _currentDevModeClickCount;

        #endregion

        public static Action onRightPaneInitialization;
        public static Action onRightPaneClose;

        #region Override Methods

        protected override string BasePath() => "MegaPint/User Interface/Windows/Base Window";
        
        public override MegaPintEditorWindowBase ShowWindow()
        {
            titleContent.text = "MegaPint";
            return this;
        }

        private static bool _DevMode => MegaPintSettings.instance.GetSetting("General").GetValue("devMode", false);

        protected override void CreateGUI()
        {
            base.CreateGUI();

            _root = rootVisualElement;

            PackageCache.onCacheStartRefreshing += StartCacheRefresh;
            
            if (!PackageCache.WasInitialized)
            {
                PackageCache.Refresh();
            }
            else
                CreateGUIContent(_root);
        }

        private void StartCacheRefresh()
        {
            GUIUtility.DisplaySplashScreen(_root, () => {CreateGUIContent(_root);});
        }

        private void CreateGUIContent(VisualElement root)
        {
            VisualElement content = _baseWindow.Instantiate();
            GUIUtility.ApplyTheme(content);
            
            root.Add(content);

            #region References

            _rightPane = content.Q<VisualElement>("RightPane");

            _btnPackages = content.Q<Button>("BTN_Packages");
            _btnSettings = content.Q<Button>("BTN_Settings");

            _packagesList = content.Q<ListView>("PackagesList");
            _settingsList = content.Q<ListView>("SettingsList");
            
            _searchField = content.Q<ToolbarSearchField>("SearchField");

            _btnDevMode = content.Q <Button>("BTN_DevMode");
            _versionNumber = content.Q <Label>("VersionNumber");

            _updateBasePackage = content.Q <VisualElement>("UpdateBasePackage");
            _btnUpdate = _updateBasePackage.Q <Button>("BTN_Update");

            #endregion
            
            RegisterCallbacks();

            #region Packages List

            _packagesList.makeItem = () => {
                TemplateContainer item = _packageItem.Instantiate();
                GUIUtility.ApplyTheme(item);
                
                return item;
            };
            
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

            _settingsList.makeItem = () =>
            {
                TemplateContainer item = _settingItem.Instantiate();
                GUIUtility.ApplyTheme(item);
                
                return item;
            };
            
            _settingsList.bindItem = (element, i) =>
            {
                _openSettings ??= new List<MegaPintBaseSettingsData.Setting>();
                
                _visualElementsSettings ??= new List<VisualElement>();

                if (!_visualElementsSettings.Contains(element))
                    _visualElementsSettings.Add(element);

                MegaPintBaseSettingsData.Setting setting = _displayedSettings[i];

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
            
            _packagesList.style.display = DisplayStyle.Flex;
            _settingsList.style.display = DisplayStyle.None;
            
            _versionNumber.text = _DevMode ? "Development" : $"v{PackageCache.BasePackage.version}";
            _versionNumber.style.display = DisplayStyle.Flex;

            SetDisplayedPackages(_searchField.value);

            if (!PackageCache.NeedsBasePackageUpdate())
            {
                _updateBasePackage.style.display = DisplayStyle.None;
                return;
            }

            _updateBasePackage.style.display = DisplayStyle.Flex;
            _btnUpdate.clicked += UpdateBasePackage;
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
            _searchField.RegisterValueChangedCallback(OnSearchFieldChange);
            
            _packagesList.selectedIndicesChanged += OnUpdateRightPane;
            _settingsList.selectedIndicesChanged += OnRefreshSettings;

            _btnPackages.clicked += OnUpdatePackages;
            _btnSettings.clicked += OnUpdateSettings;

            _btnDevMode.clicked += OnDevMode;
        }

        protected override void UnRegisterCallbacks()
        {
            PackageCache.onCacheStartRefreshing -= StartCacheRefresh;
            
            _searchField.UnregisterValueChangedCallback(OnSearchFieldChange);
            
            _packagesList.selectedIndicesChanged -= OnUpdateRightPane;
            _settingsList.selectedIndicesChanged -= OnRefreshSettings;
            
            _btnPackages.clicked -= OnUpdatePackages;
            _btnSettings.clicked -= OnUpdateSettings;

            onRightPaneClose?.Invoke();
            
            _btnDevMode.clicked -= OnDevMode;
            _btnUpdate.clicked -= UpdateBasePackage;
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

        private void UpdateBasePackage()
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

                var template = Resources.Load <VisualTreeAsset>(contentPath);
                
                if (template == null)
                    return;
                
                TemplateContainer content = template.Instantiate();
            
                DisplayContent.DisplayRightPane(currentPackageKey, content);
            
                _rightPane.Add(content);
            }
            
            _packagesList.ClearSelection();

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

        private void OnUpdatePackages()
        {
            _searchField.value = "";
            SetDisplayedPackages("");
            _rightPane.Clear();
        }
        
        private void OnUpdateSettings()
        {
            _searchField.value = "";
            SetDisplayedSettings("");
            
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