#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.GUI.Utility;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Windows
{

/// <summary> Editor window to display the compatibilities of a package with unity versions </summary>
internal class VersionCompatibility : EditorWindowBase
{
    [Serializable]
    public class Compatibility
    {
        public string unity_versions;
        public string value;
    }

    [Serializable]
    public class VersionInfo
    {
        public string version;
        public List<Compatibility> compatibilities;
    }
    
    [Serializable]
    public class CompatibilityData
    {
        public List<VersionInfo> items;
    }
    
    private VisualTreeAsset _baseWindow;
    private VisualTreeAsset _compatibilityItem;

    private VisualElement _contentPanel;
    private VisualElement _loadingPanel;
    private VisualElement _errorPanel;
    private VisualElement _errorNoConnection;
    private VisualElement _errorNoData;
    
    private Label _title;

    #region Public Methods

    /// <summary> Initialize the version compatibility window with the targeted package </summary>
    /// <param name="package"> Targeted package </param>
    public void Initialize(CachedPackage package)
    {
        _title.text = _title.text.Replace("xxx", package.DisplayName);
        
        _ = LoadData(package.Key);
    }

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "Version Compatibility";

        minSize = new Vector2(300, 200);
        
        if (!SaveValues.BasePackage.ApplyPSVersionCompatibility)
            return this;

        this.CenterOnMainWin(450, 550);
        SaveValues.BasePackage.ApplyPSVersionCompatibility = false;

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return Constants.BasePackage.UserInterface.VersionCompatibility;
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = rootVisualElement;

        VisualElement content = GUIUtility.Instantiate(_baseWindow, root);
        content.style.flexGrow = 1f;

        _title = content.Q <Label>("Title");

        _contentPanel = content.Q <VisualElement>("Content");
        _loadingPanel = content.Q <VisualElement>("Loading");
        _errorPanel = content.Q <VisualElement>("Error");
        _errorNoConnection = _errorPanel.Q <VisualElement>("NoConnection");
        _errorNoData = _errorPanel.Q <VisualElement>("NoData");

        _contentPanel.style.display = DisplayStyle.None;
        _loadingPanel.style.display = DisplayStyle.Flex;
        _errorPanel.style.display = DisplayStyle.None;
        
        root.ActivateLinks(evt =>
        {
            switch (evt.linkText)
            {
                case "report":
                    ContextMenu.BasePackage.OpenBaseWindowPerLink("Info/Report");
                    break;
            }
        });
        
        RegisterCallbacks();
    }

    protected override bool LoadResources()
    {
        _baseWindow = Resources.Load <VisualTreeAsset>(BasePath());
        _compatibilityItem = Resources.Load <VisualTreeAsset>(Constants.BasePackage.UserInterface.CompatibilityItem);

        return _baseWindow != null;
    }

    protected override void RegisterCallbacks()
    {

    }

    protected override void UnRegisterCallbacks()
    {
    }

    #endregion

    #region Private Methods

    private async Task LoadData(PackageKey package)
    {
        UnityWebRequest request = UnityWebRequest.Get("https://tiogiras.games/megapint_api/checkup.php");

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            _loadingPanel.style.display = DisplayStyle.None;
            _errorPanel.style.display = DisplayStyle.Flex;
            
            _errorNoConnection.style.display = DisplayStyle.Flex;
            _errorNoData.style.display = DisplayStyle.None;
            return;
        }
        
        request = UnityWebRequest.Get($"https://tiogiras.games/megapint_api/get_version_compatibility.php?prefix={VersionUtility.GetPrefix(package)}");
        
        operation = request.SendWebRequest();
        
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success || request.downloadHandler.text.Contains("error"))
        {
            _loadingPanel.style.display = DisplayStyle.None;
            _errorPanel.style.display = DisplayStyle.Flex;
            
            _errorNoConnection.style.display = DisplayStyle.None;
            _errorNoData.style.display = DisplayStyle.Flex;
            return;
        }
        
        _loadingPanel.style.display  = DisplayStyle.None;
        
        CompatibilityData data = ExtractData(request.downloadHandler.text);
        DisplayData(data);
    }
    
    private static CompatibilityData ExtractData(string jsonData)
    {
        var wrappedJson = "{ \"items\": " + jsonData + " }";
        var data = JsonUtility.FromJson<CompatibilityData>(wrappedJson);
        
        data.items = data.items
                         .OrderByDescending (v => v.version)
                         .ToList();

        return data;
    }
    
    private void DisplayData(CompatibilityData data)
    {
        var tabView = _contentPanel.Q <TabView>();

        foreach (VersionInfo version in data.items)
        {
            var versionName = version.version;
            versionName = versionName[(versionName.IndexOf("_v", StringComparison.Ordinal) + 2)..];
            
            var tab = new Tab(versionName);
            tabView.Add(tab);

            tab.SetMargin(20);
            tab.style.marginBottom = 0;

            var content = new VisualElement
            {
                style =
                {
                    flexGrow = 1
                }
            };

            tab.contentContainer.Add(content);

            var listView = new ListView();
            content.Add(listView);

            listView.selectionType = SelectionType.None;

            listView.makeItem = () => _compatibilityItem.Instantiate();

            listView.bindItem = (element, i) =>
            {
                Compatibility comp = version.compatibilities[i];

                element.Q <Label>().text = comp.unity_versions;
                
                element.Q <VisualElement>("Unknown").style.display = string.IsNullOrEmpty(comp.value) || (comp.value ?? "").Equals("0") ? DisplayStyle.Flex : DisplayStyle.None;
                element.Q <VisualElement>("Verified").style.display = (comp.value ?? "").Equals("1") ? DisplayStyle.Flex : DisplayStyle.None;
                element.Q <VisualElement>("Bugged").style.display = (comp.value ?? "").Equals("2") ? DisplayStyle.Flex : DisplayStyle.None;
                element.Q <VisualElement>("Unusable").style.display = (comp.value ?? "").Equals("3") ?  DisplayStyle.Flex : DisplayStyle.None;
            };

            List <Compatibility> source = version.compatibilities;
            source.Reverse();
            
            for (var i = source.Count - 1; i >= 0; i--)
            {
                Compatibility comp = version.compatibilities[i];
                Compatibility previousComp = version.compatibilities[Mathf.Clamp(i + 1, 0, version.compatibilities.Count - 1)];

                if (string.IsNullOrEmpty(comp.value) && (previousComp.value ?? "").Equals("3"))
                    CombineCompatibilities(ref source, ref comp, ref previousComp);
                
                if (string.IsNullOrEmpty(comp.value) && (previousComp.value ?? "").Equals("0"))
                    CombineCompatibilities(ref source, ref comp, ref previousComp);
            }

            listView.itemsSource = source;
        }
        
        _contentPanel.style.display = DisplayStyle.Flex;
    }

    private static void CombineCompatibilities(ref List <Compatibility> source, ref Compatibility comp, ref Compatibility previousComp)
    {
        var comps = new List <string>();

        if (comp.unity_versions.Contains(" - "))
            comps.AddRange(comp.unity_versions.Split(" - "));

        if (previousComp.unity_versions.Contains(" - "))
            comps.AddRange(previousComp.unity_versions.Split(" - "));

        comps = comps.OrderByDescending(c => c).ToList();

        previousComp.unity_versions = $"{comps[^1]} - {comps[0]}";

        source.RemoveAt(source.IndexOf(comp));
    }

    #endregion
}

}
#endif
