using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor.Scripts
{
    public static class MegaPintPackagesData
    {
        public class MegaPintPackageData
        {
            public PackageKey PackageKey;
            public string PackageNiceName;
            public string GitUrl;
            public string Version;
            public string LastUpdate;
            public string UnityVersion;
            public string MegaPintVersion;
        }

        public static MegaPintPackageData PackageData(PackageKey packageKey) => 
            Packages.First(package => package.PackageKey == packageKey);

        public static readonly List<MegaPintPackageData> Packages = new()
        {
            new MegaPintPackageData
            {
                PackageKey = PackageKey.AutoSave,
                PackageNiceName = "Scene-AutoSave",
                GitUrl = "https://github.com/tiogiras/MegaPint-AutoSave.git",
                Version = "1.0.0",
                LastUpdate = "07.08.2023",
                UnityVersion = "2021.3.20f1 or higher",
                MegaPintVersion = "1.0.0 or higher"
            }
        };

        public enum PackageKey
        {
            AutoSave
        }
    }
}