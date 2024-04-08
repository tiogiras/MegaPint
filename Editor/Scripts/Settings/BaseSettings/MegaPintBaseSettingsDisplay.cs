#if UNITY_EDITOR
using Editor.Scripts.Windows;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = Editor.Scripts.GUI.GUIUtility;

namespace Editor.Scripts.Settings.BaseSettings
{
    internal static class MegaPintBaseSettingsDisplay
    {
        private const string BasePath = "MegaPint/User Interface/Info Content/xxx";

        private static VisualElement s_element;
        
        public static void Display(VisualElement root, MegaPintBaseSettingsData.SettingKey key)
        {
            s_element = Load(key).Instantiate();
            GUIUtility.ApplyTheme(s_element);
            
            root.Add(s_element);

            ActivateLogic(key, root);
        }

        private static VisualTreeAsset Load(MegaPintBaseSettingsData.SettingKey key)
            => Resources.Load<VisualTreeAsset>(GetDisplayPath(key));

        private static string GetDisplayPath(MegaPintBaseSettingsData.SettingKey key)
            => BasePath.Replace("xxx", key.ToString());
        
        private static void ActivateLogic(MegaPintBaseSettingsData.SettingKey key, VisualElement root)
        {
            switch (key)
            {
                case MegaPintBaseSettingsData.SettingKey.Contact: ContactLogic(root); break;
                case MegaPintBaseSettingsData.SettingKey.ManagePackages: break;
                case MegaPintBaseSettingsData.SettingKey.UsePackages: break;
                case MegaPintBaseSettingsData.SettingKey.UpdateBasePackage: break;
                default: return;
            }
        }

        private static void ContactLogic(VisualElement root)
        {
            GUIUtility.ActivateLinks(
                root,
                evt =>
                {
                    switch (evt.linkID)
                    {
                        case "discord":
                            Application.OpenURL("https://discord.com/users/282542538819108866");

                            break;

                        case "email":
                            Application.OpenURL("tiogiras@gmail.com");

                            break;

                        case "website":
                            Application.OpenURL("https://tiogiras.games");

                            break;
                    }
                });
        }
    }
}
#endif