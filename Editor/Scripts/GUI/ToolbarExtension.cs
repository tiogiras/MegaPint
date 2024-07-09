// TODO commenting

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.GUI
{

[InitializeOnLoad]
internal static class ToolbarExtension
{
    public class GUIAction : IComparable <GUIAction>
    {
        public int executionIndex;
        public Action <VisualElement> action;

        public int CompareTo(GUIAction other)
        {
            return executionIndex.CompareTo(other.executionIndex);
        }
    }

    private static readonly List <GUIAction> s_leftZoneActions = new();
    private static readonly List <GUIAction> s_rightZoneActions = new();

    public static void AddLeftZoneAction(GUIAction action)
    {
        s_leftZoneActions.Add(action);
    }
    
    public static void AddRightZoneAction(GUIAction action)
    {
        s_rightZoneActions.Add(action);
    }
    
    static ToolbarExtension()
    {
        EditorApplication.delayCall += OnInitialization;
    }

    private static void OnInitialization()
    {
        EditorApplication.delayCall -= OnInitialization;
        
        Type toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        object toolbar = Resources.FindObjectsOfTypeAll(toolbarType)[0];
        
        FieldInfo root = toolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (root == null)
            return;

        var rawRoot = root.GetValue(toolbar);
        
        if (rawRoot is not VisualElement mRoot)
            return;
        
        VisualElement leftZone = mRoot.Q("ToolbarZoneLeftAlign");
        VisualElement rightZone = mRoot.Q("ToolbarZoneRightAlign");

        if (s_leftZoneActions.Count > 0)
        {
            s_leftZoneActions.Sort();
            
            foreach (GUIAction action in s_leftZoneActions)
            {
                action.action?.Invoke(leftZone);
            }
        }

        if (s_rightZoneActions.Count <= 0)
            return;

        s_rightZoneActions.Sort();
            
        foreach (GUIAction action in s_rightZoneActions)
        {
            action.action?.Invoke(rightZone);
        }
    }

    public static ToolbarToggle CreateToolbarToggle(string name, Action<bool> action)
    {
        var toggle = new ToolbarToggle {text = name};
        
        toggle.RegisterValueChangedCallback(evt => action?.Invoke(evt.newValue));

        return toggle;
    }
    
    public static ToolbarToggle CreateToolbarToggle(string resourcePath, Action <VisualElement> onCreation, Action<bool> action)
    {
        var toggle = new ToolbarToggle( );
        
        toggle.Q(className: "unity-toggle__input").style.display = DisplayStyle.None;
        
        toggle.RegisterValueChangedCallback(evt => action?.Invoke(evt.newValue));

        var template = Resources.Load <VisualTreeAsset>(resourcePath);
        VisualElement content = GUIUtility.Instantiate(template);
        content.style.flexGrow = 1f;
        content.style.flexShrink = 1f;
        
        onCreation?.Invoke(content);
        
        toggle.Add(content);

        return toggle;
    }
}

}
#endif
