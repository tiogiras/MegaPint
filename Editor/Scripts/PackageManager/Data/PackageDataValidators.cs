#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Data
{

public static class PackageDataValidators
{
    #region Public Methods

    public static MegaPintPackagesData.MegaPintPackageData Get()
    {
        return new MegaPintPackagesData.MegaPintPackageData
        {
            packageKey = MegaPintPackagesData.PackageKey.Validators,
            packageName = "com.tiogiras.megapint-validators",
            packageNiceName = "Validators",
            gitUrl = "https://github.com/tiogiras/MegaPint-Validators.git#v1.0.0",
            version = "1.0.0",
            lastUpdate = "10.01.2024",
            unityVersion = "2022.3.15f1 or higher",
            megaPintVersion = "1.0.0 or higher",
            infoText = "Validators adds a system to create validatable MonoBehaviours, which can be extended with your own requirements.\n" +
                       "See the status and occured issues on all GameObjects with validatable MonoBehaviours in one window and automatically fix any occuring issue."
        };
    }

    #endregion
}

}
#endif
