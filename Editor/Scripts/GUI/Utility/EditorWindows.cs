#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MegaPint.Editor.Scripts.GUI.Utility
{

internal static partial class GUIUtility
{
    #region Private Methods

    /// <summary> Center the window on the main window </summary>
    /// <param name="window"> Targeted window </param>
    /// <param name="width"> Width of the window </param>
    /// <param name="height"> Height of the window </param>
    public static void CenterOnMainWin(this EditorWindow window, int width = -1, int height = -1)
    {
        Rect main = GetEditorMainWindowPos();
        Rect pos = window.position;

        if (width > -1)
            pos.width = width;

        if (height > -1)
            pos.height = height;
        
        var w = (main.width - pos.width) * 0.5f;
        var h = (main.height - pos.height) * 0.5f;
        
        pos.x = main.x + w;
        pos.y = main.y + h;
        
        window.position = pos;
    }

    /// <summary> Get all derived types </summary>
    /// <param name="aAppDomain"> Targeted AppDomain </param>
    /// <param name="aType"> Target type </param>
    /// <returns> All derived types from type </returns>
    private static IEnumerable <Type> GetAllDerivedTypes(this AppDomain aAppDomain, Type aType)
    {
        Assembly[] assemblies = aAppDomain.GetAssemblies();

        return (from assembly in assemblies
                from type in assembly.GetTypes()
                where type.IsSubclassOf(aType)
                select type).ToArray();
    }

    /// <summary> Get the rect of the main editor window </summary>
    /// <returns> Rect of the found main editor window </returns>
    /// <exception cref="MissingMemberException"> ContainerWindow not found </exception>
    /// <exception cref="MissingFieldException"> m_ShowMode or position not found </exception>
    /// <exception cref="NotSupportedException"> Main window not found </exception>
    private static Rect GetEditorMainWindowPos()
    {
        Type containerWinType = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject)).
                                          FirstOrDefault(t => t.Name == "ContainerWindow");

        if (containerWinType == null)
        {
            throw new MissingMemberException(
                "Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
        }

        FieldInfo showModeField = containerWinType.GetField(
            "m_ShowMode",
            BindingFlags.NonPublic | BindingFlags.Instance);

        PropertyInfo positionProperty = containerWinType.GetProperty(
            "position",
            BindingFlags.Public | BindingFlags.Instance);

        if (showModeField == null || positionProperty == null)
        {
            throw new MissingFieldException(
                "Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
        }

        Object[] windows = Resources.FindObjectsOfTypeAll(containerWinType);

        foreach (Object win in windows)
        {
            var showMode = (int)showModeField.GetValue(win);

            if (showMode != 4) // main window
                continue;

            var pos = (Rect)positionProperty.GetValue(win, null);

            return pos;
        }

        throw new NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
    }

    #endregion
}

}
#endif
