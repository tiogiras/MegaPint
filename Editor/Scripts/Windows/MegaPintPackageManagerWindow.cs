#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor.Scripts.GUI;
using Editor.Scripts.PackageManager;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.PackageManager.Packages;
using Editor.Scripts.PackageManager.Utility;
using Editor.Scripts.Settings;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = Editor.Scripts.GUI.GUIUtility;

namespace Editor.Scripts.Windows
{

internal class MegaPintPackageManagerWindow : MegaPintEditorWindowBase
{
    private static Action <PackageKey> s_showWithLink;

    private static bool _DevMode => MegaPintSettings.instance.GetSetting("General").GetValue("devMode", false);

    #region Public Methods

    public static void OpenPerLink(PackageKey key)
    {
        BaseWindow.OnOpenPackageManager();
        s_showWithLink?.Invoke(key);
    }

    public override MegaPintEditorWindowBase ShowWindow()
    {
        titleContent.text = "Package Manager";

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return Path;
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
        _listItem = Resources.Load <VisualTreeAsset>(ListItemTemplate);
        _variationsListItem = Resources.Load <VisualTreeAsset>(VariationsListItemTemplate);
        _dependencyItem = Resources.Load <VisualTreeAsset>(DependencyItemTemplate);
        _imageItem = Resources.Load <VisualTreeAsset>(ImageItemTemplate);

        return _baseWindow != null &&
               _listItem != null &&
               _variationsListItem != null &&
               _dependencyItem != null &&
               _imageItem != null;
    }

    protected override void RegisterCallbacks()
    {
        s_showWithLink += OnShowLink;

        _packageSearch.RegisterValueChangedCallback(OnSearchStringChanged);

        _list.selectedIndicesChanged += OnUpdateRightPane;

        ButtonSubscriptions(true);
    }

    protected override void UnRegisterCallbacks()
    {
        if (!_callbacksRegistered)
            return;

        s_showWithLink -= OnShowLink;

        PackageCache.onCacheStartRefreshing -= StartCacheRefresh;

        _packageSearch.UnregisterValueChangedCallback(OnSearchStringChanged);

        _list.selectedIndicesChanged -= OnUpdateRightPane;

        ButtonSubscriptions(false);

        _callbacksRegistered = false;
    }

    #endregion

    #region Private Methods

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

        _callbacksRegistered = true;
    }

    private void CreateGUIContent(VisualElement root)
    {
        VisualElement content = GUIUtility.Instantiate(_baseWindow, root);

        #region References

        _list = content.Q <ListView>("MainList");

        _rightPane = content.Q <VisualElement>("RightPane");
        _packageName = _rightPane.Q <Label>("PackageName");
        _content = _rightPane.Q <GroupBox>("Content");

        _infoText = _content.Q <Label>("InfoText");

        _installedVersion = _content.Q <VisualElement>("InstalledVersion");
        _newestVersion = _content.Q <VisualElement>("NewestVersion");

        _lastUpdate = _content.Q <VisualElement>("LastUpdate");
        _unityVersion = _content.Q <VisualElement>("UnityVersion");
        _megaPintVersion = _content.Q <VisualElement>("MegaPintVersion");

        _packageVariationsParent = _content.Q <VisualElement>("PackageVariationsParent");
        _packageVariations = _content.Q <ListView>("PackageVariations");

        _dependencies = _content.Q <Foldout>("Dependencies");

        //_separator = _content.Q <VisualElement>("DependencySeparator");

        _btnImport = _rightPane.Q <Button>("BTN_Import");
        _btnRemove = _rightPane.Q <Button>("BTN_Remove");
        _btnUpdate = _rightPane.Q <Button>("BTN_Update");

        _packageSearch = content.Q <ToolbarSearchField>("PackageSearch");

        #endregion

        RegisterCallbacks();

        #region List

        _list.makeItem = () => GUIUtility.Instantiate(_listItem);

        _list.bindItem = UpdateItem;

        _list.destroyItem = element => element.Clear();

        #endregion

        #region Variations List

        _packageVariations.makeItem = () => GUIUtility.Instantiate(_variationsListItem);

        _packageVariations.bindItem = UpdateVariationItem;

        _packageVariations.destroyItem = element => element.Clear();

        #endregion

        SetDisplayedPackages(_packageSearch.value);

        OnUpdateRightPane();
    }

    private void OnShowLink(PackageKey key)
    {
        _packageSearch.value = "";
        SetDisplayedPackages(_packageSearch.value);

        var targetIndex = -1;

        for (var i = 0; i < _displayedPackages.Count; i++)
        {
            CachedPackage package = _displayedPackages[i];

            if (package.Key != key)
                continue;

            targetIndex = i;

            break;
        }

        if (targetIndex == -1)
            return;

        _list.selectedIndex = targetIndex;
    }

    private async void ReselectItem(int index)
    {
        await Task.Delay(500);
        _list.selectedIndex = index;
    }

    private void SetDisplayedPackages(string searchString)
    {
        _list.style.display = DisplayStyle.Flex;

        _displayedPackages = searchString.Equals("")
            ? PackageCache.GetAllMpPackages()
            : PackageCache.GetAllMpPackages().
                           Where(package => package.DisplayName.ToLower().Contains(searchString.ToLower())).
                           ToList();

        _displayedPackages.Sort();

        _list.itemsSource = _displayedPackages;
        _list.RefreshItems();
    }

