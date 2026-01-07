#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace MegaPint.Editor.Scripts.GUI.Utility
{
internal static class MainToolbarUtility
{
    public static void ForceShowElement(string path, Action onSuccess)
    {
        if (Application.isBatchMode)
            return;

        EditorApplication.delayCall += () => EnableWhenReady(path, onSuccess);
    }

    private static void EnableWhenReady(string path, Action onSuccess)
    {
        var triesLeft = 300;

        EditorApplication.update += Tick;
        return;

        void Tick()
        {
            if (triesLeft-- <= 0)
            {
                EditorApplication.update -= Tick;
                return;
            }

            if (!IsMainToolbarWindowExists())
                return;

            EditorApplication.update -= Tick;

            if (InvokeMainToolbarShowAll(path))
                onSuccess?.Invoke();

            MainToolbar.Refresh(path);
        }
    }

    private static bool IsMainToolbarWindowExists()
    {
        Type t = typeof(MainToolbar);

        PropertyInfo prop = t.GetProperty("windowExists",
            BindingFlags.Static | BindingFlags.NonPublic);

        if (prop == null)
            return false;

        return (bool)prop.GetValue(null);
    }

    private static bool InvokeMainToolbarShowAll(string prefix)
    {
        Type t = typeof(MainToolbar);

        MethodInfo method = t.GetMethod("ShowAll",
            BindingFlags.Static | BindingFlags.NonPublic);

        if (method == null)
            return false;

        method.Invoke(null, new object[]
        {
            prefix,
        });

        return true;
    }
}
}
#endif
