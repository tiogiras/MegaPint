#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Packages.Data
{

internal static class PackageDataPlayModeStartScene
{
    #region Public Methods

    public static PackageData Get()
    {
        return new PackageData
        {
            key = PackageKey.PlayModeStartScene,
            reqMpVersion = "1.1.1 or higher",
            version = "1.0.1",
            unityVersion = "2022.3.15f1",
            lastUpdate = "22.02.2024",
            name = "com.tiogiras.megapint-playmodestartscene",
            displayName = "PlayMode Start Scene",
            description = "This package allows you to set a starting scene for entering playmode.",
            repository = "https://github.com/tiogiras/megapint-playmodestartscene.git"
        };
    }

    #endregion
}

}
#endif
