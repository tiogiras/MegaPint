﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.PackageManager.Utility;
using UnityEditor.PackageManager;

namespace MegaPint.Editor.Scripts.PackageManager.Cache
{

/// <summary>
///     Stores combined data from <see cref="UnityEditor.PackageManager.PackageInfo" /> and
///     <see cref="PackageData" />
/// </summary>
internal class CachedPackage : IComparable <CachedPackage>
{
    /// <summary> <see cref="PackageKey" /> of the package </summary>
    public PackageKey Key {get;}

    /// <summary> Name of the package </summary>
    public string Name {get;}

    /// <summary> All samples of the package </summary>
    public List <SampleData> Samples {get;}

    /// <summary> If the package has samples to import </summary>
    public bool HasSamples {get;}

    /// <summary> Name of the package used when displayed </summary>
    public string DisplayName {get;}

    /// <summary> Last date the package was edited </summary>
    public string LastUpdate {get;}

    /// <summary> Description of the package </summary>
    public string Description {get;}

    /// <summary> Minimal required MegaPint BasePackage version for this package </summary>
    public string ReqMpVersion {get;}

    /// <summary> Newest(should be) version of this package </summary>
    public string Version {get;}

    /// <summary> Minimal required unity version for this package </summary>
    public string UnityVersion {get;}

    /// <summary> Current version of this package </summary>
    public string CurrentVersion {get;}

    /// <summary> Git url directing to the hosted gitHub repo of this package </summary>
    public string Repository {get;}

    /// <summary> If the package is currently installed </summary>
    public bool IsInstalled {get;}

    /// <summary> If the package is up to date </summary>
    public bool IsNewestVersion {get;}

    /// <summary> All variations available for this package </summary>
    public List <CachedVariation> Variations {get; private set;}

    /// <summary> The currently active variation of this package </summary>
    public CachedVariation CurrentVariation {get;}

    /// <summary> Hash of the current active variation </summary>
    public string CurrentVariationHash {get; private set;}

    /// <summary> All dependencies that this package has </summary>
    public List <Dependency> Dependencies {get; private set;}

    public List <string> Images {get; private set;}

    private List <PackageKey> _myDependants;

    /// <summary>
    ///     Create a new CachedPackage from the corresponding <see cref="UnityEditor.PackageManager.PackageInfo" /> and
    ///     <see cref="PackageData" />
    /// </summary>
    /// <param name="packageData"> Corresponding <see cref="PackageData" /> </param>
    /// <param name="packageInfo"> Corresponding <see cref="UnityEditor.PackageManager.PackageInfo" /> </param>
    /// <param name="dependencies"> All dependencies this package has </param>
    public CachedPackage(PackageData packageData, PackageInfo packageInfo, out List <Dependency> dependencies)
    {
        // PackageData Information
        Key = packageData.key;
        Name = packageData.name;
        DisplayName = packageData.displayName;
        Samples = packageData.samples;
        Description = packageData.description;
        LastUpdate = packageData.lastUpdate;
        ReqMpVersion = packageData.reqMpVersion;
        Version = packageData.version;
        UnityVersion = packageData.unityVersion;
        Repository = packageData.repository;
        Images = packageData.images;

        // PackageInfo Information
        IsInstalled = packageInfo != null;

        dependencies = null;

        if (packageData.dependencies is {Count: > 0})
            dependencies = packageData.dependencies;

        SetVariations(packageData, packageInfo, out Variation installedVariation);

        if (installedVariation == null)
        {
            if (packageData.dependencies is {Count: > 0})
                dependencies = packageData.dependencies;
        }
        else
        {
            CurrentVariation = PackageManagerUtility.VariationToCache(installedVariation, CurrentVersion, Repository);

            if (installedVariation.dependencies is {Count: > 0})
                dependencies = installedVariation.dependencies;
        }

        Dependencies = dependencies;

        if (!IsInstalled)
            return;

        CurrentVersion = packageInfo!.version;
        IsNewestVersion = packageInfo.version == packageData.version;
        HasSamples = Samples is {Count: > 0};
    }

    #region Public Methods

    /// <summary> Compare method against another <see cref="CachedPackage" /> </summary>
    /// <param name="other"> Compare against </param>
    /// <returns> Compare int value </returns>
    public int CompareTo(CachedPackage other)
    {
        if (ReferenceEquals(this, other))
            return 0;

        return ReferenceEquals(null, other)
            ? 1
            : string.Compare(DisplayName, other.DisplayName, StringComparison.Ordinal);
    }

    /// <summary> If the package can be removed or if packages are dependant on it </summary>
    /// <param name="dependencies"> Dependencies that point to this package </param>
    /// <returns> True when no dependencies point to this package </returns>
    public bool CanBeRemoved(out List <PackageKey> dependencies)
    {
        dependencies = null;

        if (_myDependants is not {Count: > 0})
            return true;

        dependencies = _myDependants.Where(PackageCache.IsInstalled).ToList();

        return dependencies is not {Count: > 0};
    }

    /// <summary> Register dependencies that point to this package </summary>
    /// <param name="dependencies"> Dependencies to register </param>
    public void RegisterDependencies(List <PackageKey> dependencies)
    {
        _myDependants = dependencies;
    }

    #endregion

    #region Private Methods

    /// <summary> Set the variations of a package </summary>
    /// <param name="packageData"> Targeted packageData </param>
    /// <param name="packageInfo"> Targeted packageInfo </param>
    /// <param name="installedVariation"> Currently installed variation </param>
    private void SetVariations(
        PackageData packageData,
        PackageInfo packageInfo,
        out Variation installedVariation)
    {
        installedVariation = null;

        if (packageData.variations is not {Count: > 0})
            return;

        Variations = packageData.variations.Select(
                                     variation => PackageManagerUtility.VariationToCache(
                                         variation,
                                         CurrentVersion,
                                         Repository)).
                                 ToList();

        if (packageInfo == null)
            return;

        CurrentVariationHash = "";

        var commitHash = packageInfo.git?.hash;
        var branch = packageInfo.git?.revision;

        for (var i = 0; i < packageData.variations.Count; i++)
        {
            Variation variation = packageData.variations[i];
            var hash = PackageManagerUtility.GetVariationHash(Variations[i]);

            if (hash.Equals(commitHash))
            {
                CurrentVariationHash = commitHash;
                installedVariation = variation;

                break;
            }

            if (!hash.Equals(branch))
                continue;

            CurrentVariationHash = branch;
            installedVariation = variation;

            break;
        }
    }

    #endregion
}

}
#endif
