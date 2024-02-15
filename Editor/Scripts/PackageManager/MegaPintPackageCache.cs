using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.PackageManager
{

internal class MegaPintPackageCache
{

    private static MegaPintPackageCache s_allPackages;

    private readonly List <PackageCache> _packages = new();

    private PackageInfo _basePackage;
    private string _basePackageVersion;

    private readonly Dictionary <MegaPintPackagesData.PackageKey, List <string>> _dependencies = new();

    private MegaPintPackageCache()
    {
        Initialize();
    }

    #region Public Methods

    public static PackageInfo BasePackage()
    {
        return s_allPackages._basePackage;
    }

    public static bool BasePackageUpdate()
    {
        // Should be repository.url
        
        Debug.Log(BasePackage());
        Debug.Log(BasePackage().repository); // NULL
        Debug.Log(BasePackage().source);
        Debug.Log(BasePackage().git);
        Debug.Log(BasePackage().git.revision);
        Debug.Log(BasePackage().git.hash);
        Debug.Log(BasePackage().registry);
        Debug.Log(BasePackage().registry.url);

        //var repository = BasePackage().repository.url;
        //var tag = GitExtension.LatestGitTag(repository);

        return false;
    }

    public static string BasePackageVersion()
    {
        return s_allPackages._basePackageVersion;
    }

    public static void GetInstalled(
        out List <MegaPintPackagesData.MegaPintPackageData> packages,
        out List <MegaPintPackagesData.MegaPintPackageData.PackageVariation> variations)
    {
        packages = new List <MegaPintPackagesData.MegaPintPackageData>();
        variations = new List <MegaPintPackagesData.MegaPintPackageData.PackageVariation>();

        foreach (PackageCache packageCache in s_allPackages._packages)
        {
            if (!packageCache.installed)
                continue;

            MegaPintPackagesData.MegaPintPackageData package = MegaPintPackagesData.PackageData(packageCache.key);

            if (string.IsNullOrEmpty(packageCache.currentVariation))
            {
                packages.Add(package);

                continue;
            }

            foreach (MegaPintPackagesData.MegaPintPackageData.PackageVariation variation in package.variations.
                         Where(variation => s_allPackages.IsVariation(package.key, MegaPintPackageManager.GetVariationHash(variation, true))))
            {
                variations.Add(variation);

                break;
            }
        }
    }

    public static void Refresh()
    {
        s_allPackages = null;
        RequestAllPackages();
    }

    public static void RequestAllPackages()
    {
        if (s_allPackages == null)
        {
            var newInstance = new MegaPintPackageCache();
            s_allPackages = newInstance;
        }
        else
        {
            if (OnCompleteActions is not {Count: > 0})
                return;

            foreach (ListableAction <MegaPintPackageCache> action in OnCompleteActions)
                action?.Invoke(s_allPackages);
        }
    }

    public static void UpdateLoadingLabel(Label loadingLabel, int currentProgress, int refreshRate, out int newProgress)
    {
        if (currentProgress >= refreshRate)
        {
            currentProgress = 0;

            var pointCount = loadingLabel.text.Count(c => c == '.');
            var loadingText = new StringBuilder("Loading");

            if (pointCount == 3)
                pointCount = 0;

            for (var i = 0; i < pointCount + 1; i++)
                loadingText.Append(".");

            loadingLabel.style.display = DisplayStyle.Flex;
            loadingLabel.text = loadingText.ToString();
        }
        else
            currentProgress += MegaPintPackageManager.RefreshRate;

        newProgress = currentProgress;
    }

    public bool CanBeRemoved(MegaPintPackagesData.PackageKey key, out List <string> packages)
    {
        return !_dependencies.TryGetValue(key, out packages);
    }

    public string CurrentVersion(MegaPintPackagesData.PackageKey key)
    {
        return _packages.First(package => package.key == key).currentVersion;
    }

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public bool IsImported(MegaPintPackagesData.PackageKey key)
    {
        return _packages.First(package => package.key == key).installed;
    }

    public bool IsVariation(MegaPintPackagesData.PackageKey key, string gitHash)
    {
        return _packages.First(package => package.key == key).currentVariation.Equals(gitHash);
    }

    public bool NeedsUpdate(MegaPintPackagesData.PackageKey key)
    {
        return !_packages.First(package => package.key == key).newestVersion;
    }

    public bool NeedsVariationUpdate(MegaPintPackagesData.PackageKey key, string niceName)
    {
        PackageCache package = _packages.First(package => package.key == key); // <- package.variations is null

        VariationsCache variation = package.variations.First(variation => variation.niceName.Equals(niceName));

        return !variation.newestVersion;
    }

    public List <MegaPintPackagesData.MegaPintPackageData> ToDisplay()
    {
        return _packages.Select(package => MegaPintPackagesData.PackageData(package.key)).ToList();
    }

    #endregion

    #region Private Methods

    

    private void RegisterDependencies(
        MegaPintPackagesData.PackageKey key,
        string variation,
        List <MegaPintPackagesData.MegaPintPackageData.Dependency> dependencies)
    {
        var name = $"{key}{(string.IsNullOrEmpty(variation) ? "" : $"/{variation}")}";

        if (dependencies is not {Count: > 0})
            return;

        foreach (MegaPintPackagesData.MegaPintPackageData.Dependency dependency in dependencies)
        {
            if (_dependencies.TryGetValue(dependency.packageKey, out List <string> list))
            {
                list.Add(name);

                continue;
            }

            _dependencies.Add(dependency.packageKey, new List <string> {name});
        }
    }

    #endregion

    #region Actions

    public static readonly List <ListableAction> OnUpdateActions = new();
    public static readonly List <ListableAction <MegaPintPackageCache>> OnCompleteActions = new();

    public class ListableAction
    {
        public readonly string name;
        private readonly Action _action;

        public ListableAction(Action action, string name)
        {
            _action = action;
            this.name = name;
        }

        #region Public Methods

        public void Invoke()
        {
            _action.Invoke();
        }

        #endregion
    }

    public class ListableAction <T>
    {
        public readonly string name;
        private readonly Action <T> _action;

        public ListableAction(Action <T> action, string name)
        {
            _action = action;
            this.name = name;
        }

        #region Public Methods

        public void Invoke(T value)
        {
            _action.Invoke(value);
        }

        #endregion
    }

    public static void RemoveUpdateAction(string name)
    {
        if (OnUpdateActions is not {Count: > 0})
            return;

        for (var i = 0; i < OnUpdateActions.Count; i++)
        {
            ListableAction action = OnUpdateActions[i];

            if (!action.name.Equals(name))
                continue;

            OnUpdateActions.RemoveAt(i);

            return;
        }
    }

    public static void RemoveCompleteAction(string name)
    {
        if (OnCompleteActions is not {Count: > 0})
            return;

        for (var i = 0; i < OnCompleteActions.Count; i++)
        {
            ListableAction <MegaPintPackageCache> action = OnCompleteActions[i];

            if (!action.name.Equals(name))
                continue;

            OnCompleteActions.RemoveAt(i);

            return;
        }
    }

    #endregion
}

}
