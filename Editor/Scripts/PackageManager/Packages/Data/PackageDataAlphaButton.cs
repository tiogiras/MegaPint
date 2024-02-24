#if UNITY_EDITOR
using System.Collections.Generic;

namespace Editor.Scripts.PackageManager.Packages.Data
{

internal static class PackageDataAlphaButton
{
    #region Public Methods

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
            description = "This package adds a new component based on the normal button component that is only clickable where the alpha is greater than a certain threshold.",
            repository = "https://github.com/tiogiras/MegaPint-AlphaButton.git",

            variations = new List <Variation>
            {
                new()
                {
                    name = "MegaPint Validators Integration",
                    version = "1.0.0",
                    
                    tag = "a",
                    devBranch = "validatorsIntegration/development",
                    
                    dependencies = new List <Dependency> {
                        new()
                        {
                            name = "Validators", 
                            key = PackageKey.Validators
                        }}
                }
            }
        };
    }

    #endregion
}

}
#endif
