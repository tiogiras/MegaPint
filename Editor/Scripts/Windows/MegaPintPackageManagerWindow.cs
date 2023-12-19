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
    public class MegaPintPackageManagerWindow : MegaPintEditorWindowBase
    {
        #region Const

        private const string ListItemTemplate = "User Interface/Import/MegaPintPackageItem";
        
        private readonly Color _normalColor = new (0.823529422f, 0.823529422f, 0.823529422f);
        private readonly Color _wrongVersionColor = new (0.688679218f,0.149910346f,0.12019401f);

        #endregion

        #region Visual References

        private GroupBox _rightPane;
        private GroupBox _content;
        
        private Label _loading;
        private Label _packageName;
        private Label _version;
        private Label _lastUpdate;
        private Label _unityVersion;
        private Label _megaPintVersion;
        private Label _infoText;
        
        private ScrollView _packages;

        private ListView _list;
        
        private ToolbarSearchField _packageSearch;
        
        private Button _btnImport;
        private Button _btnRemove;
        private Button _btnUpdate;
        
        #endregion

        #region Private

        /// <summary> Loaded uxml references </summary>
        private VisualTreeAsset _baseWindow;
        private VisualTreeAsset _listItem;

        private MegaPintPackageManager.CachedPackages _allPackages;
        private List<MegaPintPackagesData.MegaPintPackageData> _displayedPackages;
        private MegaPintPackagesData.MegaPintPackageData _selectedPackage;

        private int _currentLoadingLabelProgress;

        #endregion

        #region Override Methods

        protected override string BasePath() => "User Interface/Import/MegaPintPackageManager";

        public override MegaPintEditorWindowBase ShowWindow()
        {
            titleContent.text = "Package Manager";
            return this;
        }
        
        protected override void CreateGUI()
        {
            base.CreateGUI();

            var root = rootVisualElement;

            VisualElement content = _baseWindow.Instantiate();

            #region References

            _packages = content.Q<ScrollView>("Packages");
            _list = content.Q<ListView>("MainList");
            _loading = content.Q<Label>("Loading");
            
            _rightPane = content.Q<GroupBox>("RightPane");
            _packageName = _rightPane.Q<Label>("PackageName");
            _content = _rightPane.Q<GroupBox>("Content");
            _version = _content.Q<Label>("NewestVersion");
            _lastUpdate = _content.Q<Label>("LastUpdate");
            _unityVersion = _content.Q<Label>("UnityVersion");
            _megaPintVersion = _content.Q<Label>("MegaPintVersion");
            _infoText = content.Q <Label>("InfoText");

            _btnImport = _rightPane.Q<Button>("BTN_Import");
            _btnRemove = _rightPane.Q<Button>("BTN_Remove");
            _btnUpdate = _rightPane.Q<Button>("BTN_Update");
            
            _packageSearch = content.Q<ToolbarSearchField>("PackageSearch");

            #endregion
            
            RegisterCallbacks();
            
            #region List
            
            _list.makeItem = () => _listItem.Instantiate();

            _list.bindItem = UpdateItem;

            _list.destroyItem = element => element.Clear();

            #endregion

            _loading.style.display = DisplayStyle.Flex;
            _packages.style.display = DisplayStyle.None;
            
            MegaPintPackageManager.CachedPackages.RequestAllPackages();

            OnUpdateRightPane();

            root.Add(content);
        }

        protected override bool LoadResources()
        {
            _baseWindow = Resources.Load<VisualTreeAsset>(BasePath());
            _listItem = Resources.Load<VisualTreeAsset>(ListItemTemplate);
            return _baseWindow != null && _listItem != null;
        }

        protected override void RegisterCallbacks()
        {
            MegaPintPackageManager.CachedPackages.OnUpdateActions.Add(
                new MegaPintPackageManager.CachedPackages.ListableAction(_onLoadingPackages, "PackageManager"));
            
            MegaPintPackageManager.CachedPackages.OnCompleteActions
                .Add(new MegaPintPackageManager.CachedPackages.ListableAction<MegaPintPackageManager.CachedPackages>(_onPackagesLoaded, "PackageManager"));
            
            _packageSearch.RegisterValueChangedCallback(OnSearchStringChanged);
            
            _list.selectedIndicesChanged += OnUpdateRightPane;
            
            _btnImport.clicked += OnImport;
            _btnRemove.clicked += OnRemove;
            _btnUpdate.clicked += OnUpdate;
        }

        protected override void UnRegisterCallbacks()
        {
            MegaPintPackageManager.CachedPackages.RemoveUpdateAction("PackageManager");
            MegaPintPackageManager.CachedPackages.RemoveCompleteAction("PackageManager");
            
            _packageSearch.UnregisterValueChangedCallback(OnSearchStringChanged);
            
            _list.selectedIndicesChanged -= OnUpdateRightPane;
            
            _btnImport.clicked -= OnImport;
            _btnRemove.clicked -= OnRemove;
            _btnUpdate.clicked -= OnUpdate;
        }

        #endregion

        #region Callbacks

        private Action _onLoadingPackages => () =>
        {
            _loading.style.display = DisplayStyle.Flex;
            _packages.style.display = DisplayStyle.None;
            
            MegaPintPackageManager.CachedPackages.UpdateLoadingLabel(
                _loading, 
                _currentLoadingLabelProgress, 
                30, 
                out _currentLoadingLabelProgress);
        };

        private Action<MegaPintPackageManager.CachedPackages> _onPackagesLoaded => packages =>
        {
            _loading.style.display = DisplayStyle.None;
            _packages.style.display = DisplayStyle.Flex;
            
            _currentLoadingLabelProgress = 0;
            
            _allPackages = packages;
            SetDisplayedPackages(_packageSearch.value);
        };

        private void OnSearchStringChanged(ChangeEvent<string> evt) => SetDisplayedPackages(_packageSearch.value);
        
        private void OnUpdateRightPane(IEnumerable<int> _ = null)
        {
            var index = _list.selectedIndex;

            if (index < 0)
            {
                _content.style.display = DisplayStyle.None;
                return;
            }
            
            _content.style.display = DisplayStyle.Flex;
            
            MegaPintPackagesData.MegaPintPackageData package = _displayedPackages[index];
            _packageName.text = package.PackageNiceName;
            _version.text = package.Version;
            _lastUpdate.text = package.LastUpdate;
            _unityVersion.text = package.UnityVersion;
            _megaPintVersion.text = package.MegaPintVersion;
            _infoText.text = package.InfoText;

            var isImported = _allPackages.IsImported(package.PackageKey);
            _btnImport.style.display = isImported ? DisplayStyle.None : DisplayStyle.Flex;
            _btnRemove.style.display = isImported ? DisplayStyle.Flex : DisplayStyle.None;
            _btnUpdate.style.display = isImported && _allPackages.NeedsUpdate(package.PackageKey)
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
        }

        #region Import

        private void OnImport()
        {
            MegaPintPackageManager.onSuccess += OnImportSuccess;
            MegaPintPackageManager.onFailure += OnFailure;
            MegaPintPackageManager.AddEmbedded(_displayedPackages[_list.selectedIndex].GitUrl);
        }

        private static void OnImportSuccess()
        {
            MegaPintPackageManager.onSuccess -= OnImportSuccess;
            MegaPintPackageManager.onFailure -= OnFailure;
        }

        #endregion

        #region Remove

        private void OnRemove()
        {
            MegaPintPackageManager.onSuccess += OnRemoveSuccess;
            MegaPintPackageManager.onFailure += OnFailure;
            MegaPintPackageManager.Remove(_displayedPackages[_list.selectedIndex].PackageName);
        }

        private static void OnRemoveSuccess()
        {
            MegaPintPackageManager.onSuccess -= OnRemoveSuccess;
            MegaPintPackageManager.onFailure -= OnFailure;
        }

        #endregion

        #region Update

        private void OnUpdate()
        {
            MegaPintPackageManager.onSuccess += OnUpdateSuccess;
            MegaPintPackageManager.onFailure += OnFailure;
            MegaPintPackageManager.AddEmbedded(_displayedPackages[_list.selectedIndex].GitUrl);
        }

        private static void OnUpdateSuccess()
        {
            MegaPintPackageManager.onSuccess -= OnUpdateSuccess;
            MegaPintPackageManager.onFailure -= OnFailure;
        }

        #endregion

        private static void OnFailure(string error) => Debug.LogError(error);
        
        #endregion
        
        #region Internal Methods

        private void UpdateItem(VisualElement element, int index)
        {
            var package = _displayedPackages[index];
            
            element.Q<Label>("PackageName").text = package.PackageNiceName;
            
            var version = element.Q<Label>("Version");
            version.text = _allPackages.CurrentVersion(package.PackageKey);

            version.style.display = _allPackages.IsImported(package.PackageKey) ? DisplayStyle.Flex : DisplayStyle.None;
            version.style.color = _allPackages.NeedsUpdate(package.PackageKey) ? _wrongVersionColor : _normalColor;
        }

        private void SetDisplayedPackages(string searchString)
        {
            _loading.style.display = DisplayStyle.None;
            _packages.style.display = DisplayStyle.Flex;
            
            _displayedPackages = searchString.Equals("") ? 
                _allPackages.ToDisplay() :
                _allPackages.ToDisplay().Where(package => package.PackageNiceName.ToLower().Contains(searchString.ToLower())).ToList();
            
            _displayedPackages.Sort();

            _list.itemsSource = _displayedPackages;
            _list.RefreshItems();
        }

        #endregion
    }
}
#endif