#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace Editor.Scripts.Windows.BaseWindowContent.SettingsTabContent
{
    internal static class SettingsTabData
    {
        public struct Setting : IComparable<Setting>
        {
            public SettingsKey settingsKey;
            public string settingsName;
            public int intendLevel;
            public List<Setting> subSettings;
            
            public int CompareTo(Setting other)
            {
                return string.Compare(settingsName, other.settingsName, StringComparison.Ordinal);
            }
        }

        public static readonly List<Setting> Settings = new()
        {
            new Setting
            {
                settingsName = "Appearance",
                intendLevel = 0,
                subSettings = new List<Setting>
                {
                    new()
                    {
                        settingsKey = SettingsKey.Theme,
                        settingsName = "Theme",
                        intendLevel = 1
                    }
                }
            }
        };
        
        public enum SettingsKey
        {
            Theme
        }
    }
}
#endif