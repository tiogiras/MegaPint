#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor.Scripts.Settings;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.PackageManager
{

internal static class MegaPintPackageManager
{
    public class CachedPackages
    {
        private struct VariationsCache
        {
            public string niceName;
            public bool newestVersion;
        }

        private struct PackageCache
        {
            public MegaPintPackagesData.PackageKey key;
            public bool installed;
            public bool newestVersion;
            public string currentVersion;
            public List <VariationsCache> variations;
            public string currentVariation;
        }

        private static CachedPackages s_allPackages;

        private readonly List <PackageCache> _packages = new();

        private Dictionary <MegaPintPackagesData.PackageKey, List<string>> _dependencies = new();

        private CachedPackages()
        {
            Initialize();
        }

        #region Public Methods

        public static void Refresh()
        {
            s_allPackages = null;
            RequestAllPackages();
        }

        public static void RequestAllPackages()
        {
            if (s_allPackages == null)
            {
                var newInstance = new CachedPackages();
                s_allPackages = newInstance;
            }
            else
            {
                if (OnCompleteActions is not {Count: > 0})
                    return;

                foreach (ListableAction <CachedPackages> action in OnCompleteActions)
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
                currentProgress += RefreshRate;

            newProgress = currentProgress;
        }

        public string CurrentVersion(MegaPintPackagesData.PackageKey key)
        {
            return _packages.First(package => package.key == key).currentVersion;
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
        
        public static List <MegaPintPackagesData.MegaPintPackageData> GetInstalled()
        {
            return (from packageCache in s_allPackages._packages where packageCache.installed 
                    select MegaPintPackagesData.PackageData(packageCache.key)).ToList();
        }

        #endregion

        #region Private Methods

        private async void Initialize()
        {
            List <MegaPintPackagesData.MegaPintPackageData> allPackages = MegaPintPackagesData.Packages;
            List <PackageInfo> installedPackages = await GetInstalledPackages();
            List <string> installedPackagesNames = installedPackages.Select(installedPackage => installedPackage.name).ToList();
            
            _dependencies.Clear();

            foreach (MegaPintPackagesData.MegaPintPackageData package in allPackages)
            {
                var installed = installedPackagesNames.Contains(package.packageName);
                var newestVersion = false;
                var currentVersion = "";
                var currentVariation = "";

                List <VariationsCache> variations = null;

                if (installed)
                {
                    PackageInfo installedPackage = installedPackages[installedPackagesNames.IndexOf(package.packageName)];
                    newestVersion = installedPackage.version == package.version;
                    currentVersion = installedPackage.version;
                    
                    var commitHash = installedPackage.git?.hash;
                    var branch = installedPackage.git?.revision;

                    MegaPintPackagesData.MegaPintPackageData.PackageVariation installedVariation = null;
                    
                    if (package.variations is {Count: > 0})
                    {
                        variations = package.variations.Select(
                                                 variation => new VariationsCache
                                                 {
                                                     niceName = variation.niceName,
                                                     newestVersion = installedPackage.version == variation.version,
                                                 }).
                                             ToList();
                        
                        foreach (MegaPintPackagesData.MegaPintPackageData.PackageVariation variation in package.variations)
                        {
                            var importedUrlHash = $"v{variation.version}{variation.variationTag}";

                            if (importedUrlHash.Equals(commitHash))
                            {
                                currentVariation = commitHash;
                                installedVariation = variation;
                                break;
                            }

                            if (!importedUrlHash.Equals(branch))
                                continue;

                            currentVariation = branch;
                            installedVariation = variation;
                        }
                    }

                    RegisterDependencies(
                        package.packageKey,
                        installedVariation == null ? "" : installedVariation.niceName,
                        installedVariation == null ? package.dependencies : installedVariation.dependencies);
                }
                
                _packages.Add(
                    new PackageCache
                    {
                        key = package.packageKey,
                        installed = installed,
                        newestVersion = newestVersion,
                        currentVersion = currentVersion,
                        currentVariation = currentVariation,
                        variations = variations
                    });
            }

            if (OnCompleteActions is not {Count: > 0})
                return;

            foreach (ListableAction <CachedPackages> action in OnCompleteActions)
                action?.Invoke(s_allPackages);
        }

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
                
                _dependencies.Add(dependency.packageKey, new List <string>{name});
            }
        }

        public bool CanBeRemoved(MegaPintPackagesData.PackageKey key, out List <string> packages)
        {
            return !_dependencies.TryGetValue(key, out packages);
        }

        #endregion

        #region Actions

        public static readonly List <ListableAction> OnUpdateActions = new();
        public static readonly List <ListableAction <CachedPackages>> OnCompleteActions = new();

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
                ListableAction <CachedPackages> action = OnCompleteActions[i];

                if (!action.name.Equals(name))
                    continue;

                OnCompleteActions.RemoveAt(i);

                return;
            }
        }

        #endregion

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    private const int RefreshRate = 10;

    public static Action onSuccess;
    public static Action <string> onFailure;

    #region Public Methods

    public static async void UpdateAll()
    {
        foreach (MegaPintPackagesData.MegaPintPackageData packageData in CachedPackages.GetInstalled())
        {
            Debug.Log($"Updating {packageData.packageKey}");
            await AddEmbedded(packageData);
        }
    }
    
    public static async Task AddEmbedded(MegaPintPackagesData.MegaPintPackageData package)
    {
        await AddEmbedded(GetPackageUrl(package), package.dependencies);
    }

    public static async Task AddEmbedded(MegaPintPackagesData.MegaPintPackageData.PackageVariation variation)
    {
        await AddEmbedded(GetPackageUrl(variation), variation.dependencies);
    }

    public static async void Remove(string packageName)
    {
        RemoveRequest request = Client.Remove(packageName);

        while (!request.IsCompleted)
            await Task.Delay(RefreshRate);

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);
        else
        {
            onSuccess?.Invoke();
            CachedPackages.Refresh();
        }
    }

    #endregion

    #region Private Methods

    private static string GetPackageUrl(MegaPintPackagesData.MegaPintPackageData package)
    {
        var devMode = MegaPintSettings.instance.GetSetting("General").GetValue("devMode", false);
        var hash = devMode ? "development" : $"v{package.version}";

        return $"{package.gitUrl}#{hash}";
    }

    private static string GetPackageUrl(MegaPintPackagesData.MegaPintPackageData.PackageVariation variation)
    {
        var devMode = MegaPintSettings.instance.GetSetting("General").GetValue("devMode", false);
        var hash = devMode ? variation.developmentBranch : $"v{variation.version}{variation.variationTag}";
        
        return $"{variation.gitUrl}#{hash}";
    }
    
    private static async Task <bool> Add(string packageUrl)
    {
        AddRequest request = Client.Add(packageUrl);

        while (!request.IsCompleted)
            await Task.Delay(RefreshRate);

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);

        return request.Status == StatusCode.Success;
    }

    private static async Task AddEmbedded(string gitUrl, List <MegaPintPackagesData.MegaPintPackageData.Dependency> dependencies)
    {
        Debug.Log($"Importing: {gitUrl}");
        
        if (dependencies is {Count: > 0})
        {
            foreach (MegaPintPackagesData.MegaPintPackageData.Dependency dependency in dependencies)
            {
                MegaPintPackagesData.MegaPintPackageData package = MegaPintPackagesData.PackageData(dependency.packageKey);

                await AddEmbedded(GetPackageUrl(package), package.dependencies);
                await Task.Delay(250);
            }
        }

        await Task.Delay(250);
        await AddEmbedded(gitUrl);

        onSuccess?.Invoke();
        CachedPackages.Refresh();
    }

    private static async Task AddEmbedded(string packageUrl)
    {
        if (!await Add(packageUrl))
            return;

        try
        {
            await Embed(packageUrl);
        }
        catch (Exception)
        {
            /* ignored */
        }
    }

    private static async Task Embed(string packageName)
    {
        EmbedRequest request = Client.Embed(packageName);

        while (!request.IsCompleted)
            await Task.Delay(RefreshRate);

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);
    }

    private static async Task <List <PackageInfo>> GetInstalledPackages()
    {
        ListRequest request = Client.List();

        while (!request.IsCompleted)
        {
            await Task.Delay(RefreshRate);

            if (CachedPackages.OnUpdateActions is not {Count: > 0})
                continue;

            foreach (CachedPackages.ListableAction action in CachedPackages.OnUpdateActions)
                action?.Invoke();
        }

        if (request.Status >= StatusCode.Failure)
            onFailure?.Invoke(request.Error.message);

        return request.Result.Where(packageInfo => packageInfo.name.ToLower().Contains("megapint")).ToList();
    }

    #endregion
}

}
#endif
