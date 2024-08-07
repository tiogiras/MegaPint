﻿#if UNITY_EDITOR
using System.Collections.Generic;

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
            reqMpVersion = "1.3.0 or higher",
            version = "1.0.3",
            unityVersion = "2022.3.15f1",
            lastUpdate = "03.08.2024",
            name = "com.tiogiras.megapint-notepad",
            displayName = "NotePad",
            description =
                "You want to tell your team or remember what a certain MonoBehaviour does?\n\nNotePad allows you to take notes directly on the GameObject. Create simple \"How to's\" or note something todo right where you need it.",
            repository = "https://github.com/tiogiras/MegaPint-NotePad.git",
            samples = new List <SampleData> {new() {displayName = "Basics", path = "Basics"}}
        };
    }

    #endregion
}

}
#endif
