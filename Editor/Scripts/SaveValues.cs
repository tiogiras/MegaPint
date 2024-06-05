#if UNITY_EDITOR
using System;
using MegaPint.Editor.Scripts.GUI.Utility;
using MegaPint.Editor.Scripts.Settings;
using UnityEditor;

namespace MegaPint.Editor.Scripts
{

/// <summary> Partial class storing saveData values (BasePackage) </summary>
internal static partial class SaveValues
{
    public static class BasePackage
    {
        private static CacheValue <int> s_editorTheme = new() {defaultValue = 0};
        private static CacheValue <bool> s_devMode = new() {defaultValue = false};

        public static int EditorTheme
        {
            get => ValueProperty.Get("EditorTheme", ref s_editorTheme, _GeneralSettings);
            set
            {
                ValueProperty.Set("EditorTheme", value, ref s_editorTheme, _GeneralSettings);
                
                GUIUtility.ForceRepaint();
            }
        }

        public static bool IsDarkTheme => EditorTheme == 1 || (EditorTheme == 0 && EditorGUIUtility.isProSkin);

        public static bool DevMode
        {
            get => ValueProperty.Get("DevMode", ref s_devMode, _GeneralSettings);
            set => ValueProperty.Set("DevMode", value, ref s_devMode, _GeneralSettings);
        }
    }

    /// <summary> Utility class to access save values and cache them to reduce accessing the savedata </summary>
    private static class ValueProperty
    {
        #region Public Methods

        public static bool Get(string key, ref CacheValue <bool> cacheValue, SettingsBase settings)
        {
            if (cacheValue.wasInitialized)
                return cacheValue.value;

            if (settings == null)
                return cacheValue.defaultValue;

            cacheValue.value = settings.GetValue(key, cacheValue.defaultValue);
            cacheValue.wasInitialized = true;

            return cacheValue.value;
        }

        public static int Get(string key, ref CacheValue <int> cacheValue, SettingsBase settings)
        {
            if (cacheValue.wasInitialized)
                return cacheValue.value;

            if (settings == null)
                return cacheValue.defaultValue;

            cacheValue.value = settings.GetValue(key, cacheValue.defaultValue);
            cacheValue.wasInitialized = true;

            return cacheValue.value;
        }

        public static float Get(string key, ref CacheValue <float> cacheValue, SettingsBase settings)
        {
            if (cacheValue.wasInitialized)
                return cacheValue.value;

            if (settings == null)
                return cacheValue.defaultValue;

            cacheValue.value = settings.GetValue(key, cacheValue.defaultValue);
            cacheValue.wasInitialized = true;

            return cacheValue.value;
        }

        public static string Get(string key, ref CacheValue <string> cacheValue, SettingsBase settings)
        {
            if (cacheValue.wasInitialized)
                return cacheValue.value;

            if (settings == null)
                return cacheValue.defaultValue;

            cacheValue.value = settings.GetValue(key, cacheValue.defaultValue);
            cacheValue.wasInitialized = true;

            return cacheValue.value;
        }

        public static void Set(string key, bool value, ref CacheValue <bool> cacheValue, SettingsBase settings)
        {
            if (cacheValue.value == value)
                return;

            cacheValue.value = value;
            settings?.SetValue(key, value);
        }

        public static void Set(string key, int value, ref CacheValue <int> cacheValue, SettingsBase settings)
        {
            if (cacheValue.value == value)
                return;

            cacheValue.value = value;
            settings?.SetValue(key, value);
        }

        public static void Set(string key, float value, ref CacheValue <float> cacheValue, SettingsBase settings)
        {
            if (Math.Abs(cacheValue.value - value) < 0)
                return;

            cacheValue.value = value;
            settings?.SetValue(key, value);
        }

        public static void Set(string key, string value, ref CacheValue <string> cacheValue, SettingsBase settings)
        {
            if (cacheValue.value.Equals(value))
                return;

            cacheValue.value = value;
            settings?.SetValue(key, value);
        }

        #endregion
    }

    private struct CacheValue <T>
    {
        public T value;
        public T defaultValue;
        public bool wasInitialized;
    }

    private static SettingsBase s_generalSettings;

    private static SettingsBase _GeneralSettings
    {
        get
        {
            if (MegaPintSettings.Exists())
                return s_generalSettings ??= MegaPintSettings.instance.GetSetting("General");

            return null;
        }
    }
}

}
#endif
