#if UNITY_EDITOR
using System.Collections.Generic;

namespace MegaPint.Editor.Scripts.PackageManager.Packages.Data
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
            description =
                "Want to take screenshots during development? \n\nThis package allows you to render your in game camera's as well as editor windows. Setup different camera angles and capture different screenshots at runtime from all angles with one button press.",
            repository = "https://github.com/tiogiras/MegaPint-Screenshot.git",
            samples = new List <SampleData>
            {
                new() {displayName = "Basics", path = "Basics"},
                new() {displayName = "Basics (URP)", path = "Basics (URP)"},
                new() {displayName = "Basics (HDRP)", path = "Basics (HDRP)"}
            }
        };
    }

    #endregion
}

}
#endif
