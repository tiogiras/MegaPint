#if UNITY_EDITOR
using UnityEngine;

namespace MegaPint.Editor.Scripts.DevMode
{

/// <summary> Utility class to log messages only when the development mode is activated. </summary>
internal static class DevLog
{
    #region Public Methods

    /// <summary> Log a message if the devMode is enabled </summary>
    /// <param name="msg"> Message to be logged </param>
    public static void Log(string msg)
    {
        if (SaveValues.BasePackage.DevMode)
            Debug.Log($"[DevMode]: {msg}");
    }

    /// <summary> Log an errorMessage if the devMode is enabled </summary>
    /// <param name="msg"> Message to be logged </param>
    public static void LogError(string msg)
    {
        if (SaveValues.BasePackage.DevMode)
            Debug.LogError($"[DevMode]: {msg}");
    }

    /// <summary> Log a warningMessage if the devMode is enabled </summary>
    /// <param name="msg"> Message to be logged </param>
    public static void LogWarning(string msg)
    {
        if (SaveValues.BasePackage.DevMode)
            Debug.LogWarning($"[DevMode]: {msg}");
    }

    #endregion
}

}
#endif
