#if UNITY_EDITOR
using System;

namespace MegaPint.Editor.Scripts.PackageManager.Packages
{

/// <summary> Stores data about a MegaPint package dependency that is set before distribution </summary>
internal class Dependency : IComparable <Dependency>
{
    /// <summary> Key for internal references </summary>
    public PackageKey key;

    /// <summary> Display name of this dependency </summary>
    public string name;

    #region Public Methods

    /// <summary> Compare method against another <see cref="Dependency" /> </summary>
    /// <param name="other"> Compare against </param>
    /// <returns> Compare int value </returns>
    public int CompareTo(Dependency other)
    {
        return string.Compare(name, other.name, StringComparison.Ordinal);
    }

    #endregion
}

}
#endif
