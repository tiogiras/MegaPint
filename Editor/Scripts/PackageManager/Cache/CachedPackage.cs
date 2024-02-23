using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.PackageManager.Packages;
using Editor.Scripts.PackageManager.Utility;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Editor.Scripts.PackageManager.Cache
{

internal class CachedPackage : IComparable <CachedPackage>
{
    public PackageKey Key {get;}
    public string Name {get;}
    public string DisplayName {get;}
    public string LastUpdate {get;}
    public string Description {get;}
    public string ReqMpVersion {get;}
    public string Version {get;}
    public string UnityVersion {get;}
    public string CurrentVersion {get;}
    public string Repository {get;}
    public bool IsInstalled {get;}
    public bool IsNewestVersion {get;}
    public List <CachedVariation> Variations {get; private set;}
    public CachedVariation CurrentVariation {get;}
    public string CurrentVariationHash {get; private set;}
    public List <Dependency> Dependencies {get; private set;}

    private List <Dependency> _myDependants;

    public CachedPackage(PackageData packageData, PackageInfo packageInfo, out List <Dependency> dependencies)
    {
        // PackageData data
        Key = packageData.key;
        Name = packageData.name;
        DisplayName = packageData.displayName;
        Description = packageData.description;
        LastUpdate = packageData.lastUpdate;
        ReqMpVersion = packageData.reqMpVersion;
        Version = packageData.version;
        UnityVersion = packageData.unityVersion;
        Repository = packageData.repository;
        
        IsInstalled = packageInfo != null;

        dependencies = null;
        
        SetVariations(packageData, packageInfo, out Variation installedVariation);
        
        if (!IsInstalled)
            return;

        CurrentVersion = packageInfo!.version;
        IsNewestVersion = packageInfo.version == packageData.version; // TODO Update with branch and dev stuff
        
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

    public void RegisterDependencies(List<Dependency> dependencies)
    {
        _myDependants = dependencies;
    }

    public bool CanBeRemoved(out List <Dependency> dependencies)
    {
        Debug.Log($"Deps: {_myDependants.Count}");

        dependencies = _myDependants;
        return _myDependants is not {Count: > 0};
    }

    public int CompareTo(CachedPackage other)
    {
        if (ReferenceEquals(this, other))
            return 0;

        return ReferenceEquals(null, other) ? 1 : string.Compare(DisplayName, other.DisplayName, StringComparison.Ordinal);
    }
}

}
