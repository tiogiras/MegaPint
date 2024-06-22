#if UNITY_EDITOR
using System.IO;
using MegaPint.Editor.Scripts.GUI.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Windows.BaseWindowContent.InfoTabContent
{

/// <summary> Contains logic for displaying the info tabs </summary>
internal static class InfosTabDisplay
{
    private static string s_basePath;

    private static string _BasePath =>
        s_basePath ??= Path.Combine(Constants.BasePackage.UserInterface.InfoContent, "xxx");

    #region Public Methods

    /// <summary> Display the info based on the given key </summary>
    /// <param name="root"> Root <see cref="VisualElement" /> the info is added to </param>
    /// <param name="key"> Key corresponding to the target info </param>
    public static void Display(VisualElement root, InfosTabData.InfoKey key)
    {
        GUIUtility.Instantiate(Load(key), root);
        ActivateLogic(key, root);
    }

    #endregion

    #region Private Methods

    /// <summary> Invoked when the info is added to it's parent </summary>
    /// <param name="key"> Key of the added info </param>
    /// <param name="root"> Info as <see cref="VisualElement" /> </param>
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

            default:
                return;
        }
    }

    /// <summary> Logic of the contact info </summary>
    /// <param name="root"> Info as <see cref="VisualElement" /> </param>
    private static void ContactLogic(VisualElement root)
    {
        root.ActivateLinks(
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

    /// <summary> Load the uxml file of the selected info </summary>
    /// <param name="key"> Key corresponding to the targeted information </param>
    /// <returns> Loaded uxml file </returns>
    private static VisualTreeAsset Load(InfosTabData.InfoKey key)
    {
        return Resources.Load <VisualTreeAsset>(_BasePath.Replace("xxx", key.ToString()));
    }

    /// <summary> Logic of the managePackages info </summary>
    /// <param name="root"> Info as <see cref="VisualElement" /> </param>
    private static void ManagePackagesLogic(VisualElement root)
    {
        root.ActivateLinks(
            evt =>
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

    /// <summary> Logic of the shortcuts info </summary>
    /// <param name="root"> Info as <see cref="VisualElement" /> </param>
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

    /// <summary> Logic of the updateBasePackage info </summary>
    /// <param name="root"> Info as <see cref="VisualElement" /> </param>
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

    #endregion
}

}
#endif
