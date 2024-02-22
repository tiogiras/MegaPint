#if UNITY_EDITOR
using System.Collections;
using Editor.Scripts.PackageManager.Cache;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Editor.Scripts.Tests
{

public class PackageCacheTests : MonoBehaviour
{
    private bool _initialized;
    
    [UnityTest, Order(0)]
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

    private void CacheRefreshed()
    {
        PackageCache.onCacheRefreshed -= CacheRefreshed;
        _initialized = true;
    }
}

}
#endif
