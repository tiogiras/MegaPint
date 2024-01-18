#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Data
{

public static class PackageDataPlayModeStartScene
{
    #region Public Methods

    public static MegaPintPackagesData.MegaPintPackageData Get()
    {
        return new MegaPintPackagesData.MegaPintPackageData
        {
            packageKey = MegaPintPackagesData.PackageKey.PlayModeStartScene,
            packageName = "com.tiogiras.megapint-playmodestartscene",
            packageNiceName = "PlayMode Start Scene",
            gitUrl = "https://github.com/tiogiras/megapint-playmodestartscene.git#b3aa4eea16ae258f5a08a4dc3c6ae52776e46f92",
            version = "1.0.0",
            lastUpdate = "15.01.2024",
            unityVersion = "2022.3.15f1 or higher",
            megaPintVersion = "1.1.0 or higher",
            infoText =
                "This package allows you to select a starting scene for entering playmode. This makes entering the game from a main menu much easier."
        };
    }

    #endregion
}

}
#endif
