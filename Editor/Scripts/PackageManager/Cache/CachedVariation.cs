using System.Collections.Generic;
using Editor.Scripts.PackageManager.Packages;

namespace Editor.Scripts.PackageManager.Cache
{

internal class CachedVariation
{
    public string name;
    public bool isNewestVersion;
    public string devBranch;
    public string version;
    public string tag;
    public string repository;
    public List <Dependency> dependencies;
}

}
