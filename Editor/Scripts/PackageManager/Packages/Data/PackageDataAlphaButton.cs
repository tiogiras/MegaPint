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
            name = "com.tiogiras.megapint-alphabutton",
            
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
