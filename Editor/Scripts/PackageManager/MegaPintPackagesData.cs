#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor.Scripts.PackageManager
{

public static class MegaPintPackagesData
{
    public class MegaPintPackageData : IComparable <MegaPintPackageData>
    {
        public string GitUrl;
        public string InfoText;
        public string LastUpdate;
        public string MegaPintVersion;
        public PackageKey PackageKey;
        public string PackageName;
        public string PackageNiceName;
        public string UnityVersion;
        public string Version;

        #region Public Methods

        public int CompareTo(MegaPintPackageData other)
        {
            return string.Compare(PackageNiceName, other.PackageNiceName, StringComparison.Ordinal);
        }

        #endregion
    }

    public enum PackageKey
    {
        AutoSave, Validators
    }

    public static readonly List <MegaPintPackageData> Packages = new()
    {
        new MegaPintPackageData
        {
            PackageKey = PackageKey.AutoSave,
            PackageName = "com.tiogiras.megapint-autosave",
            PackageNiceName = "Scene-AutoSave",
            GitUrl = "https://github.com/tiogiras/MegaPint-AutoSave.git",
            Version = "1.0.0",
            LastUpdate = "28.08.2023",
            UnityVersion = "2022.3.15f1 or higher",
            MegaPintVersion = "1.0.0 or higher",
            InfoText = ""
        },
        new MegaPintPackageData
        {
            PackageKey = PackageKey.Validators,
            PackageName = "com.tiogiras.megapint-validators",
            PackageNiceName = "Validators",
            GitUrl = "https://github.com/tiogiras/MegaPint-Validators.git",
            Version = "1.0.0",
            LastUpdate = "18.12.2023",
            UnityVersion = "2022.3.15f1 or higher",
            MegaPintVersion = "1.0.0 or higher",
            InfoText = "Validators adds a system to create validatable MonoBehaviours, which can be extended with your own requirements.\n" +
                       "See the status and occured issues on all GameObjects with validatable MonoBehaviours in one window and automatically fix any occuring issue."
        }
    };

    #region Public Methods

    public static MegaPintPackageData PackageData(PackageKey packageKey)
    {
        return Packages.First(package => package.PackageKey == packageKey);
    }

    #endregion
}

}
#endif
