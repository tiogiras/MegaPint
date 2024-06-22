#if UNITY_EDITOR
using System.Collections.Generic;

namespace MegaPint.Editor.Scripts.PackageManager.Packages.Data
{

/// <summary> Data for the Validators package </summary>
internal static class PackageDataValidators
{
    #region Public Methods

    /// <summary> Get the data for this package </summary>
    /// <returns> Corresponding <see cref="PackageData" /> </returns>
    public static PackageData Get()
    {
        return new PackageData
        {
            key = PackageKey.Validators,
            reqMpVersion = "1.2.2 or higher",
            version = "1.0.3",
            unityVersion = "2022.3.15f1",
            lastUpdate = "22.06.2024",
            name = "com.tiogiras.megapint-validators",
            displayName = "Validators",
            description =
                "You don't want to recheck all settings for all items or GameObjects?\n\nWith the Validators package you can create custom validators that display the status of your GameObject. See all issues in a scene or your complete project in one window.\n\nWhile you can select from a predefined collection of requirements to create your validations you can also create your own requirements to completely customize the validators to your needs.",
            repository = "https://github.com/tiogiras/MegaPint-Validators.git",
            samples = new List <SampleData> {new() {displayName = "Basics", path = "Basics"}}
        };
    }

    #endregion
}

}
#endif
