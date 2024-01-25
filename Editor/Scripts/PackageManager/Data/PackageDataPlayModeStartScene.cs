#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Data
{

internal static class PackageDataPlayModeStartScene
{
    #region Public Methods

    public static MegaPintPackagesData.MegaPintPackageData Get()
    {
        return new MegaPintPackagesData.MegaPintPackageData
        {
            // Git Info & Identification
            gitUrl = "https://github.com/tiogiras/megapint-playmodestartscene.git",
            packageKey = MegaPintPackagesData.PackageKey.PlayModeStartScene,
        
            // Versions
            version = "1.0.0",
            unityVersion = "2022.3.15f1 or higher",
            megaPintVersion = "1.1.0 or higher",
        
            // Metadata
            infoText = "This package allows you to select a starting scene for entering playmode. This makes entering the game from a main menu much easier.",            
            lastUpdate = "15.01.2024",
            packageName = "com.tiogiras.megapint-playmodestartscene",
            packageNiceName = "PlayMode Start Scene"
        };
    }

    #endregion
}

}
#endif
