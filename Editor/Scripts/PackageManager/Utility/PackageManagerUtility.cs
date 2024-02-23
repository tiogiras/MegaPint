using System.Linq;
using System.Text;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.PackageManager.Packages;
using Editor.Scripts.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.PackageManager.Utility
{

internal static class PackageManagerUtility
{
    private static MegaPintSettingsBase s_settings;

    private static MegaPintSettingsBase Settings() => s_settings ??= MegaPintSettings.instance.GetSetting("General");

    private static bool DevMode() => Settings().GetValue("devMode", false);
    
    public static string GetPackageHash(CachedPackage package)
    {
        return DevMode() ? "development" : $"v{package.Version}";
    }
    
    public static string GetPackageUrl(CachedPackage package)
    {
        return $"{package.Repository}#{GetPackageHash(package)}";
    }

    public static string GetVariationHash(CachedVariation variation, bool invert = false)
    {
        var devMode = invert ? !DevMode() : DevMode();
        return devMode ? variation.devBranch : $"v{variation.version}{variation.tag}";
    }

    public static string GetPackageUrl(CachedVariation variation)
    {
        return $"{variation.repository}#{GetVariationHash(variation)}";
    }

    public static CachedVariation VariationToCache(Variation variation, string currentVersion, string repository)
    {
        Debug.Log($"{variation.version} | {currentVersion}");
        
        return new CachedVariation
        {
            devBranch = variation.devBranch,
            isNewestVersion = variation.version.Equals(currentVersion),
            name = variation.name,
            repository = repository,
            tag = variation.tag,
            version = variation.version,
            dependencies = variation.dependencies
        };
    }
    
    public static void UpdateLoadingLabel(Label loadingLabel, int currentProgress, int refreshRate, out int newProgress)
    {
        if (currentProgress >= refreshRate)
        {
            currentProgress = 0;

            var pointCount = loadingLabel.text.Count(c => c == '.');
            var loadingText = new StringBuilder("Loading");

            if (pointCount == 3)
                pointCount = 0;

            for (var i = 0; i < pointCount + 1; i++)
                loadingText.Append(".");

            loadingLabel.style.display = DisplayStyle.Flex;
            loadingLabel.text = loadingText.ToString();
        }
        else
            currentProgress += MegaPintPackageManager.RefreshRate;

        newProgress = currentProgress;
    }
}

}
