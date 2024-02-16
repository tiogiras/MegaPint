using System;
using System.Collections.Generic;

namespace Editor.Scripts.PackageManager.Packages
{

/// <summary> Stores data about a megaPint package variation that is set before distribution </summary>
internal class Variation : IComparable <Variation>
{
    /// <summary> DisplayName of this variation </summary>
    public string name;
    
    /// <summary> Version of this variation </summary>
    public string version;
    
    /// <summary> Tag used to differentiate the main branch and the variation branch tags </summary>
    public string tag;

    /// <summary> Branch used for development </summary>
    public string devBranch;

    /// <summary> All megaPint package dependencies of this variation </summary>
    public List <Dependency> dependencies;

    #region Public Methods

    public int CompareTo(Variation other)
    {
        return string.Compare(name, other.name, StringComparison.Ordinal);
    }

    #endregion
}

}
