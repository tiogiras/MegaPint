using System.Collections.Generic;
using Editor.Scripts.PackageManager.Packages;
using UnityEditor.PackageManager;

namespace Editor.Scripts.PackageManager.Cache
{

internal class CachedPackage
{
    public PackageKey key;
    
    public string name;
    public string displayName;
    public string description;
    
    public string version;
    public string reqMpVersion;

    public string repository;
    
    public bool IsInstalled {get;}
    public bool IsNewestVersion {get;}

    public List <CachedVariation> variations;
    public string currentVariation;

    public CachedPackage(PackageData packageData, PackageInfo packageInfo)
    {
        key = packageData.key;

        name = packageData.name;
        displayName = packageInfo.displayName;
        description = packageInfo.description;

        version = packageInfo.version;
        reqMpVersion = packageData.reqMpVersion;

        repository = packageInfo.repository.url;
    }
    
    
}

}
