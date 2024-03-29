#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Editor.Scripts.PackageManager;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.PackageManager.Packages;
using Editor.Scripts.PackageManager.Utility;
using Editor.Scripts.Settings;
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

        private List<CachedPackage> _displayedPackages;

        private List <CachedVariation> _displayedPackageVariations;

        private CachedPackage _currentPackage;
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

        private static bool _DevMode => MegaPintSettings.instance.GetSetting("General").GetValue("devMode", false);
        
        protected override void CreateGUI()
        {
            base.CreateGUI();

            VisualElement root = rootVisualElement;

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

            #region Variations List

            _packageVariations.makeItem = () => _variationsListItem.Instantiate();

            _packageVariations.bindItem = UpdateVariationItem;

            _packageVariations.destroyItem = element => element.Clear();

            #endregion

            _loading.style.display = DisplayStyle.Flex;
            _packages.style.display = DisplayStyle.None;
            
            PackageCache.Refresh();

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
            MegaPintPackageManager.onRefreshingPackages += _onLoadingPackages;
            PackageCache.onCacheRefreshed += _onPackagesLoaded;
            
            _packageSearch.RegisterValueChangedCallback(OnSearchStringChanged);
            
            _list.selectedIndicesChanged += OnUpdateRightPane;

            ButtonSubscriptions(true);
        }

        private void ButtonSubscriptions(bool status)
        {
            _btnImport.style.opacity = status ? 1f : .5f;
            _btnRemove.style.opacity = status ? 1f : .5f;
            _btnUpdate.style.opacity = status ? 1f : .5f;
            
            if (status)
            {
                _btnImport.clicked += OnImport;
                _btnRemove.clicked += OnRemove;
                _btnUpdate.clicked += OnUpdate;
            }
            else
            {
                _btnImport.clicked -= OnImport;
                _btnRemove.clicked -= OnRemove;
                _btnUpdate.clicked -= OnUpdate;
            }
        }

        protected override void UnRegisterCallbacks()
        {
            MegaPintPackageManager.onRefreshingPackages -= _onLoadingPackages;
            PackageCache.onCacheRefreshed -= _onPackagesLoaded;
            
            _packageSearch.UnregisterValueChangedCallback(OnSearchStringChanged);
            
            _list.selectedIndicesChanged -= OnUpdateRightPane;
            
            ButtonSubscriptions(false);
        }

        #endregion

        #region Callbacks

        private Action _onLoadingPackages => () =>
        {
            _loading.style.display = DisplayStyle.Flex;
            _packages.style.display = DisplayStyle.None;

            PackageManagerUtility.UpdateLoadingLabel(
                _loading, 
                _currentLoadingLabelProgress, 
                30, 
                out _currentLoadingLabelProgress);
        };

        private Action _onPackagesLoaded => () =>
        {
            _loading.style.display = DisplayStyle.None;
            _packages.style.display = DisplayStyle.Flex;

            _currentLoadingLabelProgress = 0;

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
            
            CachedPackage package = _displayedPackages[_currentIndex];
            _packageName.text = package.DisplayName;
            _version.text = package.Version;
            _lastUpdate.text = package.LastUpdate;
            _unityVersion.text = package.UnityVersion;
            _megaPintVersion.text = package.ReqMpVersion;
            _infoText.text = package.Description;

            var hasVariation = package.Variations is {Count: > 0};
            var hasDependency = package.Dependencies is {Count: > 0};

            _separator.style.display = hasVariation || hasDependency ? DisplayStyle.Flex : DisplayStyle.None;

            if (hasVariation)
            {
                _packageVariationsParent.style.display = DisplayStyle.Flex;

                _displayedPackageVariations = package.Variations;
                _packageVariations.itemsSource = _displayedPackageVariations;
                _packageVariations.RefreshItems();
            }
            else 
                _packageVariationsParent.style.display = DisplayStyle.None;

            if (hasDependency)
            {
                _dependencies.Clear();

                foreach (Dependency dependency in package.Dependencies)
                {
                    TemplateContainer item = _dependencyItem.Instantiate();
                    item.Q <Label>("PackageName").text = dependency.name;

                    var imported = PackageCache.IsInstalled(dependency.key);

                    item.Q <Label>("Missing").style.display = imported ? DisplayStyle.None : DisplayStyle.Flex;
                    item.Q <Label>("Imported").style.display = imported ? DisplayStyle.Flex : DisplayStyle.None;
                    
                    _dependencies.Add(item);
                }
                
                _dependencies.style.display = DisplayStyle.Flex;
            }
            else
                _dependencies.style.display = DisplayStyle.None;

            var isImported = PackageCache.IsInstalled(package.Key);
            _btnImport.style.display = isImported ? DisplayStyle.None : DisplayStyle.Flex;
            _btnRemove.style.display = isImported ? DisplayStyle.Flex : DisplayStyle.None;
            _btnUpdate.style.display = isImported && PackageCache.NeedsUpdate(package.Key)
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
        }

        #region Import

        private void OnImport()
        {
            ButtonSubscriptions(false);
            
            MegaPintPackageManager.onSuccess += OnImportSuccess;
            MegaPintPackageManager.onFailure += OnFailure;
#pragma warning disable CS4014
            MegaPintPackageManager.AddEmbedded(_displayedPackages[_list.selectedIndex]);
#pragma warning restore CS4014
        }

        private void OnImportVariation(CachedVariation variation)
        {
            ButtonSubscriptions(false);
            
            MegaPintPackageManager.onSuccess += OnImportSuccess;
            MegaPintPackageManager.onFailure += OnFailure;
#pragma warning disable CS4014
            MegaPintPackageManager.AddEmbedded(variation);
#pragma warning restore CS4014
        }

        private void OnImportSuccess()
        {
            ButtonSubscriptions(true);
            ReselectItem(_currentIndex);
            _list.ClearSelection();

            MegaPintPackageManager.onSuccess -= OnImportSuccess;
            MegaPintPackageManager.onFailure -= OnFailure;
        }

        #endregion

        #region Remove

        private void OnRemove()
        {
            ButtonSubscriptions(false);
            
            CachedPackage package = _displayedPackages[_list.selectedIndex];

            if (package.CanBeRemoved(out List <Dependency> dependants))
            {
                MegaPintPackageManager.onSuccess += OnRemoveSuccess;
                MegaPintPackageManager.onFailure += OnFailure;
                MegaPintPackageManager.Remove(package.Name);
            }
            else
            {
                List <string> deps = dependants.Select(dependant => dependant.name).ToList();
                
                var str = string.Join(", ", deps);
                EditorUtility.DisplayDialog("Remove Failed", $"Cannot remove the package because [{str}] depend on it!", "Ok");
            }
        }
        
        private void OnRemoveVariation()
        {
            OnImport();
        }

        private void OnRemoveSuccess()
        {
            ButtonSubscriptions(true);
            ReselectItem(_currentIndex);
            _list.ClearSelection();
            
            MegaPintPackageManager.onSuccess -= OnRemoveSuccess;
            MegaPintPackageManager.onFailure -= OnFailure;
        }

        #endregion

        #region Update

        private void OnUpdate()
        {
            ButtonSubscriptions(false);
            
            MegaPintPackageManager.onSuccess += OnUpdateSuccess;
            MegaPintPackageManager.onFailure += OnFailure;
#pragma warning disable CS4014
            MegaPintPackageManager.AddEmbedded(_displayedPackages[_list.selectedIndex]);
#pragma warning restore CS4014
        }
        
        private void OnUpdateVariation(CachedVariation variation)
        {
            ButtonSubscriptions(false);
            
            MegaPintPackageManager.onSuccess += OnUpdateSuccess;
            MegaPintPackageManager.onFailure += OnFailure;
#pragma warning disable CS4014
            MegaPintPackageManager.AddEmbedded(variation);
#pragma warning restore CS4014
        }

        private void OnUpdateSuccess()
        {
            ButtonSubscriptions(true);
            ReselectItem(_currentIndex);
            _list.ClearSelection();

            MegaPintPackageManager.onSuccess -= OnUpdateSuccess;
            MegaPintPackageManager.onFailure -= OnFailure;
        }

        #endregion

        private void OnFailure(string error)
        {
            ButtonSubscriptions(true);
            Debug.LogError(error);
        }

        #endregion

        private async void ReselectItem(int index)
        {
            await Task.Delay(500);
            _list.selectedIndex = index;
        }
        
        #region Internal Methods

        private void UpdateItem(VisualElement element, int index)
        {
            CachedPackage package = _displayedPackages[index];
            _currentPackage = package;
            
            element.Q<Label>("PackageName").text = package.DisplayName;
            
            var version = element.Q<Label>("Version");

            version.text = _DevMode ? "Dev" : package.CurrentVersion;

            version.style.display = package.IsInstalled ? DisplayStyle.Flex : DisplayStyle.None;
            version.style.color = !package.IsNewestVersion ? _wrongVersionColor : _normalColor;
        }
        
        private void UpdateVariationItem(VisualElement element, int index)
        {
            CachedVariation variation = _displayedPackageVariations[index];
            _currentPackage = _displayedPackages[_list.selectedIndex];
            
            element.Q<Label>("PackageName").text = variation.name;

            var version = element.Q <Label>("Version");
            var btnImport = element.Q <Button>("BTN_Import");
            var btnRemove = element.Q<Button>("BTN_Remove");
            var btnUpdate = element.Q<Button>("BTN_Update");

            var dependencies = element.Q <Foldout>("Dependencies");
            
            if (variation.dependencies is {Count: > 0})
            {
                dependencies.Clear();

                foreach (Dependency dependency in variation.dependencies)
                {
                    TemplateContainer item = _dependencyItem.Instantiate();
                    item.Q <Label>("PackageName").text = dependency.name;

                    var imported = PackageCache.IsInstalled(dependency.key);

                    item.Q <Label>("Missing").style.display = imported ? DisplayStyle.None : DisplayStyle.Flex;
                    item.Q <Label>("Imported").style.display = imported ? DisplayStyle.Flex : DisplayStyle.None;
                    
                    dependencies.Add(item);
                }
                
                dependencies.style.display = DisplayStyle.Flex;
            }
            else
                dependencies.style.display = DisplayStyle.None;
            
            if (!_currentPackage.IsInstalled)
            {
                version.style.display = DisplayStyle.None;
                btnImport.style.display = DisplayStyle.None;
                btnRemove.style.display = DisplayStyle.None;
                btnUpdate.style.display = DisplayStyle.None;
            }
            else
            {
                version.text = _DevMode ? "Development" : _currentPackage.CurrentVersion;
                
                var isVariation = PackageCache.IsVariation(_currentPackage.Key, PackageManagerUtility.GetVariationHash(variation));
                var needsUpdate = PackageCache.NeedsVariationUpdate(_currentPackage.Key, variation.name);
            
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
                PackageCache.GetAllMpPackages() :
                PackageCache.GetAllMpPackages().Where(package => package.DisplayName.ToLower().Contains(searchString.ToLower())).ToList();
            
            _displayedPackages.Sort();

            _list.itemsSource = _displayedPackages;
            _list.RefreshItems();
        }

        #endregion
    }
}
#endif