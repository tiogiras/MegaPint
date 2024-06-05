#if UNITY_EDITOR
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using UnityEngine;

namespace MegaPint.Editor.Scripts.Tests.Utility
{

/// <summary> Partial utility class for unit testing </summary>
internal static partial class TestsUtility
{
    private static bool s_initialized;

    #region Public Methods

    /// <summary> Check if the <see cref="PackageCache" /> was initialized </summary>
    /// <returns> Initialization status of the <see cref="PackageCache" /> </returns>
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

    #endregion

    #region Private Methods

    /// <summary> Callback when <see cref="PackageCache" /> was refreshed </summary>
    private static void CacheRefreshed()
    {
        PackageCache.onCacheRefreshed -= CacheRefreshed;
        s_initialized = true;
    }

    #endregion
}

}
#endif
