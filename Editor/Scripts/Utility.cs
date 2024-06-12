#if UNITY_EDITOR
using System.IO;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor.Scripts
{

/// <summary> Class containing general utility functions </summary>
internal static class Utility
{
    #region Public Methods

    /// <summary> Combine the given strings like Path.Combine but for menuItem execution </summary>
    /// <param name="arg0"> First part of the path </param>
    /// <param name="arg1"> Second part of the path </param>
    /// <returns> Combined path suitable for menuItems </returns>
    public static string CombineMenuItemPath(string arg0, string arg1)
    {
        var path = Path.Combine(arg0, arg1);

        return path.Replace("\\", "/");
    }

    /// <summary> Copy and load an asset used in different render pipelines </summary>
    /// <param name="oldAsset"> Source asset </param>
    /// <param name="newPath"> New destination </param>
    /// <typeparam name="T"> Type of the asset </typeparam>
    /// <returns> New asset </returns>
    public static T CopyAndLoadAsset <T>(T oldAsset, string newPath) where T : Object
    {
        return CopyAndLoadAsset <T>(AssetDatabase.GetAssetPath(oldAsset), newPath);
    }

    /// <summary> Copy and load an asset used in different render pipelines </summary>
    /// <param name="oldPath"> Source asset </param>
    /// <param name="newPath"> New destination </param>
    /// <typeparam name="T"> Type of the asset </typeparam>
    /// <returns> New asset </returns>
    public static T CopyAndLoadAsset <T>(string oldPath, string newPath) where T : Object
    {
        AssetDatabase.CopyAsset(oldPath, newPath);

        return AssetDatabase.LoadAssetAtPath <T>(newPath);
    }

    /// <summary> Get the path to the sample of a specified package </summary>
    /// <param name="key"> Key to the targeted package </param>
    /// <param name="localSamplePath"> Path to the sample unityPackage inside the sample folder of the package </param>
    /// <returns> Full path to the sample unityPackage </returns>
    public static string GetPackageSamplePath(PackageKey key, string localSamplePath)
    {
        return Path.Combine(
            Application.dataPath[..^7],
            "Packages",
            PackageCache.Get(key).Name,
            "Samples",
            $"{localSamplePath}.unitypackage");
    }

    /// <summary> Check if the string is a path inside the unity assets folder </summary>
    /// <param name="source"> String to be checked </param>
    /// <param name="pathInProject"> If the string is a path inside the project use this path </param>
    /// <returns> If the string is a path in the project </returns>
    public static bool IsPathInProject(this string source, out string pathInProject)
    {
        var valid = source.StartsWith(Application.dataPath);
        pathInProject = valid ? source[(Application.dataPath.Length - 6)..] : "";

        return valid;
    }

    /// <summary> Check if the current project is the unity project the product is created with </summary>
    /// <returns> If the current project is the production project </returns>
    public static bool IsProductionProject()
    {
        return Application.companyName.Equals("Tiogiras") && Application.productName.Equals("MegaPintProject");
    }

    #endregion
}

}
#endif
