#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.Scripts.Windows.BaseWindowContent.InfoTabContent;
using MegaPint.Editor.Scripts;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.GUIUtility;

namespace Editor.Scripts.Windows.BaseWindowContent
{

internal class InfosTab
{
    private readonly string _itemPath = Path.Combine(Constants.BasePackage.PathWindows, "Base Window", "Info item");

    private readonly VisualTreeAsset _itemTemplate;

    private readonly ToolbarSearchField _searchField;

    private readonly VisualElement _content;

    private readonly ListView _list;

    private bool _active;
    private List <InfosTabData.Info> _allInfos;

    private VisualElement _currentVisualElement;
    private List <InfosTabData.Info> _displayedInfos;
    private bool _inSearch;

    private List <InfosTabData.Info> _openInfos;
    private List <VisualElement> _visualElements;

    public InfosTab(VisualElement root)
    {
        _itemTemplate = Resources.Load <VisualTreeAsset>(_itemPath);

        _list = root.Q <ListView>("InfosList");
        _searchField = root.Q <ToolbarSearchField>("SearchField");

        var rightPane = root.Q <VisualElement>("RightPane");

        _content = rightPane.Q <VisualElement>("InfosContent");

        RegisterCallbacks();

        Hide();
    }

    #region Public Methods

    public void Hide()
    {
        Clear();

        _list.style.display = DisplayStyle.None;
        _active = false;
    }

    public void ResetVariables()
    {
        _currentVisualElement = null;
        _visualElements = null;
    }

    public void Show()
    {
        _searchField.value = "";
        SetDisplayed("");

        _displayedInfos = InfosTabData.Infos;
        _openInfos = new List <InfosTabData.Info>();
        _visualElements = new List <VisualElement>();

        _inSearch = false;

        _list.itemsSource = _displayedInfos;
        _list.RefreshItems();

        _list.style.display = DisplayStyle.Flex;
        _active = true;

        if (_allInfos != null)
            return;

        _allInfos = new List <InfosTabData.Info>();

        foreach (InfosTabData.Info info in InfosTabData.Infos)
            _allInfos.AddRange(GetAll(info));
    }

    #endregion

    #region Private Methods

    private static IEnumerable <InfosTabData.Info> GetAll(InfosTabData.Info infos)
    {
        List <InfosTabData.Info> result = new();

        var isCategory = infos.subInfos is {Count: > 0};

        if (!isCategory)
            result.Add(infos);
        else
        {
            foreach (InfosTabData.Info subInfo in infos.subInfos)
                result.AddRange(GetAll(subInfo));
        }

        return result;
    }

    private void Add(InfosTabData.Info info)
    {
        _displayedInfos.Add(info);

        if (info.subInfos is not {Count: > 0})
            return;

        if (!_openInfos.Contains(info))
            return;

        foreach (InfosTabData.Info subInfo in info.subInfos)
            Add(subInfo);
    }

    private void Clear()
    {
        if (!_active)
            return;

        _content.Clear();
    }

    private void OnSearchFieldChange(ChangeEvent <string> evt)
    {
        if (!_active)
            return;

        SetDisplayed(_searchField.value);
    }

    private void OnUpdateRightPane(IEnumerable <int> _)
    {
        if (_list.selectedItem == null)
            return;

        var castedItem = (InfosTabData.Info)_list.selectedItem;

        if (_currentVisualElement != null)
        {
            _currentVisualElement.Q <Label>("Name").style.borderLeftWidth = 0;
            _currentVisualElement = null;
        }

        if (castedItem.subInfos is {Count: > 0})
        {
            _displayedInfos = new List <InfosTabData.Info>();

            _openInfos ??= new List <InfosTabData.Info>();

            _visualElements = new List <VisualElement>();

            if (_openInfos.Contains(castedItem))
                _openInfos.Remove(castedItem);
            else
                _openInfos.Add(castedItem);

            foreach (InfosTabData.Info info in InfosTabData.Infos)
                Add(info);

            _list.itemsSource = _displayedInfos;
            _list.RefreshItems();
        }
        else
        {
            _currentVisualElement = _visualElements[_list.selectedIndex];
            _currentVisualElement.Q <Label>("Name").style.borderLeftWidth = 2.5f;

            UpdateRightPane();
        }

        _list.ClearSelection();
    }

    private void RegisterCallbacks()
    {
        _list.makeItem = () => GUIUtility.Instantiate(_itemTemplate);

        _list.bindItem = (element, i) =>
        {
            _openInfos ??= new List <InfosTabData.Info>();

            _visualElements ??= new List <VisualElement>();

            if (!_visualElements.Contains(element))
                _visualElements.Add(element);

            InfosTabData.Info info = _displayedInfos[i];

            var nameLabel = element.Q <Label>("Name");
            nameLabel.text = info.infoName;
            nameLabel.style.borderLeftWidth = _currentVisualElement == element ? 2.5f : 0;

            element.style.marginLeft = _inSearch ? 0 : info.intendLevel * 10;

            element.Q <VisualElement>("Open").style.display = _openInfos.Contains(info)
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            element.Q <VisualElement>("Closed").style.display = info.subInfos is {Count: > 0}
                ? _openInfos.Contains(info) ? DisplayStyle.None : DisplayStyle.Flex
                : DisplayStyle.None;
        };

        _list.destroyItem = element =>
        {
            _visualElements ??= new List <VisualElement>();

            if (_visualElements.Contains(element))
                _visualElements.Remove(element);

            element.Clear();
        };

        _list.selectedIndicesChanged += OnUpdateRightPane;

        _searchField.RegisterValueChangedCallback(OnSearchFieldChange);
    }

    private void SetDisplayed(string searchString)
    {
        _displayedInfos = searchString.Equals("")
            ? InfosTabData.Infos
            : _allInfos.Where(
                               info =>
                                   info.infoName.ToLower().Contains(searchString.ToLower())).
                           ToList();

        if (!searchString.Equals(""))
        {
            _inSearch = true;
            _displayedInfos.Sort();
        }
        else
            _inSearch = false;

        _openInfos = new List <InfosTabData.Info>();
        _visualElements = new List <VisualElement>();

        _list.ClearSelection();
        _currentVisualElement = null;

        _list.itemsSource = _displayedInfos;
        _list.RefreshItems();
    }

    private void UpdateRightPane()
    {
        Clear();

        if (_list.selectedItem == null)
            return;

        InfosTabData.InfoKey currentInfoKey =
            ((InfosTabData.Info)_list.selectedItem).infoKey;

        InfosTabDisplay.Display(_content, currentInfoKey);

        BaseWindow.onRightPaneInitialization?.Invoke();
    }

    #endregion
}

}
#endif
