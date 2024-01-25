#if UNITY_EDITOR
using System.Collections.Generic;

namespace Editor.Scripts.PackageManager.Data
{

internal static class PackageDataAlphaButton
{
    #region Public Methods

    public static MegaPintPackagesData.MegaPintPackageData Get()
    {
        return new MegaPintPackagesData.MegaPintPackageData
        {
            // Git Info & Identification
            gitUrl = "https://github.com/tiogiras/MegaPint-AlphaButton.git",
            packageKey = MegaPintPackagesData.PackageKey.AlphaButton,
        
            // Versions
            version = "1.0.0",
            unityVersion = "2022.3.15f1 or higher",
            megaPintVersion = "1.1.0 or higher",
        
            // Metadata
            infoText =
                "This package adds a new button component that makes use of the build in alpha-threshold of a image component, " +
                "so that a button is only clickable where it's alpha is greater than the specified value.",            
            lastUpdate = "20.01.2024",
            packageName = "com.tiogiras.megapint-alphabutton",
            packageNiceName = "Alpha Button",

            // Dependencies & Variations
            variations = new List <MegaPintPackagesData.MegaPintPackageData.PackageVariation>
            {
                new()
                {
                    // Git Info & Identification
                    gitUrl = "https://github.com/tiogiras/MegaPint-AlphaButton.git",
                    variationTag = "a",
            
                    // Version
                    version = "1.0.0",
            
                    // Metadata
                    niceName = "MegaPint Validators Integration",

                    dependencies = new List <MegaPintPackagesData.MegaPintPackageData.Dependency>
                    {
                        new() {niceName = "Validators", packageKey = MegaPintPackagesData.PackageKey.Validators}
                    }
                }
            }
        };
    }

    #endregion
}

}
#endif
