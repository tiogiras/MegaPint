#if UNITY_EDITOR
using Editor.Scripts.Settings;
using UnityEngine;

namespace Editor.Scripts.DevModeUtil
{

internal static class DevLog
{
    private static MegaPintSettingsBase s_settings;
    
    private static bool IsDevMode()
    {
        s_settings ??= MegaPintSettings.instance.GetSetting("General");
        return s_settings.GetValue("devMode", false);
    }
    
    public static void Log<T>(T caller, string msg)
    {
        if (IsDevMode())
            Debug.Log($"[{caller.GetType()}]: {msg}");
    }
    
    public static void LogWarning<T>(T caller, string msg)
    {
        if (IsDevMode())
            Debug.LogWarning($"[{caller.GetType()}]: {msg}");
    }
    
    public static void LogError<T>(T caller, string msg)
    {
        if (IsDevMode())
            Debug.LogError($"[{caller.GetType()}]: {msg}");
    }
}

}
#endif
