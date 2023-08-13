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
        #region Const

        private const string PackageItem = "User Interface/MegaPintBasePackageItem";
        private const string SettingItem = "User Interface/MegaPintBaseSettingItem";

        private const string RightPaneContentBase = "User Interface/xxxDisplayContent";

        #endregion

        #region Visual References

        private VisualElement _rightPane;
        
        private Label _loading;
        
        private ListView _packagesList;
        private ListView _settingsList;

        private ToolbarSearchField _searchField;
        
        private Button _btnPackages;
        private Button _btnSettings;
        private Button _btnOpenPackageManager;
        
        #endregion

        #region Private

        /// <summary> Loaded uxml references </summary>
        private VisualTreeAsset _baseWindow;
        private VisualTreeAsset _packageItem;
        private VisualTreeAsset _settingItem;

        private MegaPintPackageManager.CachedPackages _allPackages;
        private List<MegaPintPackagesData.MegaPintPackageData> _displayedPackages;

        private int _currentLoadingLabelProgress;
        
        #endregion

        #region Override Methods

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

            #region References

            _btnOpenPackageManager = content.Q<Button>("OpenImporter");
            
            _rightPane = content.Q<VisualElement>("RightPane");

            _btnPackages = content.Q<Button>("BTN_Packages");
            _btnSettings = content.Q<Button>("BTN_Settings");

            _packagesList = content.Q<ListView>("PackagesList");
            _settingsList = content.Q<ListView>("SettingsList");
            
            _searchField = content.Q<ToolbarSearchField>("SearchField");
            _loading = content.Q<Label>("Loading");

            #endregion
            
            RegisterCallbacks();

            #region Packages List

            _packagesList.makeItem = () => _packageItem.Instantiate();
            
            _packagesList.bindItem = (element, i) =>
            {
                element.Q<Label>("PackageName").text = _displayedPackages[i].PackageNiceName;
            };
            
            _packagesList.destroyItem = element => element.Clear();

            #endregion

            #region Settings List

            _settingsList.makeItem = () => _settingItem.Instantiate();
            
            _settingsList.bindItem = (element, i) => { };
            
            _settingsList.destroyItem = element => element.Clear();

            #endregion
            
            _loading.style.display = DisplayStyle.Flex;
            _packagesList.style.display = DisplayStyle.None;
            _settingsList.style.display = DisplayStyle.None;

            MegaPintPackageManager.CachedPackages.RequestAllPackages();
            
            root.Add(content);
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
            MegaPintPackageManager.CachedPackages.OnUpdateActions.Add(
                new MegaPintPackageManager.CachedPackages.ListableAction(OnLoadingPackages, "BaseWindow"));
            
            MegaPintPackageManager.CachedPackages.OnCompleteActions
                .Add(new MegaPintPackageManager.CachedPackages.ListableAction<MegaPintPackageManager.CachedPackages>(OnPackagesLoaded, "BaseWindow"));

            _searchField.RegisterValueChangedCallback(OnSearchFieldChange);
            
            _packagesList.onSelectedIndicesChange += OnUpdateRightPane;
            
            _btnPackages.clicked += MegaPintPackageManager.CachedPackages.RequestAllPackages;
            _btnSettings.clicked += OnUpdateSettings;
            _btnOpenPackageManager.clicked += OnOpenPackageManager;
        }

        protected override void UnRegisterCallbacks()
        {
            MegaPintPackageManager.CachedPackages.RemoveUpdateAction("BaseWindow");
            MegaPintPackageManager.CachedPackages.RemoveCompleteAction("BaseWindow");

            _searchField.UnregisterValueChangedCallback(OnSearchFieldChange);
            
            _packagesList.onSelectedIndicesChange -= OnUpdateRightPane;
            
            _btnPackages.clicked -= MegaPintPackageManager.CachedPackages.RequestAllPackages;
            _btnSettings.clicked -= OnUpdateSettings;
            _btnOpenPackageManager.clicked += OnOpenPackageManager;
        }

        #endregion

        #region Callback Methods

        private Action OnLoadingPackages => () =>
        {
            _loading.style.display = DisplayStyle.Flex;
            _packagesList.style.display = DisplayStyle.None;
            
            MegaPintPackageManager.CachedPackages.UpdateLoadingLabel(
                _loading, 
                _currentLoadingLabelProgress, 
                30, 
                out _currentLoadingLabelProgress);
        };

        private Action<MegaPintPackageManager.CachedPackages> OnPackagesLoaded => packages =>
        {
            _loading.style.display = DisplayStyle.None;
            _packagesList.style.display = DisplayStyle.Flex;
            
            _currentLoadingLabelProgress = 0;
            
            _allPackages = packages;
            SetDisplayedPackages(_searchField.value);
        };
        
        private void OnUpdateRightPane(IEnumerable<int> _)
        {
            _rightPane.Clear();

            var currentPackageKey = ((MegaPintPackagesData.MegaPintPackageData)_packagesList.selectedItem).PackageKey;
            var contentPath = RightPaneContentBase.Replace("xxx", currentPackageKey.ToString());
            var content = Resources.Load<VisualTreeAsset>(contentPath);
            
            _rightPane.Add(content.Instantiate());
        }
        
        public static void OnOpenPackageManager() => ContextMenu.TryOpen<MegaPintPackageManagerWindow>(true, "Package Manager");

        private void OnSearchFieldChange(ChangeEvent<string> evt) => SetDisplayedPackages(_searchField.value);

        private void OnUpdateSettings()
        {
            SwitchState(1);
        }
        
        #endregion

        #region Internal Methods

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

        private void SwitchState(int page)
        {
            _packagesList.style.display = page == 0
                ? _allPackages == null ? DisplayStyle.None : DisplayStyle.Flex
                : DisplayStyle.None;
            
            _settingsList.style.display = page == 1 ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        #endregion
    }
}
#endif