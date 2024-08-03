#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace MegaPint.Editor.Scripts.Windows.BaseWindowContent.SettingsTabContent
{

/// <summary> Stores data for the settings tab in the baseWindow </summary>
internal static class SettingsTabData
{
    public class Setting : IComparable <Setting>
    {
        public int intendLevel;
        public SettingsKey settingsKey;
        public string settingsName;
        public List <Setting> subSettings;

        #region Public Methods

        public int CompareTo(Setting other)
        {
            return string.Compare(settingsName, other.settingsName, StringComparison.Ordinal);
        }

        #endregion
    }

    public enum SettingsKey
    {
        Theme, Testing, Toolbar
    }

    public static readonly List <Setting> Settings = new()
    {
        new Setting
        {
            settingsName = "Appearance",
            intendLevel = 0,
            subSettings = new List <Setting>
            {
                new() {settingsKey = SettingsKey.Theme, settingsName = "Theme", intendLevel = 1},
                new() {settingsKey = SettingsKey.Toolbar, settingsName = "Toolbar", intendLevel = 1}
            }
        },
        new Setting {settingsKey = SettingsKey.Testing, settingsName = "Testing", intendLevel = 0}
    };
}

}
#endif
