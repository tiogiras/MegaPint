#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace Editor.Scripts.Settings.BaseSettings
{
    internal static class MegaPintBaseSettingsData
    {
        public struct Setting : IComparable<Setting>
        {
            public SettingKey settingKey;
            public string settingName;
            public int intendLevel;
            public List<Setting> subSettings;
            
            public int CompareTo(Setting other)
            {
                return string.Compare(settingName, other.settingName, StringComparison.Ordinal);
            }
        }

        public static readonly List<Setting> Settings = new()
        {
            new Setting
            {
                settingName = "Help",
                intendLevel = 0,
                subSettings = new List<Setting>
                {
                    new()
                    {
                        settingName = "How To's",
                        intendLevel = 1,
                        subSettings = new List<Setting>
                        {
                            new ()
                            {
                                settingKey = SettingKey.UpdateBasePackage,
                                settingName = "How To: Update Base Package",
                                intendLevel = 2
                            },
                            new ()
                            {
                                settingKey = SettingKey.ManagePackages,
                                settingName = "How To: Manage Packages",
                                intendLevel = 2
                            },
                            new ()
                            {
                                settingKey = SettingKey.UsePackages,
                                settingName = "How To: Use Packages",
                                intendLevel = 2
                            }
                        }
                    }
                }
            },
            new Setting
            {
                settingKey = SettingKey.Contact,
                settingName = "Contact",
                intendLevel = 0
            }
        };
        
        public enum SettingKey
        {
            Contact, UpdateBasePackage, ManagePackages, UsePackages
        }
    }
}
#endif