#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Packages.Data
{

internal static class PackageDataValidators
{
    #region Public Methods

    public static PackageData Get()
    {
        return new PackageData
        {
            key = PackageKey.Validators,
            reqMpVersion = "1.1.0 or higher",
            name = "com.tiogiras.megapint-validators",
        };
    }

    #endregion
}

}
#endif
