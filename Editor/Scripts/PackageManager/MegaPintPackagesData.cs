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
        AutoSave, Validators, AlphaButton, PlayModeStartScene
    }

    public static readonly List <MegaPintPackageData> Packages = new()
    {
        new MegaPintPackageData
        {
            PackageKey = PackageKey.AutoSave,
            PackageName = "com.tiogiras.megapint-autosave",
            PackageNiceName = "Scene-AutoSave",
            GitUrl = "https://github.com/tiogiras/MegaPint-AutoSave.git#febb86f0c4921218a96d973b8359bf419c7ff8ce",
            Version = "1.0.0",
            LastUpdate = "10.01.2024",
            UnityVersion = "2022.3.15f1 or higher",
            MegaPintVersion = "1.0.0 or higher",
            InfoText = "Scene-AutoSave is a small tool that provides you with an additional layer of protection when it comes to saving your scenes."
        },
        new MegaPintPackageData
        {
            PackageKey = PackageKey.Validators,
            PackageName = "com.tiogiras.megapint-validators",
            PackageNiceName = "Validators",
            GitUrl = "https://github.com/tiogiras/MegaPint-Validators.git#24e5ca177735c34330c14953dd8fdcc765344eea",
            Version = "1.0.0",
            LastUpdate = "10.01.2024",
            UnityVersion = "2022.3.15f1 or higher",
            MegaPintVersion = "1.0.0 or higher",
            InfoText = "Validators adds a system to create validatable MonoBehaviours, which can be extended with your own requirements.\n" +
                       "See the status and occured issues on all GameObjects with validatable MonoBehaviours in one window and automatically fix any occuring issue."
        },
        new MegaPintPackageData
        {
            PackageKey = PackageKey.AlphaButton,
            PackageName = "com.tiogiras.megapint-alphabutton",
            PackageNiceName = "Alpha Button",
            GitUrl = "",
            Version = "1.0.0",
            LastUpdate = "10.01.2024",
            UnityVersion = "2022.3.15f1 or higher",
            MegaPintVersion = "1.1.0 or higher",
            InfoText = ""
        },
        new MegaPintPackageData
        {
            PackageKey = PackageKey.PlayModeStartScene,
            PackageName = "com.tiogiras.megapint-playmodestartscene",
            PackageNiceName = "PlayMode Start Scene",
            GitUrl = "",
            Version = "1.0.0",
            LastUpdate = "14.01.2024",
            UnityVersion = "2022.3.15f1 or higher",
            MegaPintVersion = "1.1.0 or higher",
            InfoText = ""
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
