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
            public string PackageName;
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
                PackageName = "com.tiogiras.megapint-autosave",
                PackageNiceName = "Scene-AutoSave",
                GitUrl = "https://github.com/tiogiras/MegaPint-AutoSave.git",
                Version = "1.0.1",
                LastUpdate = "07.08.2023",
                UnityVersion = "2021.3.20f1 or higher",
                MegaPintVersion = "1.0.0 or higher"
            },
            new MegaPintPackageData
            {
                PackageKey = PackageKey.Test,
                PackageName = "com.tiogiras.test",
                PackageNiceName = "Test",
                GitUrl = "",
                Version = "1.0.0",
                LastUpdate = "07.08.2023",
                UnityVersion = "2021.3.20f1 or higher",
                MegaPintVersion = "1.0.0 or higher"
            }
        };

        public enum PackageKey
        {
            AutoSave,
            Test
        }
    }
}