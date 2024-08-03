#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace MegaPint.Editor.Scripts
{

/// <summary> Class containing general utility functions </summary>
internal static class Utility
{
    [Serializable]
    private class Result
    {
        public bool exists;
    }

    private const string ValidateTokenURL = "https://tiogiras.games/checkToken.php";

    public static Action onTesterTokenValidated;

    private static bool s_validTesterToken;
    private static bool s_tokenValidated;

    #region Public Methods

    /// <summary> Clone a serializable object </summary>
    /// <param name="input"> Source object to clone </param>
    /// <typeparam name="T"> Serializable Object </typeparam>
    /// <returns> Cloned object </returns>
    public static T Clone <T>(this T input)
    {
        Type type = input.GetType();

        FieldInfo[] fields = type.GetFields(
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        var clonedObj = (T)Activator.CreateInstance(type);

        foreach (FieldInfo field in fields)
        {
            var value = field.GetValue(input);
            field.SetValue(clonedObj, value);
        }

        return clonedObj;
    }

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
            "Editor",
            "Resources",
            "MegaPint",
            key.ToString(),
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

    public static async Task <bool> IsValidTesterToken()
    {
        if (s_tokenValidated)
            return s_validTesterToken;

        return await ValidateTesterToken();
    }

    /// <summary> Validate the saved tester token </summary>
    /// <returns> If the token is valid </returns>
    public static async Task <bool> ValidateTesterToken()
    {
        var token = SaveValues.BasePackage.TesterToken;

        if (string.IsNullOrEmpty(token))
        {
            s_tokenValidated = true;
            s_validTesterToken = false;
            onTesterTokenValidated?.Invoke();
            
            return false;
        }

        UnityWebRequest request =
            UnityWebRequest.Get(
                $"{ValidateTokenURL}?token={UnityWebRequest.EscapeURL(SaveValues.BasePackage.TesterToken)}");

        request.SendWebRequest();

        while (!request.isDone)
            await Task.Delay(100);

        var returnValue = false;

        switch (request.result)
        {
            case UnityWebRequest.Result.InProgress:
                break;

            case UnityWebRequest.Result.Success:
                var response = request.downloadHandler.text;
                var result = JsonUtility.FromJson <Result>(response);

                returnValue = result.exists;

                break;

            case UnityWebRequest.Result.ConnectionError:
                Debug.LogError(
                    "MegaPint could not connect to the internet to validate the tester token. Please check your connection and try again.");

                break;

            case UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError:
                Debug.LogError(
                    $"MegaPint could not validate the tester token. Please try again later.\n{request.error}");

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        s_tokenValidated = true;
        s_validTesterToken = returnValue;
        onTesterTokenValidated?.Invoke();

        return returnValue;
    }

    #endregion
}

}
#endif
