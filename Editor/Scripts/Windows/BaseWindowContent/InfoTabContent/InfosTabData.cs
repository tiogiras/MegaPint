#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace Editor.Scripts.Windows.BaseWindowContent.InfoTabContent
{
    internal static class InfosTabData
    {
        public struct Info : IComparable<Info>
        {
            public InfoKey infoKey;
            public string infoName;
            public int intendLevel;
            public List<Info> subInfos;
            
            public int CompareTo(Info other)
            {
                return string.Compare(infoName, other.infoName, StringComparison.Ordinal);
            }
        }

        public static readonly List<Info> Infos = new()
        {
            new Info
            {
                infoName = "Help",
                intendLevel = 0,
                subInfos = new List<Info>
                {
                    new()
                    {
                        infoName = "How To's",
                        intendLevel = 1,
                        subInfos = new List<Info>
                        {
                            new ()
                            {
                                infoKey = InfoKey.UpdateBasePackage,
                                infoName = "How To: Update Base Package",
                                intendLevel = 2
                            },
                            new ()
                            {
                                infoKey = InfoKey.ManagePackages,
                                infoName = "How To: Manage Packages",
                                intendLevel = 2
                            },
                            new ()
                            {
                                infoKey = InfoKey.UsePackages,
                                infoName = "How To: Use Packages",
                                intendLevel = 2
                            },
                            new ()
                            {
                                infoKey = InfoKey.Shortcuts,
                                infoName = "How To: Shortcuts",
                                intendLevel = 2
                            }
                        }
                    }
                }
            },
            new Info
            {
                infoKey = InfoKey.Contact,
                infoName = "Contact",
                intendLevel = 0
            }
        };
        
        public enum InfoKey
        {
            Contact, UpdateBasePackage, ManagePackages, UsePackages, Shortcuts
        }
    }
}
#endif