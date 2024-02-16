#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Packages.Data
{

internal static class PackageDataNotePad
{
    #region Public Methods

    public static PackageData Get()
    {
        return new PackageData
        {
            key = PackageKey.NotePad,
            reqMpVersion = "1.1.0 or higher",
            name = "com.tiogiras.megapint-notepad",
        };
    }

    #endregion
}

}
#endif
