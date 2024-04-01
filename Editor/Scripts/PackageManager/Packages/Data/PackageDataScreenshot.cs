#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Packages.Data
{

/// <summary> Data for the Screenshot package </summary>
internal static class PackageDataScreenshot
{
    #region Public Methods

    /// <summary> Get the data for this package </summary>
    /// <returns> Corresponding <see cref="PackageData" /> </returns>
    public static PackageData Get()
    {
        return new PackageData
        {
            key = PackageKey.Screenshot,
            reqMpVersion = "1.2.0 or higher",
            version = "1.0.0",
            unityVersion = "2022.3.15f1",
            lastUpdate = "01.04.2024",
            name = "com.tiogiras.megapint-screenshot",
            displayName = "Screenshot",
            description = "This package adds a function to render and save camera views and editor windows.",
            repository = "https://github.com/tiogiras/MegaPint-Screenshot.git"
        };
    }

    #endregion
}

}
#endif
