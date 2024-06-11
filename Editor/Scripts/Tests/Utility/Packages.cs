#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.PackageManager;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;

namespace MegaPint.Editor.Scripts.Tests.Utility
{

// TODO commenting

/// <summary> Partial utility class for unit testing </summary>
internal static partial class TestsUtility
{
    private static bool s_packageManagerResult;
    
    public static async Task <bool> ImportPackage(PackageKey key)
    {
        s_packageManagerResult = false;
        
        MegaPintPackageManager.onSuccess += OnSuccess;
        
        await MegaPintPackageManager.AddEmbedded(PackageCache.Get(key));
        
        MegaPintPackageManager.onSuccess -= OnSuccess;

        return s_packageManagerResult;
    }

    public static void ValidatePackageDependencies(ref bool isValid, PackageKey key)
    {
        foreach (Dependency dependency in PackageCache.Get(key).Dependencies)
        {
            Validate(
                ref isValid,
                PackageCache.Get(dependency.key).CanBeRemoved(out List <PackageKey> _),
                $"Could remove {dependency.name} but it should not be removable due to dependencies!");
        }
    }
    
    public static async Task <bool> ImportVariation(PackageKey key, int variationIndex)
    {
        s_packageManagerResult = false;
        
        MegaPintPackageManager.onSuccess += OnSuccess;
        
        await MegaPintPackageManager.AddEmbedded(PackageCache.Get(key).Variations[variationIndex]);
        
        MegaPintPackageManager.onSuccess -= OnSuccess;

        return s_packageManagerResult;
    }

    public static async Task <bool> RemovePackage(PackageKey key)
    {
        s_packageManagerResult = false;
        
        MegaPintPackageManager.onSuccess += OnSuccess;
        
        await MegaPintPackageManager.Remove(PackageCache.Get(key).Name);
        
        MegaPintPackageManager.onSuccess -= OnSuccess;

        return s_packageManagerResult;
    }

    private static void OnSuccess()
    {
        s_packageManagerResult = true;
    }
}

}
#endif
#endif
