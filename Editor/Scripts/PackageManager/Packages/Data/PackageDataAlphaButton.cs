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
            reqMpVersion = "1.1.0 or higher",
            version = "1.0.0",
            unityVersion = "2022.3.15f1",
            lastUpdate = "22.02.2024",
            name = "com.tiogiras.megapint-alphabutton",
            displayName = "AlphaButton",
            description = "TODO",
            
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
