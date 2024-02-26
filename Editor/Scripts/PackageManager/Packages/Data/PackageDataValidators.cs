#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Packages.Data
{

/// <summary> Data for the Validators package </summary>
internal static class PackageDataValidators
{
    #region Public Methods

    /// <summary> Get the data for this package </summary>
    /// <returns> Corresponding <see cref="PackageData" /> </returns>
    public static PackageData Get()
    {
        return new PackageData
        {
            key = PackageKey.Validators,
            reqMpVersion = "1.1.1 or higher",
            version = "1.0.2",
            unityVersion = "2022.3.15f1",
            lastUpdate = "22.02.2024",
            name = "com.tiogiras.megapint-validators",
            displayName = "Validators",
            description =
                "This package adds an option to use and create own validations for MonoBehaviours. The results of the validations are easily readable in one window and can be automatically fixed.",
            repository = "https://github.com/tiogiras/MegaPint-Validators.git"
        };
    }

    #endregion
}

}
#endif
