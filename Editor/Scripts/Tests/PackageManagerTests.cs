#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using System.Collections.Generic;
using MegaPint.Editor.Scripts.PackageManager;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MegaPint.Editor.Scripts.Tests
{

/// <summary> Unit tests regarding the <see cref="MegaPintPackageManager" /> </summary>
internal class PackageManagerTests
{
    private bool _initialized;
    private bool _result;
    private bool _waitingForCache;
    private bool _waitingForPackageManager;

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

    [UnityTest] [Order(3)]
    public IEnumerator DependenciesRegistered()
    {
        yield return new WaitForDomainReload();

        PackageCache.onCacheRefreshed += CacheRefreshed;
        _waitingForCache = true;
        PackageCache.Refresh();

        while (_waitingForCache)
            yield return null;

        Assert.IsFalse(PackageCache.Get(PackageKey.Validators).CanBeRemoved(out List <PackageKey> _));
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

    [UnityTest] [Order(5)]
    public IEnumerator RemoveFormerDependencyPackage()
    {
        yield return new WaitForDomainReload();

        PackageCache.onCacheRefreshed += CacheRefreshed;
        _waitingForCache = true;
        PackageCache.Refresh();

        while (_waitingForCache)
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

    [UnityTest] [Order(4)]
    public IEnumerator RemovePackage()
    {
        yield return new WaitForDomainReload();

        PackageCache.onCacheRefreshed += CacheRefreshed;
        _waitingForCache = true;
        PackageCache.Refresh();

        while (_waitingForCache)
            yield return null;

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

    #endregion

    #region Private Methods

    private void CacheRefreshed()
    {
        PackageCache.onCacheRefreshed -= CacheRefreshed;
        _initialized = true;
        _waitingForCache = false;
    }

    private void Failure(string error)
    {
        MegaPintPackageManager.onSuccess -= Success;
        MegaPintPackageManager.onFailure -= Failure;

        Debug.LogError(error);
        _waitingForPackageManager = false;
        _result = false;
    }

    private void Success()
    {
        MegaPintPackageManager.onSuccess -= Success;
        MegaPintPackageManager.onFailure -= Failure;

        _waitingForPackageManager = false;
        _result = true;
    }

    #endregion
}

}
#endif
#endif
