#if UNITY_EDITOR
using System.Collections.Generic;

namespace MegaPint.Editor.Scripts.PackageManager.Packages.Data
{

/// <summary> Data for the AlphaButton package </summary>
internal static class PackageDataBaTesting
{
    #region Public Methods

    /// <summary> Get the data for this package </summary>
    /// <returns> Corresponding <see cref="PackageData" /> </returns>
    public static PackageData Get()
    {
        return new PackageData
        {
            key = PackageKey.BATesting,
            reqMpVersion = "1.3.0 or higher",
            version = "1.0.0",
            unityVersion = "2022.3.15f1",
            lastUpdate = "24.06.2024",
            name = "com.tiogiras.megapint-batesting",
            displayName = "BA Testing",
            description = "This package is required to perform the guided part of the ba testing tasks.",
            repository = "https://github.com/tiogiras/MegaPint-BATesting.git",
            dependencies = new List <Dependency>
            {
                new() {name = "AutoSave", key = PackageKey.AutoSave},
                new() {name = "PlayMode StartScene", key = PackageKey.PlayModeStartScene},
                new() {name = "Screenshot", key = PackageKey.Screenshot},
                new() {name = "Validators", key = PackageKey.Validators}
            }
        };
    }

    #endregion
}

}
#endif
