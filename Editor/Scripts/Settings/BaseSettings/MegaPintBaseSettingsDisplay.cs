#if UNITY_EDITOR
using Editor.Scripts.Windows;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Settings.BaseSettings
{
    public static class MegaPintBaseSettingsDisplay
    {
        //TODO actual content for the interactablility if needed
        //TODO use actions for registering and unregistering of the callbacks

        private const string BasePath = "User Interface/MegaPint Base Settings/xxxDisplay";

        private static VisualElement _element;
        
        public static void Display(VisualElement root, MegaPintBaseSettingsData.SettingKey key)
        {
            _element = Load(key).Instantiate();
            root.Add(_element);

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
                case MegaPintBaseSettingsData.SettingKey.Contact:

                    RegisterCallbacksContact();
                    
                    break;
                
                case MegaPintBaseSettingsData.SettingKey.ManagePackages:
                    break;
                
                case MegaPintBaseSettingsData.SettingKey.UsePackages:
                    break;
                
                default: return;
            }
        }

        #region Contact

        private static void RegisterCallbacksContact()
        {
            MegaPintBaseWindow.OnRightPaneClose += UnRegisterCallbacksContact;

            _element.Q<Button>("BTN_Mail").clicked += ContactMail;
            _element.Q<Button>("BTN_Discord1").clicked += ContactDiscord;
            _element.Q<Button>("BTN_Discord2").clicked += ContactDiscord;
            _element.Q<Button>("BTN_Website").clicked += ContactWebsite;
        }
        
        private static void UnRegisterCallbacksContact()
        {
            MegaPintBaseWindow.OnRightPaneClose -= UnRegisterCallbacksContact;
            
            _element.Q<Button>("BTN_Mail").clicked -= ContactMail;
            _element.Q<Button>("BTN_Discord1").clicked -= ContactDiscord;
            _element.Q<Button>("BTN_Discord2").clicked -= ContactDiscord;
            _element.Q<Button>("BTN_Website").clicked -= ContactWebsite;
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