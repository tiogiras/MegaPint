#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.PackageManager.Packages;
using Editor.Scripts.PackageManager.Packages.Data;

namespace Editor.Scripts.PackageManager
{

internal static class MegaPintPackagesData
{
    internal enum PackageKey
    {
        AutoSave, Validators, AlphaButton, PlayModeStartScene, NotePad
    }

    internal static readonly List <PackageData> Packages = new()
    {
        PackageDataAutoSave.Get(),
        PackageDataValidators.Get(),
        PackageDataAlphaButton.Get(),
        PackageDataPlayModeStartScene.Get(),
        PackageDataNotePad.Get()
    };
    
    public static string BasePackageDevURL => "https://github.com/tiogiras/MegaPint.git#development";

    #region Public Methods

    public static MegaPintPackageData PackageData(PackageKey packageKey)
    {
        return Packages.First(package => package.key == packageKey);
    }

    #endregion
}

}
#endif
