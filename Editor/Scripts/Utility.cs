#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor.Scripts
{

/// <summary> Class containing general utility functions </summary>
internal static class Utility
{
    #region Public Methods

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
    
    public static T CopyAndLoadAsset <T>(T oldAsset, string newPath) where T : Object
    {
        return CopyAndLoadAsset <T>(AssetDatabase.GetAssetPath(oldAsset), newPath);
    }

    public static T CopyAndLoadAsset <T>(string oldPath, string newPath) where T : Object
    {
        AssetDatabase.CopyAsset(oldPath, newPath);

        return AssetDatabase.LoadAssetAtPath <T>(newPath);
    }

    #endregion
}

}
#endif
