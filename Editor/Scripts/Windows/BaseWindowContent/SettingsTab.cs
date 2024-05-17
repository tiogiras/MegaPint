#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.Scripts.Windows.BaseWindowContent.SettingsTabContent;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = Editor.Scripts.GUI.GUIUtility;

namespace Editor.Scripts.Windows.BaseWindowContent
{

internal class SettingsTab
{
    private readonly string _itemPath = Path.Combine(Constants.BasePackage.PathWindows, "Base Window", "Info item");

    private readonly VisualTreeAsset _itemTemplate;

    private readonly ToolbarSearchField _searchField;

    private readonly VisualElement _content;

    private readonly ListView _list;

    private bool _active;
    private List <SettingsTabData.Setting> _allSettings;

    private VisualElement _currentVisualElement;
    private List <SettingsTabData.Setting> _displayedSettings;
    private bool _inSearch;

    private List <SettingsTabData.Setting> _openSettings;
    private List <VisualElement> _visualElements;

    public SettingsTab(VisualElement root)
    {
        _itemTemplate = Resources.Load <VisualTreeAsset>(_itemPath);

        _list = root.Q <ListView>("SettingsList");
        _searchField = root.Q <ToolbarSearchField>("SearchField");

        var rightPane = root.Q <VisualElement>("RightPane");

        _content = rightPane.Q <VisualElement>("SettingsContent");

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

        _displayedSettings = SettingsTabData.Settings;
        _openSettings = new List <SettingsTabData.Setting>();
        _visualElements = new List <VisualElement>();

        _inSearch = false;

        _list.itemsSource = _displayedSettings;
        _list.RefreshItems();

        _list.style.display = DisplayStyle.Flex;
        _active = true;

        if (_allSettings != null)
            return;

        _allSettings = new List <SettingsTabData.Setting>();

        foreach (SettingsTabData.Setting setting in SettingsTabData.Settings)
            _allSettings.AddRange(GetAll(setting));
    }

    #endregion

    #region Private Methods

    private static IEnumerable <SettingsTabData.Setting> GetAll(SettingsTabData.Setting settings)
    {
        List <SettingsTabData.Setting> result = new();

        var isCategory = settings.subSettings is {Count: > 0};

        if (!isCategory)
            result.Add(settings);
        else
        {
            foreach (SettingsTabData.Setting subSetting in settings.subSettings)
                result.AddRange(GetAll(subSetting));
        }

        return result;
    }

    private void Add(SettingsTabData.Setting setting)
    {
        _displayedSettings.Add(setting);

        if (setting.subSettings is not {Count: > 0})
            return;

        if (!_openSettings.Contains(setting))
            return;

        foreach (SettingsTabData.Setting subSettings in setting.subSettings)
            Add(subSettings);
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

        var castedItem = (SettingsTabData.Setting)_list.selectedItem;

        if (_currentVisualElement != null)
        {
            _currentVisualElement.Q <Label>("Name").style.borderLeftWidth = 0;
            _currentVisualElement = null;
        }

        if (castedItem.subSettings is {Count: > 0})
        {
            _displayedSettings = new List <SettingsTabData.Setting>();

            _openSettings ??= new List <SettingsTabData.Setting>();

            _visualElements = new List <VisualElement>();

            if (_openSettings.Contains(castedItem))
                _openSettings.Remove(castedItem);
            else
                _openSettings.Add(castedItem);

            foreach (SettingsTabData.Setting setting in SettingsTabData.Settings)
                Add(setting);

            _list.itemsSource = _displayedSettings;
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
            _openSettings ??= new List <SettingsTabData.Setting>();

            _visualElements ??= new List <VisualElement>();

            if (!_visualElements.Contains(element))
                _visualElements.Add(element);

            SettingsTabData.Setting setting = _displayedSettings[i];

            var nameLabel = element.Q <Label>("Name");
            nameLabel.text = setting.settingsName;
            nameLabel.style.borderLeftWidth = _currentVisualElement == element ? 2.5f : 0;

            element.style.marginLeft = _inSearch ? 0 : setting.intendLevel * 10;

            element.Q <VisualElement>("Open").style.display = _openSettings.Contains(setting)
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            element.Q <VisualElement>("Closed").style.display = setting.subSettings is {Count: > 0}
                ? _openSettings.Contains(setting) ? DisplayStyle.None : DisplayStyle.Flex
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
        _displayedSettings = searchString.Equals("")
            ? SettingsTabData.Settings
            : _allSettings.Where(
                               setting =>
                                   setting.settingsName.ToLower().Contains(searchString.ToLower())).
                           ToList();

        if (!searchString.Equals(""))
        {
            _inSearch = true;
            _displayedSettings.Sort();
        }
        else
            _inSearch = false;

        _openSettings = new List <SettingsTabData.Setting>();
        _visualElements = new List <VisualElement>();

        _list.ClearSelection();
        _currentVisualElement = null;

        _list.itemsSource = _displayedSettings;
        _list.RefreshItems();
    }

    private void UpdateRightPane()
    {
        Clear();

        if (_list.selectedItem == null)
            return;

        SettingsTabData.SettingsKey currentSettingKey =
            ((SettingsTabData.Setting)_list.selectedItem).settingsKey;

        SettingsTabDisplay.Display(_content, currentSettingKey);

        BaseWindow.onRightPaneInitialization?.Invoke();
    }

    #endregion
}

}
#endif
