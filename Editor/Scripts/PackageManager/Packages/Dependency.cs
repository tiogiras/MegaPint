using System;

namespace Editor.Scripts.PackageManager.Packages
{

/// <summary> Stores data about a megaPint package dependency that is set before distribution </summary>
internal class Dependency : IComparable <Dependency>
{
    /// <summary> Key for internal references </summary>
    public MegaPintPackagesData.PackageKey key;
    
    /// <summary> Display name of this dependency </summary>
    public string name;

    #region Public Methods

    public int CompareTo(Dependency other)
    {
        return string.Compare(name, other.name, StringComparison.Ordinal);
    }

    #endregion
}

}
