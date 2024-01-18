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
            gitUrl = "",
            version = "1.0.0",
            lastUpdate = "18.01.2024",
            unityVersion = "2022.3.15f1 or higher",
            megaPintVersion = "1.1.0 or higher",
            infoText =
                "This package adds a new button component that makes use of the build in alpha-threshold of a image component, " +
                "so that a button is only clickable where it's alpha is greater than the specified value.",
            subPackages = new List <MegaPintPackagesData.MegaPintPackageData.SubPackage>
            {
                new()
                {
                    niceName = "MegaPint Validators Integration",
                    gitURL = "",
                    version = "1.0.0",
                    installationMode = MegaPintPackagesData.MegaPintPackageData.SubPackage.InstallationMode.Replace
                }
            }
        };
    }

    #endregion
}

}
#endif