    private void StartCacheRefresh()
    {
        GUIUtility.DisplaySplashScreen(_root, () => {CreateGUIContent(_root);});
    }

    private void UpdateItem(VisualElement element, int index)
    {
        CachedPackage package = _displayedPackages[index];
        _currentPackage = package;

        element.Q <Label>("PackageName").text = package.DisplayName;

        var version = element.Q <Label>("Version");

        version.text = _DevMode ? "Dev" : package.CurrentVersion;

        version.style.display = package.IsInstalled ? DisplayStyle.Flex : DisplayStyle.None;
        version.style.color = !package.IsNewestVersion ? _wrongVersionColor : _normalColor;
    }

    private void UpdateVariationItem(VisualElement element, int index)
    {
        CachedVariation variation = _displayedPackageVariations[index];
        _currentPackage = _displayedPackages[_list.selectedIndex];

        element.Q <Label>("PackageName").text = variation.name;

        var version = element.Q <Label>("Version");
        var btnImport = element.Q <Button>("BTN_Import");
        var btnRemove = element.Q <Button>("BTN_Remove");
        var btnUpdate = element.Q <Button>("BTN_Update");

        var dependencies = element.Q <Foldout>("Dependencies");

        if (variation.dependencies is {Count: > 0})
        {
            dependencies.Clear();

            foreach (Dependency dependency in variation.dependencies)
            {
                VisualElement item = GUIUtility.Instantiate(_dependencyItem);
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

            var isVariation = PackageCache.IsVariation(
                _currentPackage.Key,
                PackageManagerUtility.GetVariationHash(variation));

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

    #endregion

    #region Const

    private const string Path = "MegaPint/User Interface/Windows/Package Manager";

    private const string ListItemTemplate = Path + "/Package Item";
    private const string VariationsListItemTemplate = Path + "/Variation Item";
    private const string DependencyItemTemplate = Path + "/Dependency Item";
    private const string ImageItemTemplate = Path + "/Image Item";

    private readonly Color _normalColor = RootElement.Colors.Text;
    private readonly Color _wrongVersionColor = RootElement.Colors.TextRed;

    #endregion

    #region Visual References

    private VisualElement _root;

    private VisualElement _rightPane;
    private GroupBox _content;

    private Label _packageName;
    private Label _infoText;

    private VisualElement _installedVersion;
    private VisualElement _newestVersion;

    private VisualElement _lastUpdate;
    private VisualElement _unityVersion;
    private VisualElement _megaPintVersion;

    private VisualElement _packageVariationsParent;
    private ListView _packageVariations;

    private Foldout _dependencies;

    //private VisualElement _separator;

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
    private VisualTreeAsset _imageItem;

    private List <CachedPackage> _displayedPackages;

    private List <CachedVariation> _displayedPackageVariations;

    private CachedPackage _currentPackage;
    private int _currentIndex;

    private bool _callbacksRegistered;

    #endregion

    #region Callbacks

    private void OnSearchStringChanged(ChangeEvent <string> evt)
    {
        SetDisplayedPackages(_packageSearch.value);
    }

    private void OnUpdateRightPane(IEnumerable <int> _ = null)
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

        _newestVersion.tooltip = $"Newest Version: {package.Version}";
        _installedVersion.tooltip = $"Installed Version: {package.CurrentVersion}";
        _lastUpdate.tooltip = $"Last Update: {package.LastUpdate}";
        _unityVersion.tooltip = $"Unity Version: {package.UnityVersion}";
        _megaPintVersion.tooltip = $"MegaPint Version: {package.ReqMpVersion}";

        _infoText.text = package.Description;

        var hasImages = package.Images is {Count: > 0};
        var galleryButton = _content.Q <VisualElement>("Gallery");

        galleryButton.style.display = hasImages ? DisplayStyle.Flex : DisplayStyle.None;

        GUIUtility.AddClickInteraction(
            galleryButton,
            () =>
            {
                var gallery = (Gallery)ContextMenu.TryOpen <Gallery>(true);
                gallery.Initialize(package);
            });

        var hasVariation = package.Variations is {Count: > 0};
        var hasDependency = package.Dependencies is {Count: > 0};

        //_separator.style.display = hasVariation || hasDependency ? DisplayStyle.Flex : DisplayStyle.None;

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
                VisualElement item = GUIUtility.Instantiate(_dependencyItem);
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
        CachedPackage package = _displayedPackages[_list.selectedIndex];

        if (package.CanBeRemoved(out List <PackageKey> dependants))
        {
            ButtonSubscriptions(false);
            
            MegaPintPackageManager.onSuccess += OnRemoveSuccess;
            MegaPintPackageManager.onFailure += OnFailure;
            MegaPintPackageManager.Remove(package.Name);
        }
        else
        {
            List <string> deps = dependants.Select(dependant => dependant.ToString()).ToList();

            var str = string.Join(", ", deps);

            EditorUtility.DisplayDialog(
                "Remove Failed",
                $"Cannot remove the package because [{str}] depends on it!",
                "Ok");
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
}

}
#endif
