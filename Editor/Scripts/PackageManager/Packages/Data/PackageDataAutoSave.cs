#if UNITY_EDITOR
namespace MegaPint.Editor.Scripts.PackageManager.Packages.Data
{

/// <summary> Data for the AutoSave package </summary>
internal static class PackageDataAutoSave
{
    #region Public Methods

    /// <summary> Get the data for this package </summary>
    /// <returns> Corresponding <see cref="PackageData" /> </returns>
    public static PackageData Get()
    {
        return new PackageData
        {
            key = PackageKey.AutoSave,
            reqMpVersion = "1.1.1 or higher",
            version = "1.0.2",
            unityVersion = "2022.3.15f1",
            lastUpdate = "26.02.2024",
            name = "com.tiogiras.megapint-autosave",
            displayName = "Scene-AutoSave",
            description =
                "This AutoSave package adds functionality to save your opened scenes in set intervals. You can overwrite your current scene file or create timestamped backup files.\n\nCustomize once and never worry about scene saving again.",
            repository = "https://github.com/tiogiras/MegaPint-AutoSave.git"
        };
    }

    #endregion
}

}
#endif
