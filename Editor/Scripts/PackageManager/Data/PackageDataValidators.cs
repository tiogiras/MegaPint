#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Data
{

internal static class PackageDataValidators
{
    #region Public Methods

    public static MegaPintPackagesData.MegaPintPackageData Get()
    {
        return new MegaPintPackagesData.MegaPintPackageData
        {
            // Git Info & Identification
            gitUrl = "https://github.com/tiogiras/MegaPint-Validators.git",
            packageKey = MegaPintPackagesData.PackageKey.Validators,
        
            // Versions
            version = "1.0.1",
            unityVersion = "2022.3.15f1 or higher",
            megaPintVersion = "1.1.0 or higher",
        
            // Metadata
            infoText = "Validators adds a system to create validatable MonoBehaviours, which can be extended with your own requirements.\n" +
                       "See the status and occured issues on all GameObjects with validatable MonoBehaviours in one window and automatically fix any occuring issue.",            
            lastUpdate = "20.01.2024",
            packageName = "com.tiogiras.megapint-validators",
            packageNiceName = "Validators"
        };
    }

    #endregion
}

}
#endif
