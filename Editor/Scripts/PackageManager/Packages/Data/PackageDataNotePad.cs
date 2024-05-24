#if UNITY_EDITOR
namespace MegaPint.Editor.Scripts.PackageManager.Packages.Data
{

/// <summary> Data for the NotePad package </summary>
internal static class PackageDataNotePad
{
    #region Public Methods

    /// <summary> Get the data for this package </summary>
    /// <returns> Corresponding <see cref="PackageData" /> </returns>
    public static PackageData Get()
    {
        return new PackageData
        {
            key = PackageKey.NotePad,
            reqMpVersion = "1.1.1 or higher",
            version = "1.0.1",
            unityVersion = "2022.3.15f1",
            lastUpdate = "28.02.2024",
            name = "com.tiogiras.megapint-notepad",
            displayName = "NotePad",
            description =
                "You want to tell your team or remember what a certain MonoBehaviour does?\n\nNotePad allows you to take notes directly on the GameObject. Create simple \"How to's\" or note something todo right where you need it.",
            repository = "https://github.com/tiogiras/MegaPint-NotePad.git"
        };
    }

    #endregion
}

}
#endif
