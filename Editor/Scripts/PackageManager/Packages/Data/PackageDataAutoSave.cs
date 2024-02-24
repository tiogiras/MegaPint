#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Packages.Data
{

internal static class PackageDataAutoSave
{
    #region Public Methods

    public static PackageData Get()
    {
        return new PackageData
        {
            key = PackageKey.AutoSave, 
            reqMpVersion = "1.1.1 or higher",
            version = "1.0.2",
            unityVersion = "2022.3.15f1",
            lastUpdate = "22.02.2024",
            name = "com.tiogiras.megapint-autosave",
            displayName = "Scene-AutoSave",
            description = "This package allows you to enable an autosaving feature for scenes.",
            repository = "https://github.com/tiogiras/MegaPint-AutoSave.git"
        };
    }

    #endregion
}

}
#endif
