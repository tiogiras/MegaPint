#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Packages.Data
{

/// <summary> Data for the NotePad package </summary>
internal static class PackageDataBehaviourInfo
{
    #region Public Methods

    /// <summary> Get the data for this package </summary>
    /// <returns> Corresponding <see cref="PackageData" /> </returns>
    public static PackageData Get()
    {
        return new PackageData
        {
            key = PackageKey.BehaviourInfo,
            reqMpVersion = "1.3.0 or higher",
            version = "1.0.0",
            unityVersion = "2022.3.15f1",
            lastUpdate = "",
            name = "com.tiogiras.megapint-behaviourinfo",
            displayName = "Behaviour Info",
            description = "",
            repository = "https://github.com/tiogiras/MegaPint-BehaviourInfo.git"
        };
    }

    #endregion
}

}
#endif
