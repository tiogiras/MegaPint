#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MegaPint.Editor.Scripts.GUI;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts
{

// TODO continue Commenting here

/// <summary> Partial class used to display the right pane in the BaseWindow </summary>
internal static partial class DisplayContent
{
    public class TabSettings
    {
        public bool guides;
        public bool help;
        public bool info;
        public bool settings;
    }

    private class TabActions
    {
        public Action <VisualElement> guides;
        public Action <VisualElement> help;
        public Action <VisualElement> info;
        public Action <VisualElement> settings;
    }

    public struct DisplayContentReferences
    {
        public VisualElement tabs;
        public VisualElement tabContent;
        public CachedPackage package;
    }

    private enum Tab
    {
        Info, Guides, Settings, Help
    }

    private const string DisplayContentBase = "xxx/User Interface/Display Content";
    public static Action <VisualElement> onRightPaneGUI;
    private static Action s_onSelectedPackageChanged;

    private static readonly Dictionary <Tab, Button> s_tabs = new();
    private static readonly List <string> s_tabsContentLocations = new();
    private static readonly List <VisualElement> s_tabsContainer = new();

    #region Public Methods
    
    public static void DisplayRightPane(DisplayContentReferences refs)
    {
        s_onSelectedPackageChanged?.Invoke();

        CallMethodByName(refs.package.Key.ToString(), new object[] {refs});
    }

    #endregion

    #region Private Methods

    private static void CallMethodByName(string name, object[] parameters)
    {
        MethodInfo method = typeof(DisplayContent).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static);

        if (method != null)
            method.Invoke(typeof(DisplayContent), parameters);
    }

    private static VisualElement InitializeDisplayContent(
        DisplayContentReferences refs,
        TabSettings settings = null,
        TabActions actions = null)
    {
        var hasSettings = settings != null;

        refs.tabs.style.display = hasSettings ? DisplayStyle.Flex : DisplayStyle.None;

        if (!hasSettings)
        {
            var contentPath = DisplayContentBase.Replace("xxx", refs.package.Key.ToString());
            var template = Resources.Load <VisualTreeAsset>(contentPath);

            VisualElement content = GUIUtility.Instantiate(template, refs.tabContent);
            content.Q <Label>("PackageInfo").text = refs.package.Description;

            return content;
        }

        SetTabContentLocations(refs.package.Key, settings);
        RegisterTabCallbacks(refs, settings, actions);

        SwitchTab(refs.tabContent, Tab.Info, refs.package, actions);

        return null;
    }

    private static void RegisterTabCallbacks(DisplayContentReferences refs, TabSettings settings, TabActions tabActions)
    {
        UnregisterAllTabCallbacks();
        s_tabs.Clear();

        TryToEnableTabButton(refs.tabs, refs.tabContent, Tab.Info, settings.info, refs.package, tabActions);
        TryToEnableTabButton(refs.tabs, refs.tabContent, Tab.Guides, settings.guides, refs.package, tabActions);
        TryToEnableTabButton(refs.tabs, refs.tabContent, Tab.Settings, settings.settings, refs.package, tabActions);
        TryToEnableTabButton(refs.tabs, refs.tabContent, Tab.Help, settings.help, refs.package, tabActions);
    }

    private static void SetTabContentLocations(PackageKey key, TabSettings settings)
    {
        s_tabsContentLocations.Clear();
        s_tabsContainer.Clear();

        var location = DisplayContentBase.Replace("xxx", key.ToString());

        if (settings.info)
        {
            s_tabsContentLocations.Add(Path.Combine(location, "Info"));
            s_tabsContainer.Add(null);
        }

        if (settings.guides)
        {
            s_tabsContentLocations.Add(Path.Combine(location, "Guides"));
            s_tabsContainer.Add(null);
        }

        if (settings.settings)
        {
            s_tabsContentLocations.Add(Path.Combine(location, "Settings"));
            s_tabsContainer.Add(null);
        }

        if (settings.help)
        {
            s_tabsContentLocations.Add(Path.Combine(location, "Help"));
            s_tabsContainer.Add(null);
        }
    }

    private static void SwitchTab(
        VisualElement tabContentParent,
        Tab newTab,
        CachedPackage package,
        TabActions tabActions)
    {
        List <Tab> keys = s_tabs.Keys.ToList();

        for (var i = 0; i < s_tabs.Keys.Count; i++)
        {
            Tab tab = keys[i];
            var contentInstantiated = s_tabsContainer[i] != null;

            if (tab == newTab)
            {
                if (contentInstantiated)
                    s_tabsContainer[i].style.display = DisplayStyle.Flex;
                else
                {
                    var template = Resources.Load <VisualTreeAsset>(s_tabsContentLocations[i]);
                    VisualElement content = GUIUtility.Instantiate(template, tabContentParent);

                    s_tabsContainer[i] = content;
                }

                s_tabs[tab].AddToClassList(StyleSheetClasses.Text.Color.ButtonActive);
                s_tabs[tab].AddToClassList(StyleSheetClasses.Background.Color.Identity);

                continue;
            }

            if (contentInstantiated)
                s_tabsContainer[i].style.display = DisplayStyle.None;

            s_tabs[tab].RemoveFromClassList(StyleSheetClasses.Text.Color.ButtonActive);
            s_tabs[tab].RemoveFromClassList(StyleSheetClasses.Background.Color.Identity);
        }

        switch (newTab)
        {
            case Tab.Info:
                tabContentParent.Q <Label>("PackageInfo").text = package.Description;
                tabActions.info?.Invoke(tabContentParent);

                break;

            case Tab.Guides:
                tabActions.guides?.Invoke(tabContentParent);

                break;

            case Tab.Settings:
                tabActions.settings?.Invoke(tabContentParent);

                break;

            case Tab.Help:
                tabActions.help?.Invoke(tabContentParent);

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newTab), newTab, null);
        }
    }

    private static void TryToEnableTabButton(
        VisualElement parent,
        VisualElement tabContent,
        Tab tab,
        bool enabled,
        CachedPackage package,
        TabActions tabActions)
    {
        var searchString = tab switch
                           {
                               Tab.Info => "TabInfo",
                               Tab.Guides => "TabGuides",
                               Tab.Settings => "TabSettings",
                               Tab.Help => "TabHelp",
                               var _ => throw new ArgumentOutOfRangeException(nameof(tab), tab, null)
                           };

        var button = parent.Q <Button>(searchString);
        button.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;

        if (!enabled)
            return;

        if (s_tabs.ContainsKey(tab))
        {
            s_tabs[tab].RemoveFromClassList(StyleSheetClasses.Text.Color.ButtonActive);
            s_tabs[tab].RemoveFromClassList(StyleSheetClasses.Background.Color.Identity);
        }

        button.clickable = new Clickable(() => {SwitchTab(tabContent, tab, package, tabActions);});

        s_tabs.Add(tab, button);
    }

    private static void UnregisterAllTabCallbacks()
    {
        foreach (Button tab in s_tabs.Values)
            tab.clickable = null;
    }

    #endregion
}

}
#endif
