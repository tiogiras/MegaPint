#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Editor.Scripts.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts
{
    public static partial class DisplayContent
    {
        private static Action <int> s_onSelectedTabChanged;
        
        private static readonly List <Button> s_tabs = new();
        private static readonly List <string> s_tabsContentLocations = new();
        private static readonly List <VisualElement> s_tabsContainer = new();

        public static void DisplayRightPane(MegaPintPackagesData.PackageKey key, VisualElement root)
        {
            switch (key)
            {
                case MegaPintPackagesData.PackageKey.AutoSave: AutoSave(root); break;
                case MegaPintPackagesData.PackageKey.Validators: Validators(root); break;
                default: return;
            }
        }

        private static void SwitchTab(VisualElement tabContentParent, int index)
        {
            for (var i = 0; i < s_tabs.Count; i++)
            {
                var contentInstantiated = s_tabsContainer[i] != null;

                if (i == index)
                {
                    if (contentInstantiated)
                        s_tabsContainer[i].style.display = DisplayStyle.Flex;
                    else
                    {
                        TemplateContainer content = Resources.Load<VisualTreeAsset>(s_tabsContentLocations[i]).Instantiate();
                        tabContentParent.Add(content);
                        
                        s_tabsContainer[i] = content;
                    }
                }
                else
                {
                    if (contentInstantiated)
                        s_tabsContainer[i].style.display = DisplayStyle.None;
                }
            }
            
            s_onSelectedTabChanged?.Invoke(index);
        }
        
        private static void SetTabContentLocations(params string[] locations)
        {
            s_tabsContentLocations.Clear();
            s_tabsContainer.Clear();
            
            foreach (var location in locations)
            {
                s_tabsContentLocations.Add(location);
                s_tabsContainer.Add(null);
            }
        }
        
        private static void RegisterTabCallbacks(VisualElement root, VisualElement tabContentParent, int expectedTabCount)
        {
            UnregisterAllTabCallbacks();
            s_tabs.Clear();
            
            for (var i = 0; i < expectedTabCount; i++)
            {
                var index = i;
                var tab = root.Q <Button>($"Tab{i}");
                
                s_tabs.Add(tab);
                tab.clickable = new Clickable(() => {SwitchTab(tabContentParent, index);});
            }
        }

        private static void UnregisterAllTabCallbacks()
        {
            foreach (Button tab in s_tabs)
            {
                tab.clickable = null;
            }
        }

        private static partial void AutoSave(VisualElement root);

        private static partial void Validators(VisualElement root);
    }
}
#endif