#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Utility
{

/// <summary> Partial utility class containing <see cref="VisualElement" /> specific utility functions </summary>
internal static partial class GUIUtility
{
    #region Public Methods

    /// <summary> Add a click interaction (button like) to the visual element </summary>
    /// <param name="element"> Targeted <see cref="VisualElement" /> </param>
    /// <param name="action"> Action to be invoked on click </param>
    public static void AddClickInteraction(this VisualElement element, Action action)
    {
        StyleColor defaultColor = element.style.unityBackgroundImageTintColor;

        element.RegisterCallback <MouseDownEvent>(
            _ => {defaultColor = element.style.unityBackgroundImageTintColor;},
            TrickleDown.TrickleDown);

        element.RegisterCallback <MouseUpEvent>(
            _ =>
            {
                element.style.unityBackgroundImageTintColor = defaultColor;
                action?.Invoke();
            },
            TrickleDown.TrickleDown);
    }

    /// <summary> Apply the theme settings to the <see cref="VisualElement" /> </summary>
    /// <param name="element"> Targeted element </param>
    /// <param name="onRepaintAction"> Action invoked when repainting the element on forceRepaint </param>
    /// <param name="subscribeToEvent"> If the method should subscribe to the forceRepaint event </param>
    public static void ApplyRootElementTheme(
        this VisualElement element,
        Action <VisualElement> onRepaintAction = null,
        bool subscribeToEvent = true)
    {
        if (subscribeToEvent)
            s_onForceRepaint += () => ApplyRootElementTheme(element, onRepaintAction, false);

        element.RemoveFromClassList(StyleSheetClasses.Theme.Dark);
        element.RemoveFromClassList(StyleSheetClasses.Theme.Light);

        element.AddToClassList("mp");
        element.AddToClassList(StyleSheetClasses.Theme.Current);

        onRepaintAction?.Invoke(element);
    }

    /// <summary> Set the border radius of the targeted <see cref="VisualElement" /> </summary>
    /// <param name="element"> Targeted element </param>
    /// <param name="radius"> Targeted border radius </param>
    public static void SetBorderRadius(this VisualElement element, float radius)
    {
        element.style.borderTopLeftRadius = radius;
        element.style.borderTopRightRadius = radius;
        element.style.borderBottomLeftRadius = radius;
        element.style.borderBottomRightRadius = radius;
    }

    /// <summary> Set the border width of the targeted <see cref="VisualElement" /> </summary>
    /// <param name="element"> Targeted element </param>
    /// <param name="width"> Targeted border width </param>
    public static void SetBorderWidth(this VisualElement element, float width)
    {
        element.style.borderTopWidth = width;
        element.style.borderRightWidth = width;
        element.style.borderBottomWidth = width;
        element.style.borderLeftWidth = width;
    }

    /// <summary> Set the margin of the targeted <see cref="VisualElement" /> </summary>
    /// <param name="element"> Targeted element </param>
    /// <param name="margin"> Targeted margin </param>
    public static void SetMargin(this VisualElement element, float margin)
    {
        element.style.marginTop = margin;
        element.style.marginRight = margin;
        element.style.marginBottom = margin;
        element.style.marginLeft = margin;
    }

    /// <summary> Set the padding of the targeted <see cref="VisualElement" /> </summary>
    /// <param name="element"> Targeted element </param>
    /// <param name="padding"> Targeted padding </param>
    public static void SetPadding(this VisualElement element, float padding)
    {
        element.style.paddingTop = padding;
        element.style.paddingRight = padding;
        element.style.paddingBottom = padding;
        element.style.paddingLeft = padding;
    }

    /// <summary> Set the flex growth variable of the element parent recursively </summary>
    /// <param name="startElement"> Element the search starts </param>
    /// <param name="iterations"> Targeted iteration count </param>
    /// <param name="value"> Value of the flex growth </param>
    public static VisualElement SetParentFlexGrowRecursive(this VisualElement startElement, int iterations, bool value)
    {
        if (startElement == null)
            return null;

        VisualElement element = startElement.parent;

        for (var i = 0; i < iterations; i++)
        {
            if (element == null)
                break;

            element.style.flexGrow = value ? 1 : 0;
            element = element.parent;
        }

        return element;
    }

    #endregion

    #region Private Methods

    /// <summary> Find the root visual element of the <see cref="VisualElement" /> </summary>
    /// <param name="root"> Element the search starts </param>
    /// <returns> RootVisualElement </returns>
    private static VisualElement GetRootVisualElement(this VisualElement root)
    {
        VisualElement check = root;

        while (check.parent != null)
        {
            if (!string.IsNullOrEmpty(check.parent.viewDataKey))
            {
                if (check.parent.viewDataKey.Equals("rootVisualContainer"))
                    break;
            }

            check = check.parent;
        }

        return check;
    }

    #endregion
}

}
#endif
