#if UNITY_EDITOR
using System.Linq;
using System.Text;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.PackageManager.Packages;
using UnityEngine.UIElements;

namespace Editor.Scripts.PackageManager.Utility
{

/// <summary> Utility class for the internal MegaPint packageManager </summary>
internal static class PackageManagerUtility
{
    #region Public Methods

    /// <summary> Get the package url for a <see cref="CachedPackage" /> </summary>
    /// <param name="package"> Targeted package </param>
    /// <returns> Git url pointing to the targeted <see cref="CachedPackage" /> </returns>
    public static string GetPackageUrl(CachedPackage package)
    {
        return $"{package.Repository}#{GetPackageHash(package)}";
    }

    /// <summary> Get the package url for a <see cref="CachedVariation" /> </summary>
    /// <param name="variation"> Targeted variation </param>
    /// <returns> Git url pointing to the targeted <see cref="CachedVariation" /> </returns>
    public static string GetPackageUrl(CachedVariation variation)
    {
        return $"{variation.repository}#{GetVariationHash(variation)}";
    }

    /// <summary> Get the variation hash for a <see cref="CachedVariation" /> </summary>
    /// <param name="variation"> Targeted variation </param>
    /// <param name="invert"> Inverts the devMode value </param>
    /// <returns> Hash of the targeted <see cref="CachedVariation" /> </returns>
    public static string GetVariationHash(CachedVariation variation, bool invert = false)
    {
        var devMode = invert ? !SaveData.DevMode() : SaveData.DevMode();

        return devMode ? variation.devBranch : $"v{variation.version}{variation.tag}";
    }

    /// <summary> Update a <see cref="Label" /> while packages are loading </summary>
    /// <param name="loadingLabel"> <see cref="Label" /> to be updated </param>
    /// <param name="currentProgress"> Current progress of the label </param>
    /// <param name="refreshRate"> RefreshRate of the update </param>
    /// <param name="newProgress"> The new progress of the <see cref="Label" /></param>
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

    /// <summary> Convert a <see cref="Variation" /> into a <see cref="CachedVariation" /> </summary>
    /// <param name="variation"> <see cref="Variation" /> to be converted </param>
    /// <param name="currentVersion"> Current version of the package </param>
    /// <param name="repository"> Repository of the package </param>
    /// <returns> Newly created <see cref="CachedVariation" /> </returns>
    public static CachedVariation VariationToCache(Variation variation, string currentVersion, string repository)
    {
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

    #endregion

    #region Private Methods

    private static string GetPackageHash(CachedPackage package)
    {
        return SaveData.DevMode() ? "development" : $"v{package.Version}";
    }

    #endregion
}

}
#endif
