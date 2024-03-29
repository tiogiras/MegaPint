#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.PackageManager.Packages;
using Editor.Scripts.PackageManager.Utility;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace Editor.Scripts.PackageManager
{

internal static class MegaPintPackageManager
{
    /// <summary> RefreshRate while loading packages </summary>
    public const int RefreshRate = 10;

    /// <summary> Called while requesting packages </summary>
    public static Action onRefreshingPackages;

    /// <summary> Called after successfully calling an action regarding the packageManager  </summary>
    public static Action onSuccess;

    /// <summary> Called after failing an action regarding the packageManager </summary>
    public static Action <string> onFailure;

    #region Public Methods

    /// <summary> Add and Embed a package </summary>
    /// <param name="package"> <see cref="CachedPackage" /> to be added </param>
    public static async Task AddEmbedded(CachedPackage package)
    {
        await AddEmbedded(PackageManagerUtility.GetPackageUrl(package), package.Dependencies);
    }

    /// <summary> Add and Embed a variation </summary>
    /// <param name="variation"> <see cref="CachedVariation" /> to be added </param>
    public static async Task AddEmbedded(CachedVariation variation)
    {
        await AddEmbedded(PackageManagerUtility.GetPackageUrl(variation), variation.dependencies);
    }

    /// <summary> Get all installed packages </summary>
    /// <returns> All <see cref="PackageInfo" /> that are currently installed </returns>
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

    /// <summary> Remove a package </summary>
    /// <param name="packageName"> Name of the package to be removed </param>
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

    /// <summary> Update all packages to their currently specified version </summary>
    public static async void UpdateAll()
    {
        var version = PackageCache.BasePackage.version;

        PackageCache.GetInstalledMpPackages(out List <CachedPackage> packages, out List <CachedVariation> variations);

        if (packages.Count > 0)
        {
            foreach (CachedPackage package in packages)
                await AddEmbedded(package);
        }

        if (variations.Count > 0)
        {
            foreach (CachedVariation variation in variations)
                await AddEmbedded(variation);
        }

        var url = SaveData.DevMode()
            ? $"{PackageCache.BasePackage.repository.url}#{DataCache.BasePackageDevBranch}"
            : $"{PackageCache.BasePackage.repository.url}#v{version}";

        await AddEmbedded(url);

        PackageCache.Refresh();
    }

    /// <summary> Update the basePackage to it's newest possible version </summary>
    public static async Task UpdateBasePackage()
    {
        var url = SaveData.DevMode()
            ? $"{PackageCache.BasePackage.repository.url}#{DataCache.BasePackageDevBranch}"
            : $"{PackageCache.BasePackage.repository.url}#v{PackageCache.NewestBasePackageVersion}";

        await AddEmbedded(url);
        PackageCache.Refresh();
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
