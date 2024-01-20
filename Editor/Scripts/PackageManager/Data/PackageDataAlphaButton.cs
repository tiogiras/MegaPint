using System.Collections.Generic;

#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Data
{

public static class PackageDataAlphaButton
{
    #region Public Methods

    public static MegaPintPackagesData.MegaPintPackageData Get()
    {
        return new MegaPintPackagesData.MegaPintPackageData
        {
            packageKey = MegaPintPackagesData.PackageKey.AlphaButton,
            packageName = "com.tiogiras.megapint-alphabutton",
            packageNiceName = "Alpha Button",
            gitUrl = "https://github.com/tiogiras/MegaPint-AlphaButton.git#development",
            version = "1.0.0",
            lastUpdate = "18.01.2024",
            unityVersion = "2022.3.15f1 or higher",
            megaPintVersion = "1.1.0 or higher",
            infoText =
                "This package adds a new button component that makes use of the build in alpha-threshold of a image component, " +
                "so that a button is only clickable where it's alpha is greater than the specified value.",
            variations = new List <MegaPintPackagesData.MegaPintPackageData.PackageVariation>
            {
                new()
                {
                    niceName = "MegaPint Validators Integration",
                    gitURL = "https://github.com/tiogiras/MegaPint-AlphaButton.git#validatorsIntegration/development",
                    version = "1.0.0",
                    dependencies = new List <MegaPintPackagesData.MegaPintPackageData.Dependency>
                    {
                        new()
                        {
                            niceName = "Validators",
                            packageKey = MegaPintPackagesData.PackageKey.Validators
                        }
                    }
                }
            }
        };
    }

    #endregion
}

}
#endif
