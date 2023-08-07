using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Editor.Scripts
{
    public class MegaPintPackageManager
    {
        private const int RefreshRate = 10;

        public static Action OnSuccess;
        public static Action<string> OnFailure;

        public static async void AddEmbedded(string packageName)
        {
            if (await Add(packageName))
                if (await Embed(packageName))
                    OnSuccess?.Invoke();
        }

        private static async Task<bool> Add(string packageName)
        {
            var request = Client.Add(packageName);
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

        public class CachedPackageProperties
        {
            private List<bool> _correctVersions = new();

            public CachedPackageProperties()
            {
                Initialize();
            }

            private async void Initialize()
            {
                var installedPackages = await GetInstalledPackages();

                foreach (var installedPackage in installedPackages)
                {
                    Debug.Log(installedPackage.name);
                }
            }

            public struct Properties
            {
                public bool Installed;
                public bool NewestVersion;
            }
        }
    }
}