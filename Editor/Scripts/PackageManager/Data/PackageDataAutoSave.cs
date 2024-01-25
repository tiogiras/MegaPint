#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Data
{

internal static class PackageDataAutoSave
{
    #region Public Methods

    public static MegaPintPackagesData.MegaPintPackageData Get()
    {
        return new MegaPintPackagesData.MegaPintPackageData
        {
            // Git Info & Identification
            gitUrl = "https://github.com/tiogiras/MegaPint-AutoSave.git",
            packageKey = MegaPintPackagesData.PackageKey.AutoSave,
        
            // Versions
            version = "1.0.1",
            unityVersion = "2022.3.15f1 or higher",
            megaPintVersion = "1.1.0 or higher",
        
            // Metadata
            infoText = "Scene-AutoSave is a small tool that provides you with an additional layer of protection when it comes to saving your scenes.",
            lastUpdate = "20.01.2024",
            packageName = "com.tiogiras.megapint-autosave",
            packageNiceName = "Scene-AutoSave"
        };
    }

    #endregion
}

}
#endif
