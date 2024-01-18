#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.PackageManager.Data;

namespace Editor.Scripts.PackageManager
{

public static class MegaPintPackagesData
{
    public class MegaPintPackageData : IComparable <MegaPintPackageData>
    {
        public class Dependency : IComparable <Dependency>
        {
            public string niceName;
            public PackageKey packageKey;

            #region Public Methods

            public int CompareTo(Dependency other)
            {
                return string.Compare(niceName, other.niceName, StringComparison.Ordinal);
            }

            #endregion
        }

        public class SubPackage : IComparable <SubPackage>
        {
            public enum InstallationMode
            {
                Additional, Replace
            }

            public string gitURL;

            public InstallationMode installationMode;
            public string niceName;
            public string version;

            #region Public Methods

            public int CompareTo(SubPackage other)
            {
                return string.Compare(niceName, other.niceName, StringComparison.Ordinal);
            }

            #endregion
        }

        public string gitUrl;
        public string infoText;
        public string lastUpdate;
        public string megaPintVersion;
        public PackageKey packageKey;
        public string packageName;
        public string packageNiceName;
        public string unityVersion;
        public string version;
        
        public List <SubPackage> subPackages;
        public List <Dependency> dependencies;
        
        #region Public Methods

        public int CompareTo(MegaPintPackageData other)
        {
            return string.Compare(packageNiceName, other.packageNiceName, StringComparison.Ordinal);
        }

        #endregion
    }

    public enum PackageKey
    {
        AutoSave, Validators, AlphaButton, PlayModeStartScene
    }

    public static readonly List <MegaPintPackageData> Packages = new()
    {
        PackageDataAutoSave.Get(), PackageDataValidators.Get(), PackageDataAlphaButton.Get(), PackageDataPlayModeStartScene.Get()
    };

    #region Public Methods

    public static MegaPintPackageData PackageData(PackageKey packageKey)
    {
        return Packages.First(package => package.packageKey == packageKey);
    }

    #endregion
}

}
#endif
