#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.GUI.Utility;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Windows.BaseWindowContent.InfoTabContent
{

/// <summary> Contains logic for displaying the info tabs </summary>
internal static class InfosTabDisplay
{
    [System.Serializable]
    public class ReportData
    {
        public string title;
        public string message;
        public string type;
        public string mpVersion;
        public string package;
        public string packageVersion;
    }
    
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
            
            case InfosTabData.InfoKey.Report:
                ReportLogic(root);

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

    /// <summary> Logic of the report info </summary>
    /// <param name="root"> Info as <see cref="VisualElement" /> </param>
    private static async Task ReportLogic(VisualElement root)
    {
        VisualElement loadingPanel = root.Q ("Loading");
        VisualElement contentPanel = root.Q ("Content");
        VisualElement errorPanel = root.Q ("Error");
        
        loadingPanel.style.display = DisplayStyle.Flex;
        contentPanel.style.display = DisplayStyle.None;
        errorPanel.style.display = DisplayStyle.None;
        
        errorPanel.ActivateLinks(evt =>
        {
            switch (evt.linkID)
            {
                case "github":
                    Application.OpenURL("https://github.com/tiogiras/MegaPint");

                    break;
            }
        });
        
        UnityWebRequest request = UnityWebRequest.Get("https://tiogiras.games/megapint_api/checkup_report.php");
        
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            loadingPanel.style.display = DisplayStyle.None;
            errorPanel.style.display = DisplayStyle.Flex;
            
            return;
        }
        
        request = UnityWebRequest.Get("https://tiogiras.games/megapint_api/get_versions.php");
        
        operation = request.SendWebRequest();
        
        while (!operation.isDone)
        {
            await Task.Yield();
        }
        
        if (request.result != UnityWebRequest.Result.Success)
        {
            loadingPanel.style.display = DisplayStyle.None;
            errorPanel.style.display = DisplayStyle.Flex;
            
            return;
        }
        
        loadingPanel.style.display = DisplayStyle.None;
        contentPanel.style.display = DisplayStyle.Flex;
        
        ReportSuggestionLogic(contentPanel.Q("Suggestion"));
        ReportIssueLogic(contentPanel.Q("Issue"));
    }

    /// <summary> Holds the logic for setting up the make a suggestion tab </summary>
    /// <param name="root"> Root element of the tab </param>
    private static void ReportSuggestionLogic(VisualElement root)
    {
        var noTitle = root.Q <Label>("NoTitle");
        var noMessage = root.Q <Label>("NoMessage");

        noTitle.style.display = DisplayStyle.None;
        noMessage.style.display = DisplayStyle.None;
        
        VisualElement success = root.Q("Success");
        VisualElement fail = root.Q("Fail");
        
        success.style.display = DisplayStyle.None;
        fail.style.display = DisplayStyle.None;
        
        var btnSend = root.Q <Button>("BTN_Send");

        btnSend.clickable = new Clickable(() =>
        {
            var title = root.Q <TextField>("Title");
            var message = root.Q <TextField>("Message");
            
            var hasTitle = !string.IsNullOrEmpty(title.text);
            var hasMessage = !string.IsNullOrEmpty(message.text);

            noTitle.style.display = hasTitle ? DisplayStyle.None : DisplayStyle.Flex;
            noMessage.style.display = hasMessage ? DisplayStyle.None : DisplayStyle.Flex;
            
            if (!hasTitle || !hasMessage)
                return;

            _ = PostReport(btnSend, title, message, "Suggestion", string.Empty, string.Empty, success, fail);
        });
    }

    private static async Task PostReport(
        Button button, 
        TextField title, 
        TextField message, 
        string type, 
        string package, 
        string packageVersion, 
        VisualElement success, 
        VisualElement error)
    {
        var json = JsonUtility.ToJson(new ReportData
        {
            title = title.text,
            message = message.text,
            type = type,
            mpVersion = PackageCache.BasePackage.version,
            package = string.IsNullOrEmpty(package) ? "No package specified" : package,
            packageVersion = string.IsNullOrEmpty(packageVersion) ? "No package version specified" : packageVersion
        });

        var request = new UnityWebRequest("https://tiogiras.games/megapint_api/post_report.php", "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        button.SetEnabled(false);

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            await FadeResult(error, button);
            return;
        }

        title.value = string.Empty;
        message.value = string.Empty;
        
        await FadeResult(success, button);
    }
    
    private static async Task FadeResult(VisualElement element, Button button)
    {
        element.style.display = DisplayStyle.Flex;
        element.style.opacity = 1.0f;

        const float fadeDuration = 1.0f;
        const int steps = 30;
        const float stepTime = fadeDuration / steps;

        for (var i = 0; i < steps; i++)
        {
            var t = (i + 1) / (float)steps;
            element.style.opacity = 1.0f - t;

            await Task.Delay((int)(stepTime * 1000));
        }

        element.style.display = DisplayStyle.None;
        
        button.SetEnabled(true);
    }

    
    /// <summary> Holds the logic for setting up the report an issue tab </summary>
    /// <param name="root"> Root element of the tab </param>
    private static void ReportIssueLogic(VisualElement root)
    {
        var noTitle = root.Q <Label>("NoTitle");
        var noMessage = root.Q <Label>("NoMessage");

        noTitle.style.display = DisplayStyle.None;
        noMessage.style.display = DisplayStyle.None;
        
        VisualElement success = root.Q("Success");
        VisualElement fail = root.Q("Fail");
        
        success.style.display = DisplayStyle.None;
        fail.style.display = DisplayStyle.None;

        var packageDropdown = root.Q <DropdownField>("Package");
        var packageVersionDropdown = root.Q <DropdownField>("Version");

        List <string> packageChoices = new ();

        foreach (PackageKey value in (PackageKey[])Enum.GetValues(typeof(PackageKey)))
        {
            if (value is PackageKey.Undefined or PackageKey.BATesting)
                continue;
            
            packageChoices.Add(PackageCache.Get(value).DisplayName);
        }

        packageDropdown.choices = packageChoices;
        packageDropdown.index = 0;
        
        var btnSend = root.Q <Button>("BTN_Send");

        btnSend.clickable = new Clickable(() =>
        {
            var title = root.Q <TextField>("Title");
            var message = root.Q <TextField>("Message");
            
            var hasTitle = !string.IsNullOrEmpty(title.text);
            var hasMessage = !string.IsNullOrEmpty(message.text);

            noTitle.style.display = hasTitle ? DisplayStyle.None : DisplayStyle.Flex;
            noMessage.style.display = hasMessage ? DisplayStyle.None : DisplayStyle.Flex;
            
            if (!hasTitle || !hasMessage)
                return;

            _ = PostReport(btnSend, title, message, "Issue", packageDropdown.value, packageVersionDropdown.value, success, fail);
        });
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
