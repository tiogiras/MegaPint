#if UNITY_EDITOR
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

[assembly: InternalsVisibleTo("tiogiras.megapint.validators.editor")]

namespace MegaPint.Editor.Scripts.GUI.Utility
{

/// <summary> Partial utility class containing general gui utility functions </summary>
internal static partial class GUIUtility
{
    private static Action s_onForceRepaint;

    #region Public Methods

    /// <summary> Force all MegaPint editor windows to repaint </summary>
    public static void ForceRepaint()
    {
        StyleSheetValues.Reset();
        s_onForceRepaint?.Invoke();
    }

    /// <summary> Instantiate a <see cref="VisualElement" /> </summary>
    /// <param name="asset"> <see cref="VisualTreeAsset" /> to be instantiated </param>
    /// <param name="root"> The root <see cref="VisualElement" /> the new element should be added to </param>
    /// <returns> Instantiated <see cref="VisualElement" /> </returns>
    public static VisualElement Instantiate(VisualTreeAsset asset, VisualElement root = null)
    {
        TemplateContainer element = asset.Instantiate();

        root?.Add(element);

        return element;
    }

    /// <summary> Toggle on/off a group of buttons (only one active) </summary>
    /// <param name="activeIndex"> Index of the active button in the params </param>
    /// <param name="elements"> All buttons of the group </param>
    public static void ToggleActiveButtonInGroup(int activeIndex, params VisualElement[] elements)
    {
        for (var i = 0; i < elements.Length; i++)
        {
            VisualElement element = elements[i];

            if (i == activeIndex)
            {
                element.AddToClassList(StyleSheetClasses.Background.Color.Identity);
                element.AddToClassList(StyleSheetClasses.Text.Color.ButtonActive);

                continue;
            }

            element.RemoveFromClassList(StyleSheetClasses.Background.Color.Identity);
            element.RemoveFromClassList(StyleSheetClasses.Text.Color.ButtonActive);
        }
    }

    #endregion
}

}
#endif
