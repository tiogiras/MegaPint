using System.Collections;
using System.Collections.Generic;
using Editor.Scripts.PackageManager;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.PackageManager.Packages;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Editor.Scripts.Tests
{

public class PackageManagerTests
{
    private bool _initialized;
    private bool _result;
    private bool _waitingForPackageManager;
    private bool _waitingForCache;

    #region Tests

    [UnityTest] [Order(0)]
    public IEnumerator CacheInitialized()
    {
        _initialized = false;
        var currentTime = 0f;

        PackageCache.onCacheRefreshed += CacheRefreshed;
        PackageCache.Refresh();

        while (!_initialized && currentTime < 200)
        {
            currentTime += Time.deltaTime;

            yield return null;
        }

        Assert.IsTrue(_initialized);
    }
    
    [UnityTest] [Order(1)]
    public IEnumerator ImportPackage()
    {
        MegaPintPackageManager.onSuccess += Success;
        MegaPintPackageManager.onFailure += Failure;

        PackageCache.onCacheRefreshed += CacheRefreshed;

        _result = false;
        _waitingForPackageManager = true;
        _waitingForCache = true;

#pragma warning disable CS4014
        MegaPintPackageManager.AddEmbedded(PackageCache.Get(PackageKey.AlphaButton));
#pragma warning restore CS4014

        while (_waitingForPackageManager || _waitingForCache)
            yield return null;

        Assert.IsTrue(_result);
    }
    
    [UnityTest] [Order(2)]
    public IEnumerator ImportPackageVariation()
    {
        MegaPintPackageManager.onSuccess += Success;
        MegaPintPackageManager.onFailure += Failure;
        
        PackageCache.onCacheRefreshed += CacheRefreshed;

        _result = false;
        _waitingForPackageManager = true;
        _waitingForCache = true;

#pragma warning disable CS4014
        MegaPintPackageManager.AddEmbedded(PackageCache.Get(PackageKey.AlphaButton).Variations[0]);
#pragma warning restore CS4014

        while (_waitingForPackageManager || _waitingForCache)
            yield return null;

        Assert.IsTrue(_result);
    }

    [UnityTest] [Order(3)]
    public IEnumerator DependenciesRegistered()
    {
        PackageCache.onCacheRefreshed += CacheRefreshed;
        
        while (_waitingForCache)
            yield return null;
        
        Assert.IsFalse(PackageCache.Get(PackageKey.Validators).CanBeRemoved(out List <Dependency> _));
    }

    [UnityTest] [Order(4)]
    public IEnumerator RemovePackage()
    {
        MegaPintPackageManager.onSuccess += Success;
        MegaPintPackageManager.onFailure += Failure;
        
        PackageCache.onCacheRefreshed += CacheRefreshed;

        _result = false;
        _waitingForPackageManager = true;
        _waitingForCache = true;

#pragma warning disable CS4014
        MegaPintPackageManager.Remove(PackageCache.Get(PackageKey.AlphaButton).Name);
#pragma warning restore CS4014

        while (_waitingForPackageManager || _waitingForCache)
            yield return null;

        Assert.IsTrue(_result);
    }
    
    [UnityTest] [Order(5)]
    public IEnumerator RemoveFormerDependencyPackage()
    {
        yield return null;
        
        MegaPintPackageManager.onSuccess += Success;
        MegaPintPackageManager.onFailure += Failure;
        
        PackageCache.onCacheRefreshed += CacheRefreshed;

        _result = false;
        _waitingForPackageManager = true;
        _waitingForCache = true;

#pragma warning disable CS4014
        MegaPintPackageManager.Remove(PackageCache.Get(PackageKey.Validators).Name);
#pragma warning restore CS4014

        while (_waitingForPackageManager || _waitingForCache)
            yield return null;

        Assert.IsTrue(_result);
    }

    #endregion

    #region Private Methods

    private void CacheRefreshed()
    {
        PackageCache.onCacheRefreshed -= CacheRefreshed;
        _initialized = true;
        _waitingForCache = false;

        Debug.Log("Cache Wait = false");
    }

    private void Failure(string error)
    {
        MegaPintPackageManager.onSuccess -= Success;
        MegaPintPackageManager.onFailure -= Failure;

        Debug.Log(error);
        _waitingForPackageManager = false;
        _result = false;
        
        Debug.Log("Wait = false");
    }

    private void Success()
    {
        MegaPintPackageManager.onSuccess -= Success;
        MegaPintPackageManager.onFailure -= Failure;

        _waitingForPackageManager = false;
        _result = true;
        
        Debug.Log("Wait = false");
    }

    #endregion
}

}
