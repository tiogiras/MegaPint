﻿using System;
using System.Collections.Generic;

namespace Editor.Scripts.PackageManager.Packages
{

/// <summary> Stores data about a megaPint package that is set before distribution </summary>
internal class PackageData : IComparable <PackageData>
{
    /// <summary> Key for internal references </summary>
    public PackageKey key;
    
    /// <summary> Required megaPint basePackage version </summary>
    public string reqMpVersion;

    /// <summary> Version attribute of the corresponding package.json </summary>
    public string version;
    
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

    #region Public Methods

    public int CompareTo(PackageData other)
    {
        // TODO return string.Compare(packageNiceName, other.packageNiceName, StringComparison.Ordinal);
        return 0;
    }

    #endregion
}

}
