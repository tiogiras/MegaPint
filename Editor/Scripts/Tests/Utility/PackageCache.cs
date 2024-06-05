#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using NUnit.Framework;
using UnityEngine;

namespace MegaPint.Editor.Scripts.Tests.Utility
{

/// <summary> Partial utility class for unit testing </summary>
internal static partial class TestsUtility
{
    private static bool s_initialized;
    
    public static async Task <bool> CheckCacheInitialization()
    {
        var currentTime = 0f;

        s_initialized = false;

        PackageCache.onCacheRefreshed += CacheRefreshed;
        PackageCache.Refresh();

        while (!s_initialized && currentTime < 200)
        {
            currentTime += Time.deltaTime;

            await Task.Delay(100);
        }

        return s_initialized;
    }
    
    private static void CacheRefreshed()
    {
        PackageCache.onCacheRefreshed -= CacheRefreshed;
        s_initialized = true;
    }
}

}
#endif
