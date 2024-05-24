#if UNITY_EDITOR
using MegaPint.Editor.Scripts.GUI.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace Editor.Scripts.Windows.BaseWindowContent.InfoTabContent
{
    internal static class InfosTabDisplay
    {
        private const string BasePath = "MegaPint/User Interface/Info Content/xxx";
        
        public static void Display(VisualElement root, InfosTabData.InfoKey key)
        {
            GUIUtility.Instantiate(Load(key), root);
            ActivateLogic(key, root);
        }

        private static VisualTreeAsset Load(InfosTabData.InfoKey key)
            => Resources.Load<VisualTreeAsset>(GetDisplayPath(key));

        private static string GetDisplayPath(InfosTabData.InfoKey key)
            => BasePath.Replace("xxx", key.ToString());
        
        private static void ActivateLogic(InfosTabData.InfoKey key, VisualElement root)
        {
            switch (key)
            {
                case InfosTabData.InfoKey.Contact: 
                    ContactLogic(root);

                    break;
                case InfosTabData.InfoKey.ManagePackages: 
                    ManagePackagesLogic(root);

                    break;
                case InfosTabData.InfoKey.UsePackages: 
                    
                    break;
                
                case InfosTabData.InfoKey.UpdateBasePackage: 
                    UpdateBasePackageLogic(root);

                    break;

                case InfosTabData.InfoKey.Shortcuts:
                    ShortcutsLogic(root);

                    break;
                
                default: return;
            }
        }

        private static void ContactLogic(VisualElement root)
        {
            root.ActivateLinks(evt =>
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
        
        private static void ManagePackagesLogic(VisualElement root)
        {
            root.ActivateLinks(evt =>
            {
                switch (evt.linkID)
                {
                    case "github":
                        Application.OpenURL("https://github.com/tiogiras/MegaPint");
                        break;
                        
                    case "MegaPint/PackageManager":
                        EditorApplication.ExecuteMenuItem(evt.linkID);
                        break;
                        
                    case "Window/Package Manager":
                        EditorApplication.ExecuteMenuItem(evt.linkID);
                        break;
                        
                    case "website":
                        Application.OpenURL("https://tiogiras.games");
                        break;
                }
            });
        }
        
        private static void UpdateBasePackageLogic(VisualElement root)
        {
            root.ActivateLinks(
                evt =>
                {
                    switch (evt.linkID)
                    {
                        case "website":
                            Application.OpenURL("https://tiogiras.games");
                            break;
                    }
                });
        }        
        
        private static void ShortcutsLogic(VisualElement root)
        {
            root.ActivateLinks(
                evt =>
                {
                    switch (evt.linkID)
                    {
                        case "Edit/Shortcuts...":
                            EditorApplication.ExecuteMenuItem(evt.linkID);
                            break;
                    }
                });
        }
    }
}
#endif