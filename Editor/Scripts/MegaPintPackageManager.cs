﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using Task = System.Threading.Tasks.Task;

namespace Editor.Scripts
{
    public class MegaPintPackageManager
    {
        private const int RefreshRate = 10;

        public static Action OnSuccess;
        public static Action<string> OnFailure;

        public static async void AddEmbedded(string packageUrl)
        {
            if (await Add(packageUrl))
                if (await Embed(packageUrl))
                    OnSuccess?.Invoke();
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
            }
            
            if (request.Status >= StatusCode.Failure)
                OnFailure?.Invoke(request.Error.message);

            return request.Result.Where(packageInfo => packageInfo.name.ToLower().Contains("megapint")).ToList();
        }

        public class CachedPackages
        {
            private readonly List<PackageCache> _packages = new ();

            public CachedPackages(Action action) => Initialize(action);

            private async void Initialize(Action action)
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

                action?.Invoke();
            }

            public struct PackageCache
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