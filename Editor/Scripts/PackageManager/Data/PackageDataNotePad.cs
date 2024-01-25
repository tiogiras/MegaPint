#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Data
{

internal static class PackageDataNotePad
{
    #region Public Methods

    public static MegaPintPackagesData.MegaPintPackageData Get()
    {
        return new MegaPintPackagesData.MegaPintPackageData
        {
            // Git Info & Identification
            gitUrl = "https://github.com/tiogiras/MegaPint-NotePad.git",
            packageKey = MegaPintPackagesData.PackageKey.NotePad,
        
            // Versions
            version = "1.0.0",
            unityVersion = "2022.3.15f1 or higher",
            megaPintVersion = "1.1.0 or higher",
        
            // Metadata
            infoText = "NotePad allows you to make and edit notes directly on a GameObjects MonoBehaviour.",            
            lastUpdate = "25.01.2024",
            packageName = "com.tiogiras.megapint-notepad",
            packageNiceName = "NotePad"
        };
    }

    #endregion
}

}
#endif
