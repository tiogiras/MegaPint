#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using Task = System.Threading.Tasks.Task;

namespace Editor.Scripts.PackageManager
{
    public static class MegaPintPackageManager
    {
        private const int RefreshRate = 10;

        public static Action OnSuccess;
        public static Action<string> OnFailure;

        public static async void AddEmbedded(string packageUrl)
        {
            if (!await Add(packageUrl))
                return;

            if (!await Embed(packageUrl)) 
                return;
            
            OnSuccess?.Invoke();  
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
                OnFailure?.Invoke(request.Error.message);
            else
            {
                OnSuccess?.Invoke();   
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
                OnFailure?.Invoke(request.Error.message);

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
                OnFailure?.Invoke(request.Error.message);
            
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
                OnFailure?.Invoke(request.Error.message);

            return request.Result.Where(packageInfo => packageInfo.name.ToLower().Contains("megapint")).ToList();
        }

        public class CachedPackages
        {
            private static CachedPackages _allPackages;
            
            private readonly List<PackageCache> _packages = new ();

            #region Actions

            public static List<ListableAction> OnUpdateActions = new();
            public static List<ListableAction<CachedPackages>> OnCompleteActions = new();

            public class ListableAction
            {
                public string Name;
                private Action _action;

                public ListableAction(Action action, string name)
                {
                    _action = action;
                    Name = name;
                }

                public void Invoke() => _action.Invoke();
            }

            public class ListableAction<T>
            {
                public string Name;
                private Action<T> _action;

                public ListableAction(Action<T> action, string name)
                {
                    _action = action;
                    Name = name;
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
                    if (!action.Name.Equals(name))
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
                    if (!action.Name.Equals(name))
                        continue;
                    
                    OnCompleteActions.RemoveAt(i);
                    return;
                }
            }
            
            #endregion

            #region Public Methods

            public static void RequestAllPackages()
            {
                if (_allPackages == null)
                {
                    var newInstance = new CachedPackages();
                    _allPackages = newInstance;
                }
                else
                {
                    if (OnCompleteActions is not {Count: > 0})
                        return;
                    
                    foreach (var action in OnCompleteActions)
                    {
                        action?.Invoke(_allPackages);
                    }
                }
            }

            public static void Refresh()
            {
                _allPackages = null;
                RequestAllPackages();
            }
            
            public bool IsImported(MegaPintPackagesData.PackageKey key) =>
                _packages.First(package => package.Key == key).Installed;

            public bool NeedsUpdate(MegaPintPackagesData.PackageKey key) =>
                !_packages.First(package => package.Key == key).NewestVersion;

            public List<MegaPintPackagesData.MegaPintPackageData> ToDisplay() =>
                _packages.Select(package => MegaPintPackagesData.PackageData(package.Key)).ToList();

            public string CurrentVersion(MegaPintPackagesData.PackageKey key) =>
                _packages.First(package => package.Key == key).CurrentVersion;
            
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
                    var installed = installedPackagesNames.Contains(package.PackageName);
                    var newestVersion = false;
                    var currentVersion = "";
                    
                    if (installed)
                    {
                        var index = installedPackagesNames.IndexOf(package.PackageName);
                        newestVersion = installedPackages[index].version == package.Version;
                        currentVersion = installedPackages[index].version;
                    }

                    _packages.Add(new PackageCache
                    {
                        Key = package.PackageKey,
                        Installed = installed,
                        NewestVersion = newestVersion,
                        CurrentVersion = currentVersion
                    });
                }

                if (OnCompleteActions is not {Count: > 0})
                    return;
                
                foreach (var action in OnCompleteActions)
                {
                    action?.Invoke(_allPackages);
                }
            }

            private struct PackageCache
            {
                public MegaPintPackagesData.PackageKey Key;
                public bool Installed;
                public bool NewestVersion;
                public string CurrentVersion;
            }

            #endregion
        }
    }
}
#endif