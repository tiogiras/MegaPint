#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.PackageManager;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Windows
{
    public class MegaPintBaseWindow : MegaPintEditorWindowBase
    {
        private const string PackageItem = "User Interface/MegaPintBasePackageItem";
        private const string SettingItem = "User Interface/MegaPintBaseSettingItem";

        private const string RightPaneContentBase = "User Interface/xxxDisplayContent";
        
        /// <summary> Loaded reference of the uxml </summary>
        private VisualTreeAsset _baseWindow;
        private VisualTreeAsset _packageItem;
        private VisualTreeAsset _settingItem;

        private VisualElement _rightPane;
        
        private Button _btnPackages;
        private Button _btnSettings;

        private ListView _packagesList;
        private ListView _settingsList;

        private ToolbarSearchField _searchField;
        
        private Label _loading;

        private MegaPintPackageManager.CachedPackages _allPackages;
        private List<MegaPintPackagesData.MegaPintPackageData> _displayedPackages;

        #region Overrides

        protected override string BasePath() => "User Interface/MegaPintBaseWindow";
        
        public override MegaPintEditorWindowBase ShowWindow()
        {
            titleContent.text = "MegaPint";
            return this;
        }

        protected override void CreateGUI()
        {
            base.CreateGUI();

            var root = rootVisualElement;

            VisualElement content = _baseWindow.Instantiate();

            content.Q<Button>("OpenImporter").clicked += OpenImporter;

            _rightPane = content.Q<VisualElement>("RightPane");

            _btnPackages = content.Q<Button>("BTN_Packages");
            _btnSettings = content.Q<Button>("BTN_Settings");

            _btnPackages.clicked += UpdatePackages;
            _btnSettings.clicked += UpdateSettings;

            _packagesList = content.Q<ListView>("PackagesList");
            _settingsList = content.Q<ListView>("SettingsList");
            
            _packagesList.makeItem = () => _packageItem.Instantiate();
            _settingsList.makeItem = () => _settingItem.Instantiate();

            _packagesList.bindItem = (element, i) =>
            {
                element.Q<Label>("PackageName").text = _displayedPackages[i].PackageNiceName;
            };
            
            _settingsList.bindItem = (element, i) => { };

            _packagesList.destroyItem = element => element.Clear();
            _settingsList.destroyItem = element => element.Clear();
            
            _packagesList.onSelectedIndicesChange += _ => UpdateRightPane();
            
            _loading = content.Q<Label>("Loading");
            
            _loading.style.display = DisplayStyle.Flex;
            _packagesList.style.display = DisplayStyle.None;
            _settingsList.style.display = DisplayStyle.None;

            _searchField = content.Q<ToolbarSearchField>("SearchField");
            _searchField.RegisterValueChangedCallback(SearchFieldChange);

            MegaPintPackageManager.OnSuccess += UpdatePackages;
            MegaPintPackageManager.CachedPackages.OnRefreshed += Refresh;
            
            UpdatePackages();
            
            root.Add(content);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            MegaPintPackageManager.OnSuccess -= UpdatePackages;
            MegaPintPackageManager.CachedPackages.OnRefreshed -= Refresh;
            
            _searchField.UnregisterValueChangedCallback(SearchFieldChange);
            
            _btnPackages.clicked -= UpdatePackages;
            _btnSettings.clicked -= UpdateSettings;
        }

        protected override bool LoadResources()
        {
            _baseWindow = Resources.Load<VisualTreeAsset>(BasePath());
            _packageItem = Resources.Load<VisualTreeAsset>(PackageItem);
            _settingItem = Resources.Load<VisualTreeAsset>(SettingItem);

            return _baseWindow != null && _packageItem != null && _settingItem != null;
        }

        #endregion
        
        private void UpdateRightPane()
        {
            _rightPane.Clear();

            var currentPackageKey = ((MegaPintPackagesData.MegaPintPackageData)_packagesList.selectedItem).PackageKey;
            var contentPath = RightPaneContentBase.Replace("xxx", currentPackageKey.ToString());
            var content = Resources.Load<VisualTreeAsset>(contentPath);
            
            _rightPane.Add(content.Instantiate());
        }
        
        public static void OpenImporter() => ContextMenu.TryOpen<MegaPintPackageManagerWindow>(true);

        private void SearchFieldChange(ChangeEvent<string> evt) => SetDisplayedPackages(_searchField.value);

        private void Refresh(MegaPintPackageManager.CachedPackages packages)
        {
            _allPackages = packages;
            SetDisplayedPackages(_searchField.value);
        }
        
        private void UpdatePackages()
        {
            MegaPintPackageManager.CachedPackages.AllPackages(_loading, packages =>
            {
                _allPackages = packages;
                SetDisplayedPackages(_searchField.value);
            });
        }
        
        private void SetDisplayedPackages(string searchString)
        {
            _loading.style.display = DisplayStyle.None;
            SwitchState(0);
            
            _displayedPackages = searchString.Equals("") ? 
                _allPackages.ToDisplay().Where(package => _allPackages.IsImported(package.PackageKey)).ToList() :
                _allPackages.ToDisplay().Where(package => _allPackages.IsImported(package.PackageKey))
                    .Where(package => package.PackageNiceName.ToLower().Contains(searchString.ToLower())).ToList();
            
            _displayedPackages.Sort();

            _packagesList.itemsSource = _displayedPackages;
            _packagesList.RefreshItems();
        }

        public void UpdateSettings()
        {
            SwitchState(1);
        }

        private void SwitchState(int page)
        {
            _packagesList.style.display = page == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            _settingsList.style.display = page == 1 ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
#endif