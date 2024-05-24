#if UNITY_EDITOR
using Editor.Scripts.Settings;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace Editor.Scripts.Windows.BaseWindowContent.SettingsTabContent
{
    internal static class SettingsTabDisplay
    {
        private const string BasePath = "MegaPint/User Interface/Settings Content/xxx";

        private static int _editorThemeIndex = -1;
        
        public static void Display(VisualElement root, SettingsTabData.SettingsKey key)
        {
            GUIUtility.Instantiate(Load(key), root);
            ActivateLogic(key, root);
        }

        private static VisualTreeAsset Load(SettingsTabData.SettingsKey key)
            => Resources.Load<VisualTreeAsset>(GetDisplayPath(key));

        private static string GetDisplayPath(SettingsTabData.SettingsKey key)
            => BasePath.Replace("xxx", key.ToString());
        
        private static void ActivateLogic(SettingsTabData.SettingsKey key, VisualElement root)
        {
            switch (key)
            {
                case SettingsTabData.SettingsKey.Theme:
                    ThemeLogic(root);
                    break;
                
                default: return;
            }
        }

        private static void ThemeLogic(VisualElement root)
        {
            var dropdown = root.Q <DropdownField>("EditorTheme");

            if (_editorThemeIndex < 0)
                _editorThemeIndex = SaveValues.BasePackage.EditorTheme;

            dropdown.SetValueWithoutNotify(dropdown.choices[_editorThemeIndex]);

            dropdown.RegisterValueChangedCallback(
                _ =>
                {
                    _editorThemeIndex = dropdown.index;
                    SaveValues.BasePackage.EditorTheme = dropdown.index;
                });
        }
    }
}
#endif