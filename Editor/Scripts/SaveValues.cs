﻿using Editor.Scripts.Settings;
using UnityEditor;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace Editor.Scripts
{

public static partial class SaveValues
{
    private static MegaPintSettingsBase s_generalSettings;
    
    private static MegaPintSettingsBase _GeneralSettings => s_generalSettings ??= MegaPintSettings.instance.GetSetting("General");
    
    public static class BasePackage
    {
        public static int EditorTheme
        {
            get => _GeneralSettings.GetValue("EditorTheme", 0);
            set
            {
                _GeneralSettings.SetValue("EditorTheme", value);

                GUIUtility.ForceRepaint();
            }
        }
        
        public static bool IsDarkTheme => EditorTheme == 1 || (EditorTheme == 0 && EditorGUIUtility.isProSkin);

        public static bool DevMode
        {
            get => _GeneralSettings.GetValue("DevMode", false);
            set => _GeneralSettings.SetValue("DevMode", value);
        }
    }
}

}
