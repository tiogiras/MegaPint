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
            reqMpVersion = "1.1.0 or higher",
            name = "com.tiogiras.megapint-playmodestartscene",
        };
    }

    #endregion
}

}
#endif
