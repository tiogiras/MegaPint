#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Editor.Scripts.Settings;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace Editor.Scripts.PackageManager
{

internal static class MegaPintPackageManager
{
    public static int RefreshRate = 10;

    public static Action onSuccess;
    public static Action <string> onFailure;

    #region Public Methods

    public static async void UpdateAll()
    {
        var version = MegaPintPackageCache.BasePackageVersion();
        
        MegaPintPackageCache.GetInstalled(
            out List <MegaPintPackagesData.MegaPintPackageData> packages, 
            out List <MegaPintPackagesData.MegaPintPackageData.PackageVariation> variations);
        
        if (packages.Count > 0)
        {
            foreach (MegaPintPackagesData.MegaPintPackageData package in packages)
            {
                await AddEmbedded(package);
            }   
        }

        if (variations.Count > 0)
        {
            foreach (MegaPintPackagesData.MegaPintPackageData.PackageVariation variation in variations)
            {
                await AddEmbedded(variation);
            }   
        }

        if (MegaPintSettings.instance.GetSetting("General").GetValue("devMode", false))
            await AddEmbedded(MegaPintPackagesData.BasePackageDevURL);
        else
            await AddEmbedded($"https://github.com/tiogiras/MegaPint.git#v{version}");

        MegaPintPackageCache.Refresh();
    }

    public static async Task AddEmbedded(MegaPintPackagesData.MegaPintPackageData package)
    {
        await AddEmbedded(GetPackageUrl(package), package.dependencies);
    }

    public static async Task AddEmbedded(MegaPintPackagesData.MegaPintPackageData.PackageVariation variation)
    {
        await AddEmbedded(GetPackageUrl(variation), variation.dependencies);
    }

    public static async void Remove(string packageName)
    {
        RemoveRequest request = Client.Remove(packageName);

        while (!request.IsCompleted)
            await Task.Delay(RefreshRate);

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);
        else
        {
            onSuccess?.Invoke();
            MegaPintPackageCache.Refresh();
        }
    }

    #endregion

    #region Private Methods

    public static string GetPackageHash(MegaPintPackagesData.MegaPintPackageData package)
    {
        var devMode = MegaPintSettings.instance.GetSetting("General").GetValue("devMode", false);
        return devMode ? "development" : $"v{package.version}";
    }
    
    private static string GetPackageUrl(MegaPintPackagesData.MegaPintPackageData package)
    {
        return $"{package.gitUrl}#{GetPackageHash(package)}";
    }

    public static string GetVariationHash(MegaPintPackagesData.MegaPintPackageData.PackageVariation variation, bool invert = false)
    {
        var devMode = MegaPintSettings.instance.GetSetting("General").GetValue("devMode", false);

        if (invert)
            devMode = !devMode;
        
        return devMode ? variation.developmentBranch : $"v{variation.version}{variation.variationTag}";
    }

    private static string GetPackageUrl(MegaPintPackagesData.MegaPintPackageData.PackageVariation variation)
    {
        return $"{variation.gitUrl}#{GetVariationHash(variation)}";
    }
    
    private static async Task <bool> Add(string packageUrl)
    {
        AddRequest request = Client.Add(packageUrl);

        while (!request.IsCompleted)
            await Task.Delay(RefreshRate);

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);

        return request.Status == StatusCode.Success;
    }

    private static async Task AddEmbedded(string gitUrl, List <MegaPintPackagesData.MegaPintPackageData.Dependency> dependencies)
    {
        if (dependencies is {Count: > 0})
        {
            foreach (MegaPintPackagesData.MegaPintPackageData.Dependency dependency in dependencies)
            {
                MegaPintPackagesData.MegaPintPackageData package = MegaPintPackagesData.PackageData(dependency.packageKey);

                await AddEmbedded(GetPackageUrl(package), package.dependencies);
                await Task.Delay(250);
            }
        }

        await Task.Delay(250);
        await AddEmbedded(gitUrl);

        onSuccess?.Invoke();
        MegaPintPackageCache.Refresh();
    }

    private static async Task AddEmbedded(string packageUrl)
    {
        if (!await Add(packageUrl))
            return;

        try
        {
            await Embed(packageUrl);
        }
        catch (Exception)
        {
            /* ignored */
        }
    }

    private static async Task Embed(string packageName)
    {
        EmbedRequest request = Client.Embed(packageName);

        while (!request.IsCompleted)
            await Task.Delay(RefreshRate);

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);
    }

    public static async Task <List <PackageInfo>> GetInstalledPackages()
    {
        ListRequest request = Client.List();

        while (!request.IsCompleted)
        {
            await Task.Delay(RefreshRate);

            if (MegaPintPackageCache.OnUpdateActions is not {Count: > 0})
                continue;

            foreach (MegaPintPackageCache.ListableAction action in MegaPintPackageCache.OnUpdateActions)
                action?.Invoke();
        }

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);

        return request.Result.Where(packageInfo => packageInfo.name.ToLower().Contains("megapint")).ToList();
    }

    #endregion
}

}
#endif
