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
        private const int LoadingLabelRefreshRate = 50;

        private static int _currentLoadingLabelProgress;
        
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

        private static async Task<List<PackageInfo>> GetInstalledPackages(Label loadingLabel)
        {
            if (loadingLabel != null)
                loadingLabel.style.display = DisplayStyle.Flex;
            
            var request = Client.List();
            while (!request.IsCompleted)
            {
                await Task.Delay(RefreshRate);

                if (loadingLabel == null) 
                    continue;
                
                if (_currentLoadingLabelProgress >= LoadingLabelRefreshRate)
                {
                    _currentLoadingLabelProgress = 0;

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
                    _currentLoadingLabelProgress += RefreshRate;
            }
            
            if (loadingLabel != null)
                loadingLabel.style.display = DisplayStyle.Flex;
            
            if (request.Status >= StatusCode.Failure)
                OnFailure?.Invoke(request.Error.message);

            return request.Result.Where(packageInfo => packageInfo.name.ToLower().Contains("megapint")).ToList();
        }

        public class CachedPackages
        {
            private static CachedPackages _allPackages;

            public static Action<CachedPackages> OnRefreshed;

            public static void AllPackages(Label loadingLabel, Action<CachedPackages> action)
            {
                if (_allPackages == null)
                {
                    var newInstance = new CachedPackages(loadingLabel, action);
                    _allPackages = newInstance;
                }
                
                action?.Invoke(_allPackages);
            }

            public static void Refresh()
            {
                var _ = new CachedPackages(null, packages =>
                {
                    _allPackages = packages;
                    OnRefreshed?.Invoke(_allPackages);
                });
            }

            private readonly List<PackageCache> _packages = new ();

            private CachedPackages(Label loadingLabel, Action<CachedPackages> action) => Initialize(loadingLabel, action);

            private async void Initialize(Label loadingLabel, Action<CachedPackages> action)
            {
                var allPackages = MegaPintPackagesData.Packages;
                var installedPackages = await GetInstalledPackages(loadingLabel);
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

                action?.Invoke(_allPackages);
            }

            private struct PackageCache
            {
                public MegaPintPackagesData.PackageKey Key;
                public bool Installed;
                public bool NewestVersion;
                public string CurrentVersion;
            }

            public bool IsImported(MegaPintPackagesData.PackageKey key) =>
                _packages.First(package => package.Key == key).Installed;

            public bool NeedsUpdate(MegaPintPackagesData.PackageKey key) =>
                !_packages.First(package => package.Key == key).NewestVersion;

            public List<MegaPintPackagesData.MegaPintPackageData> ToDisplay() =>
                _packages.Select(package => MegaPintPackagesData.PackageData(package.Key)).ToList();

            public string CurrentVersion(MegaPintPackagesData.PackageKey key) =>
                _packages.First(package => package.Key == key).CurrentVersion;
        }
    }
}
#endif