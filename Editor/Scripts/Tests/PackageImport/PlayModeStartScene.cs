#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.Tests.Utility;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MegaPint.Editor.Scripts.Tests.PackageImport
{

/// <summary> Unit tests testing the import of the PlayModeStartScene package </summary>
internal class PlayModeStartScene
{
    private static bool s_initialized;

    #region Tests

    [UnityTest] [Order(1)]
    public IEnumerator ImportPackage()
    {
        TestsUtility.SkipIfProductionProject();

        if (!s_initialized)
            Assert.Fail("FAILED ===> Missing packageCache initialization!");

        Task <bool> task = TestsUtility.ImportPackage(PackageKey.PlayModeStartScene);

        yield return task.AsIEnumeratorReturnNull();

        Assert.IsTrue(task.Result);
    }

    [UnityTest] [Order(0)]
    public IEnumerator InitializePackageCache()
    {
        TestsUtility.SkipIfProductionProject();

        Task <bool> task = TestsUtility.CheckCacheInitialization();

        yield return task.AsIEnumeratorReturnNull();

        s_initialized = task.Result;
        Assert.IsTrue(task.Result);
    }

    [UnityTest] [Order(4)]
    public IEnumerator RemovePackage()
    {
        TestsUtility.SkipIfProductionProject();

        if (!s_initialized)
            Assert.Fail("FAILED ===> Missing packageCache initialization!");

        Task <bool> task = TestsUtility.RemovePackage(PackageKey.PlayModeStartScene);

        yield return task.AsIEnumeratorReturnNull();

        Assert.IsTrue(task.Result);
    }

    #endregion
}

}
#endif
#endif
