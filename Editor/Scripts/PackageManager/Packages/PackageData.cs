#if UNITY_EDITOR
using System.Collections.Generic;

namespace Editor.Scripts.PackageManager.Packages
{

/// <summary> Stores data about a megaPint package that is set before distribution </summary>
internal class PackageData
{
    /// <summary> Key for internal references </summary>
    public PackageKey key;
    
    /// <summary> Required megaPint basePackage version </summary>
    public string reqMpVersion;

    /// <summary> Version attribute of the corresponding package.json </summary>
    public string version;
    
    /// <summary> UnityVersion attribute of the corresponding package.json </summary>
    public string unityVersion;
    
    /// <summary> Date of the last update of the package </summary>
    public string lastUpdate;
    
    /// <summary> Name attribute of the corresponding package.json for referencing </summary>
    public string name;

    /// <summary> Displayed name attribute of the corresponding package.json </summary>
    public string displayName;

    /// <summary> Display attribute of the corresponding package.json </summary>
    public string description;

    /// <summary> All megaPint package dependencies of this variation </summary>
    public List <Dependency> dependencies;
    
    /// <summary> All variations of this package </summary>
    public List <Variation> variations;
}

}
#endif
