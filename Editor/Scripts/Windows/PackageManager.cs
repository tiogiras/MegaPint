#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.GUI;
using MegaPint.Editor.Scripts.GUI.Utility;
using MegaPint.Editor.Scripts.PackageManager;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.PackageManager.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Windows
{

/// <summary> Editor window to display and handle everything related to the internal package manager </summary>
internal class PackageManager : EditorWindowBase
{
    private static Action <PackageKey> s_showWithLink;

    private VisualTreeAsset _baseWindow;

    private Button _btnImport;
    private Button _btnRemove;
    private Button _btnUpdate;

    private bool _callbacksRegistered;
    private GroupBox _content;
    private int _currentIndex;

    private CachedPackage _currentPackage;

    private Foldout _dependencies;
    private VisualTreeAsset _dependencyItem;

    private List <CachedPackage> _displayedPackages;

    private List <CachedVariation> _displayedPackageVariations;
    private Label _infoText;

    private VisualElement _installedVersion;

    private VisualElement _lastUpdate;

    private ListView _list;

    private VisualTreeAsset _listItem;
    private VisualElement _megaPintVersion;
    private VisualElement _newestVersion;

    private Label _packageName;

    private ToolbarSearchField _packageSearch;
    private ListView _packageVariations;

    private VisualElement _packageVariationsParent;

    private VisualElement _rightPane;

    private VisualElement _root;

    private VisualTreeAsset _sampleItem;
    private ListView _samples;

    private VisualElement _samplesParent;
    private VisualElement _unityVersion;
    private VisualTreeAsset _variationsListItem;

    #region Public Methods

    /// <summary> Open the package manager via link </summary>
    /// <param name="key"> Package to be shown on open </param>
    public static void OpenPerLink(PackageKey key)
    {
        BaseWindow.OnOpenPackageManager();
        s_showWithLink?.Invoke(key);
    }

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "Package Manager";

        minSize = new Vector2(700, 350);

        if (!SaveValues.BasePackage.ApplyPSPackageManager)
            return this;

        this.CenterOnMainWin(850, 450);
        SaveValues.BasePackage.ApplyPSPackageManager = false;

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return Constants.BasePackage.UserInterface.PackageManager;
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
        _listItem = Resources.Load <VisualTreeAsset>(Constants.BasePackage.UserInterface.PackageManagerPackageItem);

        _variationsListItem =
            Resources.Load <VisualTreeAsset>(Constants.BasePackage.UserInterface.PackageManagerVariationItem);

        _dependencyItem =
            Resources.Load <VisualTreeAsset>(Constants.BasePackage.UserInterface.PackageManagerDependencyItem);

        _sampleItem = Resources.Load <VisualTreeAsset>(Constants.BasePackage.UserInterface.PackageManagerSampleItem);

        return _baseWindow != null &&
               _listItem != null &&
               _variationsListItem != null &&
               _dependencyItem != null &&
               _sampleItem != null;
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

        if (s_showWithLink == null)
        {
            _callbacksRegistered = false;

            return;
        }

        s_showWithLink -= OnShowLink;

        PackageCache.onCacheStartRefreshing -= StartCacheRefresh;

        _packageSearch.UnregisterValueChangedCallback(OnSearchStringChanged);

        _list.selectedIndicesChanged -= OnUpdateRightPane;

        ButtonSubscriptions(false);

        _callbacksRegistered = false;
    }

    #endregion

    #region Private Methods

    /// <summary> Update the coloring of the installed version based on if the installed is the newest version </summary>
    /// <param name="installedVersionElement"> Targeted visual element </param>
    /// <param name="package"> Targeted package </param>
    private static void UpdateVersionColor(VisualElement installedVersionElement, CachedPackage package)
    {
        UpdateVersionColor(installedVersionElement, !package.IsNewestVersion);
    }

    /// <summary> Update the coloring of the installed version based on if the installed is the newest version </summary>
    /// <param name="installedVersionElement"> Targeted visual element </param>
    /// <param name="needsUpdate"> Targeted package </param>
    private static void UpdateVersionColor(VisualElement installedVersionElement, bool needsUpdate)
    {
        if (!needsUpdate)
            installedVersionElement.RemoveFromClassList(StyleSheetClasses.Image.Tint.Orange);
        else
            installedVersionElement.AddToClassList(StyleSheetClasses.Image.Tint.Orange);
    }

    /// <summary> Handle button subscriptions </summary>
    /// <param name="status"> Status of the buttons </param>
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
        root.Clear();

        VisualElement content = GUIUtility.Instantiate(_baseWindow, root);

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

        _samplesParent = _content.Q <VisualElement>("SamplesParent");
        _samples = _content.Q <ListView>("Samples");

        _dependencies = _content.Q <Foldout>("Dependencies");

        _btnImport = _rightPane.Q <Button>("BTN_Import");
        _btnRemove = _rightPane.Q <Button>("BTN_Remove");
        _btnUpdate = _rightPane.Q <Button>("BTN_Update");

        _packageSearch = content.Q <ToolbarSearchField>("PackageSearch");

        RegisterCallbacks();

        _list.makeItem = () => GUIUtility.Instantiate(_listItem);

        _list.bindItem = MakeItem;

        _list.destroyItem = element => element.Clear();

        _packageVariations.makeItem = () => GUIUtility.Instantiate(_variationsListItem);

        _packageVariations.bindItem = UpdateVariationItem;

        _packageVariations.destroyItem = element => element.Clear();

        SetDisplayedPackages(_packageSearch.value);

        OnUpdateRightPane();
    }

    /// <summary> Make list view item </summary>
    /// <param name="element"> List view item </param>
    /// <param name="index"> Index of the list view item </param>
    private void MakeItem(VisualElement element, int index)
    {
        CachedPackage package = _displayedPackages[index];
        _currentPackage = package;

        var packageName = element.Q <Label>("PackageName");
        packageName.text = package.DisplayName;

        var version = element.Q <Label>("Version");
        version.text = SaveValues.BasePackage.DevMode ? "Dev" : package.CurrentVersion;
        version.style.display = package.IsInstalled ? DisplayStyle.Flex : DisplayStyle.None;

        _list.selectedIndicesChanged += _ =>
        {
            var isSelected = _list.selectedIndex == index;

            if (isSelected)
            {
                packageName.AddToClassList(StyleSheetClasses.Text.Color.ButtonActive);
                version.AddToClassList(StyleSheetClasses.Text.Color.ButtonActive);
            }
            else
            {
                packageName.RemoveFromClassList(StyleSheetClasses.Text.Color.ButtonActive);
                version.RemoveFromClassList(StyleSheetClasses.Text.Color.ButtonActive);
            }
        };

        UpdateVersionColor(_installedVersion, package);
    }

    /// <summary> Called on failure </summary>
    private void OnFailure(string error)
    {
        ButtonSubscriptions(true);
        Debug.LogError(error);
    }

    /// <summary> Import the displayed package </summary>
    private void OnImport()
    {
        ButtonSubscriptions(false);

        MegaPintPackageManager.onSuccess += OnImportSuccess;
        MegaPintPackageManager.onFailure += OnFailure;
#pragma warning disable CS4014
        MegaPintPackageManager.AddEmbedded(_displayedPackages[_list.selectedIndex]);
#pragma warning restore CS4014
    }

    /// <summary> Called on successful import </summary>
    private void OnImportSuccess()
    {
        ButtonSubscriptions(true);
        ReselectItem(_currentIndex);
        _list.ClearSelection();

        MegaPintPackageManager.onSuccess -= OnImportSuccess;
        MegaPintPackageManager.onFailure -= OnFailure;
    }

    /// <summary> Import the variation of the displayed package </summary>
    /// <param name="variation"> Targeted variation </param>
    private void OnImportVariation(CachedVariation variation)
    {
        ButtonSubscriptions(false);

        MegaPintPackageManager.onSuccess += OnImportSuccess;
        MegaPintPackageManager.onFailure += OnFailure;
#pragma warning disable CS4014
        MegaPintPackageManager.AddEmbedded(variation);
#pragma warning restore CS4014
    }

    /// <summary> Remove the displayed package </summary>
    private void OnRemove()
    {
        CachedPackage package = _displayedPackages[_list.selectedIndex];

        if (package.CanBeRemoved(out List <PackageKey> dependants))
        {
            ButtonSubscriptions(false);

            MegaPintPackageManager.onSuccess += OnRemoveSuccess;
            MegaPintPackageManager.onFailure += OnFailure;

#pragma warning disable CS4014
            MegaPintPackageManager.Remove(package.Name);
#pragma warning restore CS4014
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

    /// <summary> Called on successful remove </summary>
    private void OnRemoveSuccess()
    {
        ButtonSubscriptions(true);
        ReselectItem(_currentIndex);
        _list.ClearSelection();

        MegaPintPackageManager.onSuccess -= OnRemoveSuccess;
        MegaPintPackageManager.onFailure -= OnFailure;
    }

    /// <summary> Remove the variation of the displayed package </summary>
    private void OnRemoveVariation()
    {
        OnImport();
    }

    /// <summary> SearchField callback </summary>
    /// <param name="evt"> Callback event </param>
    private void OnSearchStringChanged(ChangeEvent <string> evt)
    {
        SetDisplayedPackages(_packageSearch.value);
    }

    /// <summary> Show package via link </summary>
    /// <param name="key"> Selected package </param>
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

    /// <summary> Update the displayed package </summary>
    private void OnUpdate()
    {
        ButtonSubscriptions(false);

        MegaPintPackageManager.onSuccess += OnUpdateSuccess;
        MegaPintPackageManager.onFailure += OnFailure;
#pragma warning disable CS4014
        MegaPintPackageManager.AddEmbedded(_displayedPackages[_list.selectedIndex]);
#pragma warning restore CS4014
    }

    /// <summary> ListView callback </summary>
    /// <param name="_"> Callback event </param>
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

        _installedVersion.style.display = package.IsInstalled || Utility.IsProductionProject()
            ? DisplayStyle.Flex
            : DisplayStyle.None;

        _newestVersion.tooltip = $"Newest Version: {package.Version}";
        _lastUpdate.tooltip = $"Last Update: {package.LastUpdate}";
        _installedVersion.tooltip = $"Installed Version: {package.CurrentVersion}";
        _unityVersion.tooltip = $"Unity Version: {package.UnityVersion}";
        _megaPintVersion.tooltip = $"MegaPint Version: {package.ReqMpVersion}";

        _infoText.text = package.Description;

        var hasImages = package.Images is {Count: > 0};
        var galleryButton = _content.Q <VisualElement>("Gallery");

        galleryButton.style.display = hasImages ? DisplayStyle.Flex : DisplayStyle.None;

        galleryButton.AddClickInteraction(
            () =>
            {
                var gallery = (Gallery)ContextMenu.TryOpen <Gallery>(true);
                gallery.Initialize(package);
            });

        var hasVariation = package.Variations is {Count: > 0};
        var hasDependency = package.Dependencies is {Count: > 0};

        if (hasVariation && package.IsInstalled)
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

        var hasSamples = package.HasSamples;

        _samplesParent.style.display = hasSamples ? DisplayStyle.Flex : DisplayStyle.None;

        if (hasSamples)
        {
            _samples.makeItem = () => GUIUtility.Instantiate(_sampleItem);

            _samples.bindItem = (element, i) =>
            {
                SampleData sample = package.Samples[i];

                element.Q <Label>("SampleName").text = sample.displayName;

                element.Q <Button>("BTN_Import").clickable = new Clickable(
                    () =>
                    {
                        var samplePath = Utility.GetPackageSamplePath(package.Key, sample.path);

                        var directory = Path.Combine(Application.dataPath, "MegaPint Samples");

                        var assetFolderPath = Path.Combine(
                            directory,
                            $"{package.DisplayName}_{sample.displayName}.unitypackage");

                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);

                        File.Copy(samplePath, assetFolderPath, true);

                        AssetDatabase.ImportPackage(assetFolderPath, true);
                    });
            };

            _samples.Clear();
            _samples.itemsSource = package.Samples;
            _samples.RefreshItems();
        }
    }

    /// <summary> Called on successful update </summary>
    private void OnUpdateSuccess()
    {
        ButtonSubscriptions(true);
        ReselectItem(_currentIndex);
        _list.ClearSelection();

        MegaPintPackageManager.onSuccess -= OnUpdateSuccess;
        MegaPintPackageManager.onFailure -= OnFailure;
    }

    /// <summary> Update the variation of the displayed package </summary>
    /// <param name="variation"> Targeted variation </param>
    private void OnUpdateVariation(CachedVariation variation)
    {
        ButtonSubscriptions(false);

        MegaPintPackageManager.onSuccess += OnUpdateSuccess;
        MegaPintPackageManager.onFailure += OnFailure;
#pragma warning disable CS4014
        MegaPintPackageManager.AddEmbedded(variation);
#pragma warning restore CS4014
    }

    /// <summary> Reselect the list view item after a delay </summary>
    /// <param name="index"> Index of the targeted item </param>
    private async void ReselectItem(int index)
    {
        await Task.Delay(500);
        _list.selectedIndex = index;
    }

    /// <summary> Set the displayed packages via the searchString </summary>
    /// <param name="searchString"> String to filter the packages with </param>
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

    /// <summary> Prepare for a <see cref="PackageCache" /> refresh </summary>
    private void StartCacheRefresh()
    {
        GUIUtility.DisplaySplashScreen(_root, () => {CreateGUIContent(_root);});
    }

    /// <summary> Update a variation item </summary>
    /// <param name="element"> List view item </param>
    /// <param name="index"> Index of the list view item </param>
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
            version.text = SaveValues.BasePackage.DevMode ? "Development" : _currentPackage.CurrentVersion;

            var isVariation = PackageCache.IsVariation(
                _currentPackage.Key,
                PackageManagerUtility.GetVariationHash(variation));

            var needsUpdate = PackageCache.NeedsVariationUpdate(_currentPackage.Key, variation.name);

            version.style.display = isVariation ? DisplayStyle.Flex : DisplayStyle.None;

            UpdateVersionColor(version, needsUpdate);

            btnImport.style.display = isVariation ? DisplayStyle.None : DisplayStyle.Flex;
            btnRemove.style.display = isVariation ? DisplayStyle.Flex : DisplayStyle.None;
            btnUpdate.style.display = isVariation && needsUpdate ? DisplayStyle.Flex : DisplayStyle.None;

            btnImport.clickable = new Clickable(() => {OnImportVariation(variation);});

            btnRemove.clickable = new Clickable(OnRemoveVariation);

            btnUpdate.clickable = new Clickable(() => {OnUpdateVariation(variation);});
        }
    }

    #endregion
}

}
#endif
