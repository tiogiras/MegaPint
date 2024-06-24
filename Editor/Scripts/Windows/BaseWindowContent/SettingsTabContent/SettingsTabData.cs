#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace MegaPint.Editor.Scripts.Windows.BaseWindowContent.SettingsTabContent
{

/// <summary> Stores data for the settings tab in the baseWindow </summary>
internal static class SettingsTabData
{
    public struct Setting : IComparable <Setting>
    {
        public SettingsKey settingsKey;
        public string settingsName;
        public int intendLevel;
        public List <Setting> subSettings;

        public int CompareTo(Setting other)
        {
            return string.Compare(settingsName, other.settingsName, StringComparison.Ordinal);
        }
    }

    public enum SettingsKey
    {
        Theme, Feedback, Testing
    }

    public static readonly List <Setting> Settings = new()
    {
        new Setting
        {
            settingsName = "Appearance",
            intendLevel = 0,
            subSettings = new List <Setting>
            {
                new() {settingsKey = SettingsKey.Theme, settingsName = "Theme", intendLevel = 1}
            }
        },
        new Setting
        {
            settingsKey = SettingsKey.Feedback,
            settingsName = "Feedback",
            intendLevel = 0
        },
        new Setting
        {
            settingsKey = SettingsKey.Testing,
            settingsName = "Testing",
            intendLevel = 0
        }
    };
}

}
#endif
