#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts
{

internal static class Utility
{
    #region Public Methods

    public static T CopyAndLoadAsset <T>(T oldAsset, string newPath) where T : Object
    {
        return CopyAndLoadAsset <T>(AssetDatabase.GetAssetPath(oldAsset), newPath);
    }

    public static T CopyAndLoadAsset <T>(string oldPath, string newPath) where T : Object
    {
        AssetDatabase.CopyAsset(oldPath, newPath);

        return AssetDatabase.LoadAssetAtPath <T>(newPath);
    }

    public static bool IsPathInProject(this string source, out string pathInProject)
    {
        var valid = source.StartsWith(Application.dataPath);
        pathInProject = valid ? source[(Application.dataPath.Length - 6)..] : "";

        return valid;
    }

    #endregion
}

}
#endif
