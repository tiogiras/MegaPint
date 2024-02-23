#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.PackageManager.Packages;
using Editor.Scripts.PackageManager.Utility;
using Editor.Scripts.Settings;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Editor.Scripts.PackageManager
{

internal static class MegaPintPackageManager
{
    public static Action onRefreshingPackages;
    
    public static int RefreshRate = 10;

    public static Action onSuccess;
    public static Action <string> onFailure;

    #region Public Methods

    public static async void UpdateAll()
    {
        var version = PackageCache.BasePackage.version;
        
        PackageCache.GetInstalledMpPackages(out List <CachedPackage> packages, out List <CachedVariation> variations);
        
        if (packages.Count > 0)
        {
            foreach (CachedPackage package in packages)
            {
                await AddEmbedded(package);
            }   
        }

        if (variations.Count > 0)
        {
            foreach (CachedVariation variation in variations)
            {
                await AddEmbedded(variation);
            }   
        }

        Debug.Log("basePackage");
        Debug.Log(MegaPintSettings.instance.GetSetting("General").GetValue("devMode", false));

        if (MegaPintSettings.instance.GetSetting("General").GetValue("devMode", false))
            await AddEmbedded($"{PackageCache.BasePackage.repository}#{DataCache.BasePackageDevBranch}");
        else
            await AddEmbedded($"{PackageCache.BasePackage.repository}#v{version}");

        PackageCache.Refresh();
    }

    public static async Task AddEmbedded(CachedPackage package)
    {
        await AddEmbedded(PackageManagerUtility.GetPackageUrl(package), package.Dependencies);
    }

    public static async Task AddEmbedded(CachedVariation variation)
    {
        await AddEmbedded(PackageManagerUtility.GetPackageUrl(variation), variation.dependencies);
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
            PackageCache.Refresh();
        }
    }
    
    public static async Task <List <PackageInfo>> GetInstalledPackages()
    {
        ListRequest request = Client.List();

        while (!request.IsCompleted)
        {
            await Task.Delay(RefreshRate);

            onRefreshingPackages?.Invoke();
        }

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);

        return request.Result.Where(packageInfo => packageInfo.name.ToLower().Contains("megapint")).ToList();
    }

    #endregion

    #region Private Methods
    
    private static async Task <bool> Add(string packageUrl)
    {
        AddRequest request = Client.Add(packageUrl);

        while (!request.IsCompleted)
            await Task.Delay(RefreshRate);

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);

        return request.Status == StatusCode.Success;
    }

    private static async Task AddEmbedded(string gitUrl, List <Dependency> dependencies)
    {
        if (dependencies is {Count: > 0})
        {
            foreach (CachedPackage cachedPackage in dependencies.Select(dependency => PackageCache.Get(dependency.key)))
            {
                await AddEmbedded(PackageManagerUtility.GetPackageUrl(cachedPackage), cachedPackage.Dependencies);
                await Task.Delay(250);
            }
        }

        await Task.Delay(250);
        await AddEmbedded(gitUrl);

        onSuccess?.Invoke();
        PackageCache.Refresh();
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
            // ignored
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

    #endregion
}

}
#endif
