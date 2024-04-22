#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.Scripts.PackageManager.Cache;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = Editor.Scripts.GUI.GUIUtility;

namespace Editor.Scripts.Windows.BaseWindowContent
{

internal class PackagesTab
{
    private readonly string _itemPath = Path.Combine(Constants.BasePackage.PathWindows, "Base Window", "Package Item");

    private readonly VisualTreeAsset _itemTemplate;

    private readonly Label _packageName;

    private readonly ListView _packagesList;

    private readonly ToolbarSearchField _searchField;
    private readonly VisualElement _tabContent;
    private readonly VisualElement _tabs;

    private bool _active;

    private VisualElement _currentVisualElement;

    private List <CachedPackage> _displayedPackages;
    private bool _ignoreNextUpdate;
    private List <VisualElement> _visualElements;

    public PackagesTab(VisualElement root)
    {
        _itemTemplate = Resources.Load <VisualTreeAsset>(_itemPath);

        _packagesList = root.Q <ListView>("PackagesList");
        _searchField = root.Q <ToolbarSearchField>("SearchField");

        var rightPane = root.Q <VisualElement>("RightPane");

        _packageName = rightPane.Q <Label>("PackageName");
        _tabContent = rightPane.Q <VisualElement>("TabContent");
        _tabs = rightPane.Q <VisualElement>("Tabs");

        RegisterCallbacks();

        Hide();
    }

    #region Public Methods

    public void Hide()
    {
        Clear();

        _packageName.style.display = DisplayStyle.None;
        _tabContent.style.display = DisplayStyle.None;
        _tabs.style.display = DisplayStyle.None;

        _packagesList.style.display = DisplayStyle.None;
        _active = false;
    }

    public void ResetVariables()
    {
        _displayedPackages = null;
        _currentVisualElement = null;
        _visualElements = null;
    }

    public void Show()
    {
        _searchField.value = "";
        SetDisplayedPackages();

        _packageName.style.display = DisplayStyle.None;
        _tabContent.style.display = DisplayStyle.Flex;
        _tabs.style.display = DisplayStyle.None;

        _packagesList.style.display = DisplayStyle.Flex;
        _active = true;
    }

    #endregion

    #region Private Methods

    private void Clear()
    {
        if (!_active)
            return;

        BaseWindow.onRightPaneClose?.Invoke();
        _tabContent.Clear();
    }

    private void OnSearchFieldChange(ChangeEvent <string> evt)
    {
        if (!_active)
            return;

        SetDisplayedPackages();
    }

    private void OnUpdateRightPane(IEnumerable <int> _)
    {
        if (_ignoreNextUpdate)
        {
            _ignoreNextUpdate = false;

            return;
        }

        Clear();

        if (_packagesList.selectedItem == null)
            return;

        if (_currentVisualElement != null)
            _currentVisualElement.style.borderLeftWidth = 0;

        var visualElement = _visualElements[_packagesList.selectedIndex].
            Q <Label>("PackageName");

        visualElement.style.borderLeftWidth = 2.5f;
        _currentVisualElement = visualElement;

        var currentPackage = (CachedPackage)_packagesList.selectedItem;

        _packageName.text = currentPackage.DisplayName;
        _packageName.style.display = DisplayStyle.Flex;

        DisplayContent.DisplayRightPane(
            new DisplayContent.DisplayContentReferences
            {
                package = currentPackage, tabContent = _tabContent, tabs = _tabs
            });

        _ignoreNextUpdate = true;
        _packagesList.ClearSelection();

        BaseWindow.onRightPaneInitialization?.Invoke();
    }

    private void RegisterCallbacks()
    {
        _packagesList.makeItem = () => GUIUtility.Instantiate(_itemTemplate);

        _packagesList.bindItem = (element, i) =>
        {
            _visualElements ??= new List <VisualElement>();
            _visualElements.Add(element);

            var packageNameLabel = element.Q <Label>("PackageName");
            packageNameLabel.text = _displayedPackages[i].DisplayName;
            packageNameLabel.style.borderLeftWidth = 0;
        };

        _packagesList.destroyItem = element =>
        {
            _visualElements ??= new List <VisualElement>();

            if (_visualElements.Contains(element))
                _visualElements.Remove(element);

            element.Clear();
        };

        _packagesList.selectedIndicesChanged += OnUpdateRightPane;

        _searchField.RegisterValueChangedCallback(OnSearchFieldChange);
    }

    private void SetDisplayedPackages()
    {
        List <CachedPackage> allPackages = PackageCache.GetAllMpPackages();

        var searchString = _searchField.value;

        _displayedPackages = searchString.Equals("")
            ? allPackages.Where(package => package.IsInstalled).ToList()
            : allPackages.
              Where(package => package.IsInstalled).
              Where(package => package.DisplayName.ToLower().Contains(searchString.ToLower())).
              ToList();

        _displayedPackages.Sort();

        _packagesList.ClearSelection();
        _packagesList.itemsSource = _displayedPackages;
        _packagesList.RefreshItems();
    }

    #endregion
}

}
#endif
