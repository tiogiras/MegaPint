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
            version = "1.0.1",
            unityVersion = "2022.3.15f1",
            lastUpdate = "22.02.2024",
            name = "com.tiogiras.megapint-validators",
            displayName = "Validators",
            description = "MegaPint package for validation functions",
            repository = "https://github.com/tiogiras/MegaPint-Validators.git"
        };
    }

    #endregion
}

}
#endif
