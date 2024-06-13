using System.Collections.Generic;
using MegaPint.Editor.Scripts.PackageManager.Packages;

#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
namespace MegaPint.Editor.Scripts.Tests.Utility
{

/// <summary> Partial utility class for unit testing </summary>
internal static partial class TestsUtility
{
    #region Public Methods

    /// <summary> Validate the data of the given dependencies </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="dependencies"> Targeted dependencies </param>
    /// <param name="key"> Key of the targeted package </param>
    /// <param name="variation"> Current variation name of the targeted package </param>
    public static void ValidateDependenciesData(
        ref bool isValid,
        List <Dependency> dependencies,
        PackageKey key,
        string variation = "")
    {
        if (dependencies is not {Count: > 0})
            return;

        var variationStr = string.IsNullOrEmpty(variation) ? "" : $"Variation[{variation}]";

        foreach (Dependency dependency in dependencies)
        {
            Validate(
                ref isValid,
                dependency.key == PackageKey.Undefined,
                $"Package[{key}] {variationStr} Dependency is missing PackageKey!");

            Validate(
                ref isValid,
                string.IsNullOrEmpty(dependency.name),
                $"Package[{key}] {variationStr} Dependency[{dependency.key}] is missing a name!");
        }
    }

    /// <summary> Validate the data of a package </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="package"> Targeted package </param>
    public static void ValidatePackageData(ref bool isValid, PackageData package)
    {
        PackageKey key = package.key;

        Validate(ref isValid, key == PackageKey.Undefined, "PackageKey is missing!");

        Validate(
            ref isValid,
            string.IsNullOrEmpty(package.reqMpVersion),
            $"Package[{key}] missing [reqMpVersion]!");

        Validate(
            ref isValid,
            string.IsNullOrEmpty(package.version),
            $"Package[{key}] missing [version]!");

        Validate(
            ref isValid,
            string.IsNullOrEmpty(package.unityVersion),
            $"Package[{key}] missing [unityVersion]!");

        Validate(
            ref isValid,
            string.IsNullOrEmpty(package.lastUpdate),
            $"Package[{key}] missing [lastUpdate]!");

        Validate(ref isValid, string.IsNullOrEmpty(package.name), $"Package[{key}] missing [name]!");

        Validate(
            ref isValid,
            string.IsNullOrEmpty(package.displayName),
            $"Package[{key}] missing [displayName]!");

        Validate(
            ref isValid,
            string.IsNullOrEmpty(package.description),
            $"Package[{key}] missing [description]!");

        Validate(
            ref isValid,
            string.IsNullOrEmpty(package.repository),
            $"Package[{key}] missing [repository]!");
    }

    /// <summary> Validate the data of the given samples </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="samples"> Targeted samples </param>
    /// <param name="key"> Key to the targeted package </param>
    public static void ValidateSamplesData(ref bool isValid, List <SampleData> samples, PackageKey key)
    {
        if (samples is not {Count: > 0})
            return;

        foreach (SampleData sample in samples)
        {
            Validate(
                ref isValid,
                string.IsNullOrEmpty(sample.displayName),
                $"Package[{key}] Sample is missing [displayName]!");

            Validate(
                ref isValid,
                string.IsNullOrEmpty(sample.path),
                $"Package[{key}] Sample is missing [path]!");
        }
    }

    /// <summary> Validate the data of the given variations </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="variations"> Targeted variations </param>
    /// <param name="key"> Key to the targeted package </param>
    public static void ValidateVariationsData(ref bool isValid, List <Variation> variations, PackageKey key)
    {
        if (variations is not {Count: > 0})
            return;

        foreach (Variation variation in variations)
        {
            Validate(
                ref isValid,
                string.IsNullOrEmpty(variation.name),
                $"Package[{key}] Variation is missing a name");

            Validate(
                ref isValid,
                string.IsNullOrEmpty(variation.version),
                $"Package[{key}] Variation[{variation.name}] missing [version]!");

            Validate(
                ref isValid,
                string.IsNullOrEmpty(variation.tag),
                $"Package[{key}] Variation[{variation.name}] missing [tag]!");

            Validate(
                ref isValid,
                string.IsNullOrEmpty(variation.devBranch),
                $"Package[{key}] Variation[{variation.name}] missing [devBranch]!");

            ValidateDependenciesData(ref isValid, variation.dependencies, key, variation.name);
        }
    }

    #endregion

    // TODO commenting
}

}
#endif
#endif
