#if UNITY_EDITOR
using System.Collections.Generic;

namespace MegaPint.Editor.Scripts.PackageManager.Packages
{

/// <summary> Stores data about a megaPint package that is set before distribution </summary>
internal class PackageData
{
    /// <summary> All megaPint package dependencies of this variation </summary>
    public List <Dependency> dependencies;

    /// <summary> Display attribute of the corresponding package.json </summary>
    public string description;

    /// <summary> Displayed name attribute of the corresponding package.json </summary>
    public string displayName;

    public List <string> images;

    /// <summary> Key for internal references </summary>
    public PackageKey key;

    /// <summary> Date of the last update of the package </summary>
    public string lastUpdate;

    /// <summary> Name attribute of the corresponding package.json for referencing </summary>
    public string name;

    public string repository;

    /// <summary> Required megaPint basePackage version </summary>
    public string reqMpVersion;

    /// <summary> Sample unityPackages of the package </summary>
    public List <SampleData> samples;

    /// <summary> UnityVersion attribute of the corresponding package.json </summary>
    public string unityVersion;

    /// <summary> All variations of this package </summary>
    public List <Variation> variations;

    /// <summary> Version attribute of the corresponding package.json </summary>
    public string version;
}

}
#endif
