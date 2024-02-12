#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.PackageManager.Data;

namespace Editor.Scripts.PackageManager
{

internal static class MegaPintPackagesData
{
    internal class MegaPintPackageData : IComparable <MegaPintPackageData>
    {
        internal class Dependency : IComparable <Dependency>
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

        internal class PackageVariation : IComparable <PackageVariation>
        {
            // Git Info & Identification
            public string gitUrl;
            public string variationTag;
            public string developmentBranch;
            
            // Version
            public string version;
            
            // Metadata
            public string niceName;
            
            public List <Dependency> dependencies;

            #region Public Methods

            public int CompareTo(PackageVariation other)
            {
                return string.Compare(niceName, other.niceName, StringComparison.Ordinal);
            }

            #endregion
        }

        // Git Info & Identification
        public string gitUrl;
        public PackageKey packageKey;
        
        // Versions
        public string version;
        public string unityVersion;
        public string megaPintVersion;
        
        // Metadata
        public string infoText;
        public string lastUpdate;
        public string packageName;
        public string packageNiceName;

        // Dependencies & Variations
        public List <Dependency> dependencies;
        public List <PackageVariation> variations;

        #region Public Methods

        public int CompareTo(MegaPintPackageData other)
        {
            return string.Compare(packageNiceName, other.packageNiceName, StringComparison.Ordinal);
        }

        #endregion
    }

    internal enum PackageKey
    {
        AutoSave, Validators, AlphaButton, PlayModeStartScene, NotePad
    }

    internal static readonly List <MegaPintPackageData> Packages = new()
    {
        PackageDataAutoSave.Get(),
        PackageDataValidators.Get(),
        PackageDataAlphaButton.Get(),
        PackageDataPlayModeStartScene.Get(),
        PackageDataNotePad.Get()
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
