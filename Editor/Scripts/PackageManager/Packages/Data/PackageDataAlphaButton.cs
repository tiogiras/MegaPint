#if UNITY_EDITOR
using System.Collections.Generic;

namespace MegaPint.Editor.Scripts.PackageManager.Packages.Data
{

/// <summary> Data for the AlphaButton package </summary>
internal static class PackageDataAlphaButton
{
    #region Public Methods

    /// <summary> Get the data for this package </summary>
    /// <returns> Corresponding <see cref="PackageData" /> </returns>
    public static PackageData Get()
    {
        return new PackageData
        {
            key = PackageKey.AlphaButton,
            reqMpVersion = "1.1.1 or higher",
            version = "1.0.1",
            unityVersion = "2022.3.15f1",
            lastUpdate = "24.02.2024",
            name = "com.tiogiras.megapint-alphabutton",
            displayName = "AlphaButton",
            description =
                "<b>STOP</b> with simple rectangular buttons!\n\nAlphaButton is an improved Button component that allows you to utilize the alpha of your sprites to make the button only clickable when the mouse is over the actual graphic.",
            repository = "https://github.com/tiogiras/MegaPint-AlphaButton.git",
            variations = new List <Variation>
            {
                new()
                {
                    name = "MegaPint Validators Integration",
                    version = "1.0.0",
                    tag = "a",
                    devBranch = "validatorsIntegration/development",
                    dependencies = new List <Dependency>
                    {
                        new() {name = "Validators", key = PackageKey.Validators}
                    }
                }
            }
        };
    }

    #endregion
}

}
#endif
