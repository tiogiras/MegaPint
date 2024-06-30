#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.PackageManager.Packages.Data;

namespace MegaPint.Editor.Scripts.PackageManager.Cache
{

/// <summary> Stores references to all defined <see cref="Editor.Scripts.PackageManager.Packages.PackageData" /> </summary>
internal static class DataCache
{
    private static readonly Dictionary <PackageKey, PackageData> s_data = new()
    {
        {PackageKey.AutoSave, PackageDataAutoSave.Get()},
        {PackageKey.Validators, PackageDataValidators.Get()},
        {PackageKey.AlphaButton, PackageDataAlphaButton.Get()},
        {PackageKey.BATesting, PackageDataBaTesting.Get()},
        {PackageKey.PlayModeStartScene, PackageDataPlayModeStartScene.Get()},
        {PackageKey.Screenshot, PackageDataScreenshot.Get()},
        {PackageKey.NotePad, PackageDataNotePad.Get()}
    };

    /// <summary> Development branch of the base package </summary>
    public static string BasePackageDevBranch => "development";

    /// <summary> Name of the base package </summary>
    public static string BasePackageName => "com.tiogiras.megapint";

    /// <summary> Get all defined megaPint packages </summary>
    public static PackageData[] AllPackages => s_data.Values.ToArray();

    #region Public Methods

    /// <summary>
    ///     Get the corresponding <see cref="Editor.Scripts.PackageManager.Packages.PackageData" /> to the
    ///     <see cref="PackageKey" />
    /// </summary>
    /// <param name="key"> Key of the wanted package </param>
    /// <returns> Corresponding <see cref="Editor.Scripts.PackageManager.Packages.PackageData" /> </returns>
    public static PackageData PackageData(PackageKey key)
    {
        return s_data[key];
    }

    /// <summary> Get the corresponding <see cref="Editor.Scripts.PackageManager.Packages.PackageData" /> to the name </summary>
    /// <param name="packageName"> Name of the required package </param>
    /// <returns> Corresponding <see cref="Editor.Scripts.PackageManager.Packages.PackageData" /> </returns>
    public static PackageData PackageData(string packageName)
    {
        return s_data.Values.FirstOrDefault(package => package.name.Equals(packageName));
    }

    #endregion
}

}
#endif
