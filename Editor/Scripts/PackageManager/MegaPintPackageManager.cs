#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using Task = System.Threading.Tasks.Task;

namespace Editor.Scripts.PackageManager
{
    public static class MegaPintPackageManager
    {
        private const int RefreshRate = 10;

        public static Action onSuccess;
        public static Action<string> onFailure;

        public static async void AddEmbedded(string packageUrl)
        {
            if (!await Add(packageUrl))
                return;

            try
            {
                if (!await Embed(packageUrl)) 
                    return;
            }
            catch (Exception)
            {
                // ignored
            }

            onSuccess?.Invoke();  
            CachedPackages.Refresh();
        }

        public static async void Remove(string packageName)
        {
            var request = Client.Remove(packageName);
            while (!request.IsCompleted)
            {
                await Task.Delay(RefreshRate);
            }
            
            if (request.Status >= StatusCode.Failure)
                onFailure?.Invoke(request.Error.message);
            else
            {
                onSuccess?.Invoke();   
                CachedPackages.Refresh();
            }
        }
        
        private static async Task<bool> Add(string packageUrl)
        {
            var request = Client.Add(packageUrl);
            while (!request.IsCompleted)
            {
                await Task.Delay(RefreshRate);
            }

            if (request.Status >= StatusCode.Failure)
                onFailure?.Invoke(request.Error.message);

            return request.Status == StatusCode.Success;
        }

        private static async Task<bool> Embed(string packageName)
        {
            var request = Client.Embed(packageName);
            while (!request.IsCompleted)
            {
                await Task.Delay(RefreshRate);
            }
            
            if (request.Status >= StatusCode.Failure)
                onFailure?.Invoke(request.Error.message);
            
            return request.Status == StatusCode.Success;
        }

        private static async Task<List<PackageInfo>> GetInstalledPackages()
        {
            var request = Client.List();
            while (!request.IsCompleted)
            {
                await Task.Delay(RefreshRate);

                if (CachedPackages.OnUpdateActions is not {Count: > 0})
                    continue;

                foreach (var action in CachedPackages.OnUpdateActions)
                {
                    action?.Invoke();
                }
            }

            if (request.Status >= StatusCode.Failure)
                onFailure?.Invoke(request.Error.message);

            return request.Result.Where(packageInfo => packageInfo.name.ToLower().Contains("megapint")).ToList();
        }

        public class CachedPackages
        {
            private static CachedPackages s_allPackages;
            
            private readonly List<PackageCache> _packages = new ();

            #region Actions

            public static readonly List<ListableAction> OnUpdateActions = new();
            public static readonly List<ListableAction<CachedPackages>> OnCompleteActions = new();

            public class ListableAction
            {
                public readonly string name;
                private readonly Action _action;

                public ListableAction(Action action, string name)
                {
                    _action = action;
                    this.name = name;
                }

                public void Invoke() => _action.Invoke();
            }

            public class ListableAction<T>
            {
                public readonly string name;
                private readonly Action<T> _action;

                public ListableAction(Action<T> action, string name)
                {
                    _action = action;
                    this.name = name;
                }

                public void Invoke(T value) => _action.Invoke(value);
            }
            
            public static void RemoveUpdateAction(string name)
            {
                if (OnUpdateActions is not {Count: > 0})
                    return;

                for (var i = 0; i < OnUpdateActions.Count; i++)
                {
                    var action = OnUpdateActions[i];
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
                    var action = OnCompleteActions[i];
                    if (!action.name.Equals(name))
                        continue;
                    
                    OnCompleteActions.RemoveAt(i);
                    return;
                }
            }
            
            #endregion

            #region Public Methods

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
                    
                    foreach (var action in OnCompleteActions)
                    {
                        action?.Invoke(s_allPackages);
                    }
                }
            }

            public static void Refresh()
            {
                s_allPackages = null;
                RequestAllPackages();
            }
            
            public bool IsImported(MegaPintPackagesData.PackageKey key) =>
                _packages.First(package => package.key == key).installed;

            public bool NeedsUpdate(MegaPintPackagesData.PackageKey key) =>
                !_packages.First(package => package.key == key).newestVersion;
            
            public bool IsVariation(MegaPintPackagesData.PackageKey key, string gitHash)
            {
                return _packages.First(package => package.key == key).currentVariation.Equals(gitHash);
            }

            public bool NeedsVariationUpdate(MegaPintPackagesData.PackageKey key, string niceName)
            {
                Debug.Log(niceName);

                PackageCache package = _packages.First(package => package.key == key); // <- package.variations is null
                
                foreach (var v in package.variations)   
                {
                    Debug.Log(v.niceName);
                }

                Debug.Log("CACHE END");
                
                VariationsCache variation = package.variations.First(variation => variation.niceName.Equals(niceName));

                return !variation.newestVersion;
            }
            
            public List<MegaPintPackagesData.MegaPintPackageData> ToDisplay() =>
                _packages.Select(package => MegaPintPackagesData.PackageData(package.key)).ToList();

            public string CurrentVersion(MegaPintPackagesData.PackageKey key) =>
                _packages.First(package => package.key == key).currentVersion;

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
                    {
                        loadingText.Append(".");
                    }

                    loadingLabel.style.display = DisplayStyle.Flex;
                    loadingLabel.text = loadingText.ToString();
                }
                else
                    currentProgress += RefreshRate;

                newProgress = currentProgress;
            }

            #endregion

            #region Internal Methods

            private CachedPackages() => Initialize();

            private async void Initialize()
            {
                var allPackages = MegaPintPackagesData.Packages;
                var installedPackages = await GetInstalledPackages();
                var installedPackagesNames = installedPackages.Select(installedPackage => installedPackage.name).ToList();

                foreach (var package in allPackages)
                {
                    // TODO Variations
                    var installed = installedPackagesNames.Contains(package.packageName);
                    var newestVersion = false;
                    var currentVersion = "";
                    var hash = "";

                    List <VariationsCache> variations = null;
                    
                    if (installed)
                    {
                        PackageInfo installedPackage = installedPackages[installedPackagesNames.IndexOf(package.packageName)];
                        newestVersion = installedPackage.version == package.version;
                        currentVersion = installedPackage.version;
                        hash = installedPackage.git?.hash;

                        if (package.variations is {Count: > 0})
                        {
                            variations = new List <VariationsCache>();
                        
                            foreach (MegaPintPackagesData.MegaPintPackageData.PackageVariation variation in package.variations)
                            {
                                variations.Add(new VariationsCache
                                {
                                    niceName = variation.niceName,
                                    newestVersion = installedPackage.version == variation.version,
                                    currentVersion = installedPackage.version
                                });   
                            }
                        }
                    }

                    _packages.Add(new PackageCache
                    {
                        key = package.packageKey,
                        installed = installed,
                        newestVersion = newestVersion,
                        currentVersion = currentVersion,
                        currentVariation = hash,
                        variations = variations
                    });
                }

                if (OnCompleteActions is not {Count: > 0})
                    return;
                
                foreach (var action in OnCompleteActions)
                {
                    action?.Invoke(s_allPackages);
                }
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

            public struct VariationsCache
            {
                public string niceName;
                public bool newestVersion;
                public string currentVersion;
            }

            #endregion
        }
    }
}
#endif