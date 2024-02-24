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
            reqMpVersion = "1.1.1 or higher",
            version = "1.0.1",
            unityVersion = "2022.3.15f1",
            lastUpdate = "24.02.2024",
            name = "com.tiogiras.megapint-notepad",
            displayName = "NotePad",
            description = "NotePad allows you to make and edit notes directly on a GameObjects MonoBehaviour.",
            repository = "https://github.com/tiogiras/MegaPint-NotePad.git"
        };
    }

    #endregion
}

}
#endif
