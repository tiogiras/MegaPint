#if UNITY_EDITOR
using Editor.Scripts.Windows;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Settings.BaseSettings
{
    public static class MegaPintBaseSettingsDisplay
    {
        private const string BasePath = "User Interface/MegaPint Base Settings/xxxDisplay";

        private static VisualElement s_element;
        
        public static void Display(VisualElement root, MegaPintBaseSettingsData.SettingKey key)
        {
            s_element = Load(key).Instantiate();
            root.Add(s_element);

            ActivateLogic(key);
        }

        private static VisualTreeAsset Load(MegaPintBaseSettingsData.SettingKey key)
            => Resources.Load<VisualTreeAsset>(GetDisplayPath(key));

        private static string GetDisplayPath(MegaPintBaseSettingsData.SettingKey key)
            => BasePath.Replace("xxx", key.ToString());
        
        private static void ActivateLogic(MegaPintBaseSettingsData.SettingKey key)
        {
            switch (key)
            {
                case MegaPintBaseSettingsData.SettingKey.Contact: RegisterCallbacksContact(); break;
                case MegaPintBaseSettingsData.SettingKey.ManagePackages: break;
                case MegaPintBaseSettingsData.SettingKey.UsePackages: break;
                case MegaPintBaseSettingsData.SettingKey.UpdateBasePackage: break;
                default: return;
            }
        }

        #region Contact

        private static void RegisterCallbacksContact()
        {
            MegaPintBaseWindow.onRightPaneClose += UnRegisterCallbacksContact;

            s_element.Q<Button>("BTN_Mail").clicked += ContactMail;
            s_element.Q<Button>("BTN_Discord1").clicked += ContactDiscord;
            s_element.Q<Button>("BTN_Discord2").clicked += ContactDiscord;
            s_element.Q<Button>("BTN_Website").clicked += ContactWebsite;
        }
        
        private static void UnRegisterCallbacksContact()
        {
            MegaPintBaseWindow.onRightPaneClose -= UnRegisterCallbacksContact;
            
            s_element.Q<Button>("BTN_Mail").clicked -= ContactMail;
            s_element.Q<Button>("BTN_Discord1").clicked -= ContactDiscord;
            s_element.Q<Button>("BTN_Discord2").clicked -= ContactDiscord;
            s_element.Q<Button>("BTN_Website").clicked -= ContactWebsite;
        }

        #region Callbacks

        private static void ContactMail()
        {
            Application.OpenURL("tiogiras@gmail.com");
        }
        
        private static void ContactDiscord()
        {
            Application.OpenURL("https://discord.com/users/282542538819108866");
        }

        private static void ContactWebsite()
        {
            Application.OpenURL("https://tiogiras.games");
        }

        #endregion

        #endregion
    }
}
#endif