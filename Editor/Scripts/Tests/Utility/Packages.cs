#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.PackageManager;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using NUnit.Framework;

namespace MegaPint.Editor.Scripts.Tests.Utility
{

/// <summary> Partial utility class for unit testing </summary>
internal static partial class TestsUtility
{
    private static bool s_packageManagerResult;

    #region Public Methods

    /// <summary> Import a package and test if the import was successful </summary>
    /// <param name="key"> Key to the targeted package </param>
    /// <returns> If the package was imported correctly </returns>
    public static async Task <bool> ImportPackage(PackageKey key)
    {
        s_packageManagerResult = false;

        MegaPintPackageManager.onSuccess += OnSuccess;

        await MegaPintPackageManager.AddEmbedded(PackageCache.Get(key));

        MegaPintPackageManager.onSuccess -= OnSuccess;

        return s_packageManagerResult;
    }

    /// <summary> Import a package variation and test if the import was successful </summary>
    /// <param name="key"> Key to the targeted package </param>
    /// <param name="variationIndex"> Index of the targeted variation </param>
    /// <returns> If the variation was imported correctly </returns>
    public static async Task <bool> ImportVariation(PackageKey key, int variationIndex)
    {
        s_packageManagerResult = false;

        MegaPintPackageManager.onSuccess += OnSuccess;

        await MegaPintPackageManager.AddEmbedded(PackageCache.Get(key).Variations[variationIndex]);

        MegaPintPackageManager.onSuccess -= OnSuccess;

        return s_packageManagerResult;
    }

    /// <summary> Remove a package and test if the removing was successful </summary>
    /// <param name="key"> Key to the targeted package </param>
    /// <returns> If the package was removed correctly </returns>
    public static async Task <bool> RemovePackage(PackageKey key)
    {
        s_packageManagerResult = false;

        MegaPintPackageManager.onSuccess += OnSuccess;

        await MegaPintPackageManager.Remove(PackageCache.Get(key).Name);

        MegaPintPackageManager.onSuccess -= OnSuccess;

        return s_packageManagerResult;
    }

    /// <summary> Pass the unit test when the current unity project is the production project </summary>
    public static void SkipIfProductionProject()
    {
        if (Scripts.Utility.IsProductionProject())
            Assert.Pass("SKIPPED ===> Production project!");
    }

    /// <summary> Validate that all dependencies of the package cannot be removed </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="key"> Key to the targeted package </param>
    public static void ValidatePackageDependencies(ref bool isValid, PackageKey key)
    {
        foreach (Dependency dependency in PackageCache.Get(key).Dependencies)
        {
            Validate(
                ref isValid,
                PackageCache.Get(dependency.key).CanBeRemoved(out List <PackageKey> _),
                $"Could remove {dependency.name} but it should not be removable due to dependencies!");
        }
    }

    #endregion

    #region Private Methods

    /// <summary> Callback on successful <see cref="MegaPintPackageManager" /> action </summary>
    private static void OnSuccess()
    {
        s_packageManagerResult = true;
    }

    #endregion
}

}
#endif
#endif
