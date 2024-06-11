#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.Tests.Utility;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MegaPint.Editor.Scripts.Tests.PackageImport
{

/// <summary> Unit tests testing the import of the AlphaButton package </summary>
internal class AlphaButton
{
    private static bool s_initialized;

    private static PackageKey[] s_dependencies;

    #region Tests

    [Test] [Order(3)]
    public void CheckDependencies()
    {
        if (!s_initialized)
            Assert.Fail("FAILED ===> Missing packageCache initialization!");

        var isValid = true;

        s_dependencies = PackageCache.Get(PackageKey.AlphaButton).Dependencies.Select(package => package.key).ToArray();
        
        TestsUtility.ValidatePackageDependencies(ref isValid, PackageKey.AlphaButton);

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

        foreach (PackageKey dependency in s_dependencies)
        {
            Task <bool> task = TestsUtility.RemovePackage(dependency);

            yield return task.AsIEnumeratorReturnNull();

            TestsUtility.Validate(ref isValid, !task.Result, $"Failed to remove {dependency}!");
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
