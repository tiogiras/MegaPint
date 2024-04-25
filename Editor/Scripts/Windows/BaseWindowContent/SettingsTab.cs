#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.Scripts.Settings.BaseSettings;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = Editor.Scripts.GUI.GUIUtility;

namespace Editor.Scripts.Windows.BaseWindowContent
{

internal class SettingsTab
{
    private readonly string _itemPath = Path.Combine(Constants.BasePackage.PathWindows, "Base Window", "Setting item");

    private readonly VisualTreeAsset _itemTemplate;

    private readonly ToolbarSearchField _searchField;

    private readonly VisualElement _settingsContent;

    private readonly ListView _settingsList;

    private bool _active;
    private List <MegaPintBaseSettingsData.Setting> _allSettings;

    private VisualElement _currentVisualElement;
    private List <MegaPintBaseSettingsData.Setting> _displayedSettings;
    private bool _inSearch;

    private List <MegaPintBaseSettingsData.Setting> _openSettings;
    private List <VisualElement> _visualElements;

    public SettingsTab(VisualElement root)
    {
        _itemTemplate = Resources.Load <VisualTreeAsset>(_itemPath);

        _settingsList = root.Q <ListView>("SettingsList");
        _searchField = root.Q <ToolbarSearchField>("SearchField");

        var rightPane = root.Q <VisualElement>("RightPane");

        _settingsContent = rightPane.Q <VisualElement>("SettingsContent");

        RegisterCallbacks();

        Hide();
    }

    #region Public Methods

    public void Hide()
    {
        Clear();

        _settingsList.style.display = DisplayStyle.None;
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
        SetDisplayedSettings("");

        _displayedSettings = MegaPintBaseSettingsData.Settings;
        _openSettings = new List <MegaPintBaseSettingsData.Setting>();
        _visualElements = new List <VisualElement>();

        _inSearch = false;

        _settingsList.itemsSource = _displayedSettings;
        _settingsList.RefreshItems();

        _settingsList.style.display = DisplayStyle.Flex;
        _active = true;

        if (_allSettings != null)
            return;

        _allSettings = new List <MegaPintBaseSettingsData.Setting>();

        foreach (MegaPintBaseSettingsData.Setting setting in MegaPintBaseSettingsData.Settings)
            _allSettings.AddRange(GetAllSettings(setting));
    }

    #endregion

    #region Private Methods

    private static IEnumerable <MegaPintBaseSettingsData.Setting> GetAllSettings(
        MegaPintBaseSettingsData.Setting setting)
    {
        List <MegaPintBaseSettingsData.Setting> result = new();

        var isCategory = setting.subSettings is {Count: > 0};

        if (!isCategory)
            result.Add(setting);
        else
        {
            foreach (MegaPintBaseSettingsData.Setting subSetting in setting.subSettings)
                result.AddRange(GetAllSettings(subSetting));
        }

        return result;
    }

    private void AddSetting(MegaPintBaseSettingsData.Setting setting)
    {
        _displayedSettings.Add(setting);

        if (setting.subSettings is not {Count: > 0})
            return;

        if (!_openSettings.Contains(setting))
            return;

        foreach (MegaPintBaseSettingsData.Setting subSetting in setting.subSettings)
            AddSetting(subSetting);
    }

    private void Clear()
    {
        if (!_active)
            return;

        _settingsContent.Clear();
    }

    private void OnSearchFieldChange(ChangeEvent <string> evt)
    {
        if (!_active)
            return;

        SetDisplayedSettings(_searchField.value);
    }

    private void OnUpdateRightPane(IEnumerable <int> _)
    {
        if (_settingsList.selectedItem == null)
            return;

        var castedItem = (MegaPintBaseSettingsData.Setting)_settingsList.selectedItem;

        if (_currentVisualElement != null)
        {
            _currentVisualElement.Q <Label>("Name").style.borderLeftWidth = 0;
            _currentVisualElement = null;
        }

        if (castedItem.subSettings is {Count: > 0})
        {
            _displayedSettings = new List <MegaPintBaseSettingsData.Setting>();

            _openSettings ??= new List <MegaPintBaseSettingsData.Setting>();

            _visualElements = new List <VisualElement>();

            if (_openSettings.Contains(castedItem))
                _openSettings.Remove(castedItem);
            else
                _openSettings.Add(castedItem);

            foreach (MegaPintBaseSettingsData.Setting setting in MegaPintBaseSettingsData.Settings)
                AddSetting(setting);

            _settingsList.itemsSource = _displayedSettings;
            _settingsList.RefreshItems();
        }
        else
        {
            _currentVisualElement = _visualElements[_settingsList.selectedIndex];
            _currentVisualElement.Q <Label>("Name").style.borderLeftWidth = 2.5f;

            UpdateRightPane();
        }

        _settingsList.ClearSelection();
    }

    private void RegisterCallbacks()
    {
        _settingsList.makeItem = () => GUIUtility.Instantiate(_itemTemplate);

        _settingsList.bindItem = (element, i) =>
        {
            _openSettings ??= new List <MegaPintBaseSettingsData.Setting>();

            _visualElements ??= new List <VisualElement>();

            if (!_visualElements.Contains(element))
                _visualElements.Add(element);

            MegaPintBaseSettingsData.Setting setting = _displayedSettings[i];

            var nameLabel = element.Q <Label>("Name");
            nameLabel.text = setting.settingName;
            nameLabel.style.borderLeftWidth = _currentVisualElement == element ? 2.5f : 0;

            element.style.marginLeft = _inSearch ? 0 : setting.intendLevel * 10;

            element.Q <VisualElement>("Open").style.display = _openSettings.Contains(setting)
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            element.Q <VisualElement>("Closed").style.display = setting.subSettings is {Count: > 0}
                ? _openSettings.Contains(setting) ? DisplayStyle.None : DisplayStyle.Flex
                : DisplayStyle.None;
        };

        _settingsList.destroyItem = element =>
        {
            _visualElements ??= new List <VisualElement>();

            if (_visualElements.Contains(element))
                _visualElements.Remove(element);

            element.Clear();
        };

        _settingsList.selectedIndicesChanged += OnUpdateRightPane;

        _searchField.RegisterValueChangedCallback(OnSearchFieldChange);
    }

    private void SetDisplayedSettings(string searchString)
    {
        _displayedSettings = searchString.Equals("")
            ? MegaPintBaseSettingsData.Settings
            : _allSettings.Where(
                               setting =>
                                   setting.settingName.ToLower().Contains(searchString.ToLower())).
                           ToList();

        if (!searchString.Equals(""))
        {
            _inSearch = true;
            _displayedSettings.Sort();
        }
        else
            _inSearch = false;

        _openSettings = new List <MegaPintBaseSettingsData.Setting>();
        _visualElements = new List <VisualElement>();

        _settingsList.ClearSelection();
        _currentVisualElement = null;

        _settingsList.itemsSource = _displayedSettings;
        _settingsList.RefreshItems();
    }

    private void UpdateRightPane()
    {
        Clear();

        if (_settingsList.selectedItem == null)
            return;

        MegaPintBaseSettingsData.SettingKey currentSettingKey =
            ((MegaPintBaseSettingsData.Setting)_settingsList.selectedItem).settingKey;

        MegaPintBaseSettingsDisplay.Display(_settingsContent, currentSettingKey);

        BaseWindow.onRightPaneInitialization?.Invoke();
    }

    #endregion
}

}
#endif
