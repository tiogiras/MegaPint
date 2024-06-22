#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace MegaPint.Editor.Scripts.PackageManager.Packages
{

/// <summary> Stores data about a megaPint package variation that is set before distribution </summary>
internal class Variation : IComparable <Variation>
{
    /// <summary> All megaPint package dependencies of this variation </summary>
    public List <Dependency> dependencies;

    /// <summary> Branch used for development </summary>
    public string devBranch;

    /// <summary> Key to the corresponding basePackage </summary>
    public PackageKey key;

    /// <summary> DisplayName of this variation </summary>
    public string name;

    /// <summary> Tag used to differentiate the main branch and the variation branch tags </summary>
    public string tag;

    /// <summary> Version of this variation </summary>
    public string version;

    #region Public Methods

    /// <summary> Compare method against another <see cref="Variation" /> </summary>
    /// <param name="other"> Compare against </param>
    /// <returns> Compare int value </returns>
    public int CompareTo(Variation other)
    {
        return string.Compare(name, other.name, StringComparison.Ordinal);
    }

    #endregion
}

}
#endif
