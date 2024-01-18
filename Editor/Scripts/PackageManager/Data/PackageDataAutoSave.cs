using System.Collections.Generic;

#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Data
{

public static class PackageDataAutoSave
{
    #region Public Methods

    public static MegaPintPackagesData.MegaPintPackageData Get()
    {
        return new MegaPintPackagesData.MegaPintPackageData
        {
            packageKey = MegaPintPackagesData.PackageKey.AutoSave,
            packageName = "com.tiogiras.megapint-autosave",
            packageNiceName = "Scene-AutoSave",
            gitUrl = "https://github.com/tiogiras/MegaPint-AutoSave.git#febb86f0c4921218a96d973b8359bf419c7ff8ce",
            version = "1.0.0",
            lastUpdate = "10.01.2024",
            unityVersion = "2022.3.15f1 or higher",
            megaPintVersion = "1.0.0 or higher",
            infoText =
                "Scene-AutoSave is a small tool that provides you with an additional layer of protection when it comes to saving your scenes."
        };
    }

    #endregion
}

}
#endif
