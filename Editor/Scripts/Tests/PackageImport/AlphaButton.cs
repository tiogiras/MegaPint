#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.Tests.Utility;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MegaPint.Editor.Scripts.Tests.PackageImport
{

/// <summary> Unit tests testing the import of the AlphaButton package </summary>
internal class AlphaButton
{
    private static bool s_initialized;

    #region Tests

    [Test] [Order(3)]
    public void CheckDependencies()
    {
        if (!s_initialized)
            Assert.Fail("FAILED ===> Missing packageCache initialization!");

        var isValid = true;

        Debug.Log(PackageCache.Get(PackageKey.AlphaButton).DisplayName);
        Debug.Log(PackageCache.Get(PackageKey.AlphaButton).Dependencies);
        
        foreach (Dependency dependency in PackageCache.Get(PackageKey.AlphaButton).Dependencies)
        {
            TestsUtility.Validate(
                ref isValid,
                PackageCache.Get(dependency.key).CanBeRemoved(out List <PackageKey> _),
                $"Could remove {dependency.name} but it should not be removable due to dependencies!");
        }

        Assert.IsTrue(isValid);
    }

    [UnityTest] [Order(1)]
    public IEnumerator ImportPackage()
    {
        if (!s_initialized)
            Assert.Fail("FAILED ===> Missing packageCache initialization!");

        Task <bool> task = TestsUtility.ImportPackage(PackageKey.AlphaButton);

        yield return task.AsIEnumeratorReturnNull();

        Assert.IsTrue(task.Result);
    }

    [UnityTest] [Order(2)]
    public IEnumerator ImportVariation()
    {
        if (!s_initialized)
            Assert.Fail("FAILED ===> Missing packageCache initialization!");

        Task <bool> task = TestsUtility.ImportVariation(PackageKey.AlphaButton, 0);

        yield return task.AsIEnumeratorReturnNull();

        Assert.IsTrue(task.Result);
    }

    [UnityTest] [Order(0)]
    public IEnumerator InitializePackageCache()
    {
        Task <bool> task = TestsUtility.CheckCacheInitialization();

        yield return task.AsIEnumeratorReturnNull();

        s_initialized = task.Result;
        Assert.IsTrue(task.Result);
    }

    [UnityTest] [Order(5)]
    public IEnumerator RemoveFormerDependencies()
    {
        if (!s_initialized)
            Assert.Fail("FAILED ===> Missing packageCache initialization!");

        var isValid = true;

        foreach (Dependency dependency in PackageCache.Get(PackageKey.AlphaButton).Dependencies)
        {
            Task <bool> task = TestsUtility.RemovePackage(dependency.key);

            yield return task.AsIEnumeratorReturnNull();

            TestsUtility.Validate(ref isValid, !task.Result, $"Failed to remove {dependency.name}!");
        }

        Assert.IsTrue(isValid);
    }

    [UnityTest] [Order(4)]
    public IEnumerator RemovePackage()
    {
        if (!s_initialized)
            Assert.Fail("FAILED ===> Missing packageCache initialization!");

        Task <bool> task = TestsUtility.RemovePackage(PackageKey.AlphaButton);

        yield return task.AsIEnumeratorReturnNull();

        Assert.IsTrue(task.Result);
    }

    #endregion
}

}
#endif
#endif
