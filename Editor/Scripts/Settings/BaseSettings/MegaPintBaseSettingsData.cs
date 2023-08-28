#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace Editor.Scripts.Settings.BaseSettings
{
    public static class MegaPintBaseSettingsData
    {
        public struct Setting : IComparable<Setting>
        {
            public SettingKey SettingKey;
            public string SettingName;
            public int IntendLevel;
            public List<Setting> SubSettings;
            
            public int CompareTo(Setting other)
            {
                return string.Compare(SettingName, other.SettingName, StringComparison.Ordinal);
            }
        }

        public static readonly List<Setting> Settings = new()
        {
            new Setting
            {
                SettingName = "Help",
                IntendLevel = 0,
                SubSettings = new List<Setting>
                {
                    new()
                    {
                        SettingKey = SettingKey.Contact,
                        SettingName = "Contact",
                        IntendLevel = 1
                    },
                    new()
                    {
                        SettingName = "How To's",
                        IntendLevel = 1,
                        SubSettings = new List<Setting>
                        {
                            new ()
                            {
                                SettingKey = SettingKey.UpdateBasePackage,
                                SettingName = "How To: Update Base Package",
                                IntendLevel = 2
                            },
                            new ()
                            {
                                SettingKey = SettingKey.ManagePackages,
                                SettingName = "How To: Manage Packages",
                                IntendLevel = 2
                            },
                            new ()
                            {
                                SettingKey = SettingKey.UsePackages,
                                SettingName = "How To: Use Packages",
                                IntendLevel = 2
                            }
                        }
                    }
                }
            }
        };
        
        public enum SettingKey
        {
            Contact, UpdateBasePackage, ManagePackages, UsePackages
        }
    }
}
#endif