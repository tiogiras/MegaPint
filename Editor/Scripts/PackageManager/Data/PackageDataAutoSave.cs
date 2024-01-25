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
            gitUrl = "https://github.com/tiogiras/MegaPint-AutoSave.git#v1.0.1",
            version = "1.0.1",
            lastUpdate = "20.01.2024",
            unityVersion = "2022.3.15f1 or higher",
            megaPintVersion = "1.1.0 or higher",
            infoText =
                "Scene-AutoSave is a small tool that provides you with an additional layer of protection when it comes to saving your scenes."
        };
    }

    #endregion
}

}
#endif
