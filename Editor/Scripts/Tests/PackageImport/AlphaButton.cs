using System.Collections;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.PackageManager;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.Tests.Utility;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
namespace MegaPint.Editor.Scripts.Tests.PackageImport
{

/// <summary> Unit tests testing the import of the AlphaButton package </summary>
internal class AlphaButton
{
    private static bool s_initialized;
    
    [UnityTest] [Order(0)]
    public IEnumerator InitializePackageCache()
    {
        Task <bool> task = TestsUtility.CheckCacheInitialization();
        yield return task.AsIEnumeratorReturnNull();

        s_initialized = task.Result;
        Assert.IsTrue(task.Result);
    }
    
    [UnityTest] [Order(1)]
    public IEnumerator ImportPackage()
    {
        if (!s_initialized)
            Assert.Fail("FAILED ===> Missing packageCache initialization!");
        
        Task <bool> task = TestsUtility.ImportPackage(PackageKey.AlphaButton);
        yield return task.AsIEnumeratorReturnNull();

        Debug.Log("Import result");
        Debug.Log(task.Result);
        
        Assert.IsTrue(task.Result);
    }
    
    [UnityTest] [Order(2)]
    public IEnumerator RemovePackage()
    {
        if (!s_initialized)
            Assert.Fail("FAILED ===> Missing packageCache initialization!");
        
        Task <bool> task = TestsUtility.RemovePackage(PackageKey.AlphaButton);
        yield return task.AsIEnumeratorReturnNull();

        Debug.Log("Remove result");
        Debug.Log(task.Result);
        
        Assert.IsTrue(task.Result);
    }
    


    //     #region Tests
//
//     // TODO
//
//     [UnityTest] [Order(1)]
//     public IEnumerator ImportPackage()
//     {
//         MegaPintPackageManager.onSuccess += Success;
//         MegaPintPackageManager.onFailure += Failure;
//
//         PackageCache.onCacheRefreshed += CacheRefreshed;
//
//         _result = false;
//         _waitingForPackageManager = true;
//         _waitingForCache = true;
//
// #pragma warning disable CS4014
//         MegaPintPackageManager.AddEmbedded(PackageCache.Get(PackageKey.AlphaButton));
// #pragma warning restore CS4014
//
//         while (_waitingForPackageManager || _waitingForCache)
//             yield return null;
//
//         Assert.IsTrue(_result);
//     }
//
//     #endregion
//
//     #region Private Methods
//
//     private void CacheRefreshed()
//     {
//         PackageCache.onCacheRefreshed -= CacheRefreshed;
//         _initialized = true;
//         _waitingForCache = false;
//     }
//
//     private void Failure(string error)
//     {
//         MegaPintPackageManager.onSuccess -= Success;
//         MegaPintPackageManager.onFailure -= Failure;
//
//         Debug.LogError(error);
//         _waitingForPackageManager = false;
//         _result = false;
//     }
//
//     private void Success()
//     {
//         MegaPintPackageManager.onSuccess -= Success;
//         MegaPintPackageManager.onFailure -= Failure;
//
//         _waitingForPackageManager = false;
//         _result = true;
//     }
//
//     #endregion
}

}
#endif
#endif
