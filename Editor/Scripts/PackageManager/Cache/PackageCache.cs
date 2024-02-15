using System.Collections.Generic;
using Editor.Scripts.PackageManager.Packages;
using UnityEditor;

namespace Editor.Scripts.PackageManager.Cache
{

internal class PackageCache
{
    private static UnityEditor.PackageManager.PackageInfo s_basePackage;
    
    private static async void Initialize()
    {
        IEnumerable <PackageData> mpPackages = DataCache.AllPackages;
        List <UnityEditor.PackageManager.PackageInfo> installedPackages = await MegaPintPackageManager.GetInstalledPackages();
        
        List <string> installedPackagesNames = new();
        
        // Get names of all installed packages and find basePackage
        foreach (UnityEditor.PackageManager.PackageInfo package in installedPackages)
        {
            installedPackagesNames.Add(package.name);

            if (!package.name.ToLower().Equals(DataCache.BasePackageName))
                continue;

            s_basePackage = package;
        }

        _dependencies.Clear();

        foreach (MegaPintPackagesData.MegaPintPackageData package in allPackages)
        {
            var installed = installedPackagesNames.Contains(package.name);
            var newestVersion = false;
            var currentVersion = "";
            var currentVariation = "";

            List <VariationsCache> variations = null;

            if (installed)
            {
                PackageInfo installedPackage = installedPackages[installedPackagesNames.IndexOf(package.name)];
                newestVersion = installedPackage.version == package.version;
                currentVersion = installedPackage.version;

                var commitHash = installedPackage.git?.hash;
                var branch = installedPackage.git?.revision;

                MegaPintPackagesData.MegaPintPackageData.PackageVariation installedVariation = null;

                if (package.variations is {Count: > 0})
                {
                    variations = package.variations.Select(
                                             variation => new VariationsCache
                                             {
                                                 niceName = variation.niceName, newestVersion = installedPackage.version == variation.version
                                             }).
                                         ToList();

                    foreach (MegaPintPackagesData.MegaPintPackageData.PackageVariation variation in package.variations)
                    {
                        var hash = MegaPintPackageManager.GetVariationHash(variation);

                        if (hash.Equals(commitHash))
                        {
                            currentVariation = commitHash;
                            installedVariation = variation;

                            break;
                        }

                        if (!hash.Equals(branch))
                            continue;

                        currentVariation = branch;
                        installedVariation = variation;
                    }
                }

                RegisterDependencies(
                    package.key,
                    installedVariation == null ? "" : installedVariation.niceName,
                    installedVariation == null ? package.dependencies : installedVariation.dependencies);
            }

            _packages.Add(
                new PackageCache
                {
                    key = package.key,
                    installed = installed,
                    newestVersion = newestVersion,
                    currentVersion = currentVersion,
                    currentVariation = currentVariation,
                    variations = variations
                });
        }

        if (OnCompleteActions is not {Count: > 0})
            return;

        foreach (MegaPintPackageCache.ListableAction <MegaPintPackageCache> action in OnCompleteActions)
            action?.Invoke(s_allPackages);
    }
}

}
