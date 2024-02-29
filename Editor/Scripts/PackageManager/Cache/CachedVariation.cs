#if UNITY_EDITOR
using System.Collections.Generic;
using Editor.Scripts.PackageManager.Packages;

namespace Editor.Scripts.PackageManager.Cache
{

/// <summary> Stores references to values of a <see cref="Variation" /> </summary>
internal class CachedVariation
{
    /// <summary> Dependencies that the variation has on other packages </summary>
    public List <Dependency> dependencies;

    /// <summary> DevelopmentBranch of the variation </summary>
    public string devBranch;

    /// <summary> If the variation is up to date </summary>
    public bool isNewestVersion;

    /// <summary> Name of the variation </summary>
    public string name;

    /// <summary> Git url directing to the hosted gitHub repo of this variation </summary>
    public string repository;

    /// <summary> Variation specific tag to identify the git tag </summary>
    public string tag;

    /// <summary> Newest(should be) version of the variation </summary>
    public string version;
}

}
#endif
