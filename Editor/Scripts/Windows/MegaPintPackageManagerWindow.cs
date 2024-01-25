#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Editor.Scripts.PackageManager;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Windows
{
    internal class MegaPintPackageManagerWindow : MegaPintEditorWindowBase
    {
        #region Const

        private const string ListItemTemplate = "User Interface/Import/MegaPintPackageItem";
        private const string VariationsListItemTemplate = "User Interface/Import/MegaPintVariationItem";
        private const string DependencyItemTemplate = "User Interface/Import/MegaPintDependencyItem";
        
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

        private GroupBox _packageVariationsParent;
        private ListView _packageVariations;

        private Foldout _dependencies;
        private VisualElement _separator;

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
        private VisualTreeAsset _variationsListItem;
        private VisualTreeAsset _dependencyItem;

        private MegaPintPackageManager.CachedPackages _allPackages;
        private List<MegaPintPackagesData.MegaPintPackageData> _displayedPackages;

        private List <MegaPintPackagesData.MegaPintPackageData.PackageVariation> _displayedPackageVariations;

        private MegaPintPackagesData.MegaPintPackageData _currentPackage;
        private int _currentIndex;
        
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
            _infoText = _content.Q <Label>("InfoText");

            _packageVariationsParent = _content.Q <GroupBox>("PackageVariationsParent");
            _packageVariations = _content.Q <ListView>("PackageVariations");

            _dependencies = _content.Q <Foldout>("Dependencies");
            _separator = _content.Q <VisualElement>("Separator");

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

            #region SubPackages List

            _packageVariations.makeItem = () => _variationsListItem.Instantiate();

            _packageVariations.bindItem = UpdateVariationItem;

            _packageVariations.destroyItem = element => element.Clear();

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
            _variationsListItem = Resources.Load <VisualTreeAsset>(VariationsListItemTemplate);
            _dependencyItem = Resources.Load <VisualTreeAsset>(DependencyItemTemplate);
            
            return _baseWindow != null && _listItem != null && _variationsListItem != null && _dependencyItem != null;
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
            _currentIndex = _list.selectedIndex;

            if (_currentIndex < 0)
            {
                _content.style.display = DisplayStyle.None;
                return;
            }
            
            _content.style.display = DisplayStyle.Flex;
            
            MegaPintPackagesData.MegaPintPackageData package = _displayedPackages[_currentIndex];
            _packageName.text = package.packageNiceName;
            _version.text = package.version;
            _lastUpdate.text = package.lastUpdate;
            _unityVersion.text = package.unityVersion;
            _megaPintVersion.text = package.megaPintVersion;
            _infoText.text = package.infoText;

            var hasVariation = package.variations is {Count: > 0};
            var hasDependency = package.dependencies is {Count: > 0};

            _separator.style.display = hasVariation || hasDependency ? DisplayStyle.Flex : DisplayStyle.None;
            
            if (hasVariation)
            {
                _packageVariationsParent.style.display = DisplayStyle.Flex;

                _displayedPackageVariations = package.variations;
                _packageVariations.itemsSource = _displayedPackageVariations;
                _packageVariations.RefreshItems();
            }
            else 
                _packageVariationsParent.style.display = DisplayStyle.None;

            if (hasDependency)
            {
                _dependencies.Clear();

                foreach (MegaPintPackagesData.MegaPintPackageData.Dependency dependency in package.dependencies)
                {
                    TemplateContainer item = _dependencyItem.Instantiate();
                    item.Q <Label>("PackageName").text = dependency.niceName;

                    var imported = _allPackages.IsImported(dependency.packageKey);

                    item.Q <Label>("Missing").style.display = imported ? DisplayStyle.None : DisplayStyle.Flex;
                    item.Q <Label>("Imported").style.display = imported ? DisplayStyle.Flex : DisplayStyle.None;
                    
                    _dependencies.Add(item);
                }
                
                _dependencies.style.display = DisplayStyle.Flex;
            }
            else
                _dependencies.style.display = DisplayStyle.None;

            var isImported = _allPackages.IsImported(package.packageKey);
            _btnImport.style.display = isImported ? DisplayStyle.None : DisplayStyle.Flex;
            _btnRemove.style.display = isImported ? DisplayStyle.Flex : DisplayStyle.None;
            _btnUpdate.style.display = isImported && _allPackages.NeedsUpdate(package.packageKey)
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
        }

        #region Import

        private void OnImport()
        {
            MegaPintPackageManager.onSuccess += OnImportSuccess;
            MegaPintPackageManager.onFailure += OnFailure;
            MegaPintPackageManager.AddEmbedded(_displayedPackages[_list.selectedIndex]);
        }

        private void OnImportVariation(MegaPintPackagesData.MegaPintPackageData.PackageVariation package)
        {
            MegaPintPackageManager.onSuccess += OnImportSuccess;
            MegaPintPackageManager.onFailure += OnFailure;
            MegaPintPackageManager.AddEmbedded(package);
        }

        private void OnImportSuccess()
        {
            ReselectItem(_currentIndex);
            _list.ClearSelection();
            
            // TODO only works if variation has no dependency 
            
            MegaPintPackageManager.onSuccess -= OnImportSuccess;
            MegaPintPackageManager.onFailure -= OnFailure;
        }

        #endregion

        #region Remove

        private void OnRemove()
        {
            MegaPintPackagesData.MegaPintPackageData package = _displayedPackages[_list.selectedIndex];

            if (_allPackages.CanBeRemoved(package.packageKey, out List <string> dependants))
            {
                MegaPintPackageManager.onSuccess += OnRemoveSuccess;
                MegaPintPackageManager.onFailure += OnFailure;
                MegaPintPackageManager.Remove(package.packageName);
            }
            else
            {
                var str = string.Join(", ", dependants);
                EditorUtility.DisplayDialog("Remove Failed", $"Cannot remove the package because [{str}] depend on it!", "Ok");
            }
        }
        
        private void OnRemoveVariation()
        {
            OnImport();
        }

        private void OnRemoveSuccess()
        {
            ReselectItem(_currentIndex);
            _list.ClearSelection();
            
            MegaPintPackageManager.onSuccess -= OnRemoveSuccess;
            MegaPintPackageManager.onFailure -= OnFailure;
        }

        #endregion

        #region Update

        private void OnUpdate()
        {
            MegaPintPackageManager.onSuccess += OnUpdateSuccess;
            MegaPintPackageManager.onFailure += OnFailure;
            MegaPintPackageManager.AddEmbedded(_displayedPackages[_list.selectedIndex]);
        }
        
        private void OnUpdateVariation(MegaPintPackagesData.MegaPintPackageData.PackageVariation package)
        {
            MegaPintPackageManager.onSuccess += OnUpdateSuccess;
            MegaPintPackageManager.onFailure += OnFailure;
            MegaPintPackageManager.AddEmbedded(package);
        }

        private void OnUpdateSuccess()
        {
            ReselectItem(_currentIndex);
            _list.ClearSelection();

            MegaPintPackageManager.onSuccess -= OnUpdateSuccess;
            MegaPintPackageManager.onFailure -= OnFailure;
        }

        #endregion

        private static void OnFailure(string error) => Debug.LogError(error);
        
        #endregion

        private async void ReselectItem(int index)
        {
            await Task.Delay(500);
            _list.selectedIndex = index;
        }
        
        #region Internal Methods

        private void UpdateItem(VisualElement element, int index)
        {
            MegaPintPackagesData.MegaPintPackageData package = _displayedPackages[index];
            _currentPackage = package;
            
            element.Q<Label>("PackageName").text = package.packageNiceName;
            
            var version = element.Q<Label>("Version");
            version.text = _allPackages.CurrentVersion(package.packageKey);

            version.style.display = _allPackages.IsImported(package.packageKey) ? DisplayStyle.Flex : DisplayStyle.None;
            version.style.color = _allPackages.NeedsUpdate(package.packageKey) ? _wrongVersionColor : _normalColor;
        }
        
        private void UpdateVariationItem(VisualElement element, int index)
        {
            MegaPintPackagesData.MegaPintPackageData.PackageVariation variation = _displayedPackageVariations[index];
            _currentPackage = _displayedPackages[_list.selectedIndex];
            
            element.Q<Label>("PackageName").text = variation.niceName;

            var version = element.Q <Label>("Version");
            var btnImport = element.Q <Button>("BTN_Import");
            var btnRemove = element.Q<Button>("BTN_Remove");
            var btnUpdate = element.Q<Button>("BTN_Update");

            var dependencies = element.Q <Foldout>("Dependencies");
            
            if (variation.dependencies is {Count: > 0})
            {
                dependencies.Clear();

                foreach (MegaPintPackagesData.MegaPintPackageData.Dependency dependency in variation.dependencies)
                {
                    TemplateContainer item = _dependencyItem.Instantiate();
                    item.Q <Label>("PackageName").text = dependency.niceName;

                    var imported = _allPackages.IsImported(dependency.packageKey);

                    item.Q <Label>("Missing").style.display = imported ? DisplayStyle.None : DisplayStyle.Flex;
                    item.Q <Label>("Imported").style.display = imported ? DisplayStyle.Flex : DisplayStyle.None;
                    
                    dependencies.Add(item);
                }
                
                dependencies.style.display = DisplayStyle.Flex;
            }
            else
                dependencies.style.display = DisplayStyle.None;
            
            if (!_allPackages.IsImported(_currentPackage.packageKey))
            {
                version.style.display = DisplayStyle.None;
                btnImport.style.display = DisplayStyle.None;
                btnRemove.style.display = DisplayStyle.None;
                btnUpdate.style.display = DisplayStyle.None;
            }
            else
            {
                version.text = _allPackages.CurrentVersion(_currentPackage.packageKey);
                
                var hash = $"v{variation.version}{variation.variationTag}";

                var isVariation = _allPackages.IsVariation(_currentPackage.packageKey, hash);
                var needsUpdate = _allPackages.NeedsVariationUpdate(_currentPackage.packageKey, variation.niceName);
            
                version.style.display = isVariation ? DisplayStyle.Flex : DisplayStyle.None;
                version.style.color = needsUpdate ? _wrongVersionColor : _normalColor;
                
                btnImport.style.display = isVariation ? DisplayStyle.None : DisplayStyle.Flex;
                btnRemove.style.display = isVariation ? DisplayStyle.Flex : DisplayStyle.None;
                btnUpdate.style.display = isVariation && needsUpdate ? DisplayStyle.Flex : DisplayStyle.None;

                btnImport.clickable = new Clickable(() => {OnImportVariation(variation);});
                
                btnRemove.clickable = new Clickable(OnRemoveVariation);
                
                btnUpdate.clickable = new Clickable(() => {OnUpdateVariation(variation);});
            }
        }

        private void SetDisplayedPackages(string searchString)
        {
            _loading.style.display = DisplayStyle.None;
            _packages.style.display = DisplayStyle.Flex;
            
            _displayedPackages = searchString.Equals("") ? 
                _allPackages.ToDisplay() :
                _allPackages.ToDisplay().Where(package => package.packageNiceName.ToLower().Contains(searchString.ToLower())).ToList();
            
            _displayedPackages.Sort();

            _list.itemsSource = _displayedPackages;
            _list.RefreshItems();
        }

        #endregion
    }
}
#endif