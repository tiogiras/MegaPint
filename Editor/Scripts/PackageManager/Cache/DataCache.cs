using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.PackageManager.Packages;
using Editor.Scripts.PackageManager.Packages.Data;

namespace Editor.Scripts.PackageManager.Cache
{

/// <summary> Stores references to all defined <see cref="PackageData" /> </summary>
internal static class DataCache
{
    private static readonly Dictionary <PackageKey, PackageData> s_data = new()
    {
        {PackageKey.AutoSave, PackageDataAutoSave.Get()},
        {PackageKey.Validators, PackageDataValidators.Get()},
        {PackageKey.AlphaButton, PackageDataAlphaButton.Get()},
        {PackageKey.PlayModeStartScene, PackageDataPlayModeStartScene.Get()},
        {PackageKey.NotePad, PackageDataNotePad.Get()}
    };

    /// <summary> Development branch of the base package </summary>
    public static string BasePackageDevBranch {get;} = "development";

    /// <summary> Name of the base package </summary>
    public static string BasePackageName {get;} = "com.tiogiras.megapint";

    /// <summary> Get all defined megaPint packages </summary>
    public static IEnumerable <PackageData> AllPackages => s_data.Values.ToList();

    #region Public Methods

    /// <summary> Get the corresponding <see cref="PackageData" /> to the <see cref="PackageKey" /> </summary>
    /// <param name="key"> Key of the wanted package </param>
    /// <returns> Corresponding <see cref="PackageData" /> </returns>
    public static PackageData PackageData(PackageKey key)
    {
        return s_data[key];
    }
    
    public static PackageData PackageData(string packageName)
    {
        return s_data.Values.FirstOrDefault(package => package.name.Equals(packageName));
    }

    #endregion
}

}
