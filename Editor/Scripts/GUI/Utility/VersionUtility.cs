#if UNITY_EDITOR
using System.Collections.Generic;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;

namespace MegaPint.Editor.Scripts.GUI.Utility
{
internal static class VersionUtility
{
    public static Dictionary <PackageKey, string> Prefixes = new()
    {
        {PackageKey.Undefined, ""},
        {PackageKey.AutoSave, "mp-save"},
        {PackageKey.Validators, "mp-val"},
        {PackageKey.AlphaButton, "mp-ab"},
        {PackageKey.PlayModeStartScene, "mp-pms"},
        {PackageKey.NotePad, "mp-note"},
        {PackageKey.Screenshot, "mp-scr"},
        {PackageKey.BATesting, ""}
    };
    
    public static string GetPrefix(PackageKey package)
    {
        return Prefixes[package];
    }
    
    public static string GetPrefixByDisplayName(string displayName)
    {
        if (displayName.ToLower().Equals("megapint - basepackage"))
            return "mp";
        
        return Prefixes[PackageCache.Get(displayName)];
    }
    
    public static string GetCurrentVersionByDisplayName(string displayName)
    {
        if (displayName.ToLower().Equals("megapint - basepackage"))
            return PackageCache.BasePackage.version;
        
        return PackageCache.CurrentVersion(PackageCache.Get(displayName));
    }
}
}

#endif