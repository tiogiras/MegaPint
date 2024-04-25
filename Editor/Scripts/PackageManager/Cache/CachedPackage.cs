#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.PackageManager.Packages;
using Editor.Scripts.PackageManager.Utility;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Editor.Scripts.PackageManager.Cache
{

/// <summary> Stores combined data from <see cref="PackageInfo" /> and <see cref="PackageData" /> </summary>
internal class CachedPackage : IComparable <CachedPackage>
{
    /// <summary> <see cref="PackageKey" /> of the package </summary>
    public PackageKey Key {get;}

    /// <summary> Name of the package </summary>
    public string Name {get;}

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

    private List <Dependency> _myDependants;

    /// <summary>
    ///     Create a new CachedPackage from the corresponding <see cref="PackageInfo" /> and <see cref="PackageData" />
    /// </summary>
    /// <param name="packageData"> Corresponding <see cref="PackageData" /> </param>
    /// <param name="packageInfo"> Corresponding <see cref="PackageInfo" /> </param>
    /// <param name="dependencies"> All dependencies this package has </param>
    public CachedPackage(PackageData packageData, PackageInfo packageInfo, out List <Dependency> dependencies)
    {
        // PackageData Information
        Key = packageData.key;
        Name = packageData.name;
        DisplayName = packageData.displayName;
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

        SetVariations(packageData, packageInfo, out Variation installedVariation);

        if (!IsInstalled)
            return;

        CurrentVersion = packageInfo!.version;
        IsNewestVersion = packageInfo.version == packageData.version;

        SetVariations(packageData, packageInfo, out installedVariation);

        if (installedVariation == null)
        {
            if (packageData.dependencies is not {Count: > 0})
                return;

            dependencies = packageData.dependencies;
            Dependencies = packageData.dependencies;
        }
        else
        {
            CurrentVariation = PackageManagerUtility.VariationToCache(installedVariation, CurrentVersion, Repository);

            if (installedVariation.dependencies is {Count: > 0})
                dependencies = installedVariation.dependencies;
        }
    }

    #region Public Methods

    /// <summary> Compare method against another <see cref="CachedPackage" /> </summary>
    /// <param name="other"> Compare against </param>
    /// <returns> Compare int value </returns>
    public int CompareTo(CachedPackage other)
    {
        if (ReferenceEquals(this, other))
            return 0;

        return ReferenceEquals(null, other) ? 1 : string.Compare(DisplayName, other.DisplayName, StringComparison.Ordinal);
    }

    /// <summary> If the package can be removed or if packages are dependant on it </summary>
    /// <param name="dependencies"> Dependencies that point to this package </param>
    /// <returns> True when no dependencies point to this package </returns>
    public bool CanBeRemoved(out List <Dependency> dependencies)
    {
        dependencies = _myDependants;

        return _myDependants is not {Count: > 0};
    }

    /// <summary> Register dependencies that point to this package </summary>
    /// <param name="dependencies"> Dependencies to register </param>
    public void RegisterDependencies(List <Dependency> dependencies)
    {
        Debug.Log($"Registering Dependencies for {Name}");

        foreach (Dependency dependency in dependencies)
        {
            Debug.Log(dependency.name);
        }
        
        _myDependants = dependencies;
    }

    #endregion

    #region Private Methods

    private void SetVariations(
        PackageData packageData,
        PackageInfo packageInfo,
        out Variation installedVariation)
    {
        installedVariation = null;

        if (packageData.variations is not {Count: > 0})
            return;

        Variations = packageData.variations.Select(variation => PackageManagerUtility.VariationToCache(variation, CurrentVersion, Repository)).
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
