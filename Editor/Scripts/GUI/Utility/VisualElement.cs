#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Utility
{

/// <summary> Partial utility class containing <see cref="VisualElement"/> specific utility functions </summary>
public static partial class GUIUtility
{
    // TODO CONTINUE HERE
    
    /// <summary> Apply the theme settings to the <see cref="VisualElement"/> </summary>
    /// <param name="element"> Targeted element </param>
    /// <param name="onRepaintAction"> Action invoked when repainting the element on forceRepaint </param>
    /// <param name="subscribeToEvent"> If the method should subscribe to the forceRepaint event </param>
    public static void ApplyRootElementTheme(this VisualElement element, Action<VisualElement> onRepaintAction = null, bool subscribeToEvent = true)
    {
        if (subscribeToEvent)
            onForceRepaint += () => ApplyRootElementTheme(element, onRepaintAction, false);
        
        element.RemoveFromClassList(StyleSheetClasses.Theme.Dark);
        element.RemoveFromClassList(StyleSheetClasses.Theme.Light);

        element.AddToClassList("mp");
        element.AddToClassList(StyleSheetClasses.Theme.Current);
        
        onRepaintAction?.Invoke(element);
    }
    
    /// <summary> Set the flex growth variable of the element parent recursively </summary>
    /// <param name="startElement"> Element the search starts </param>
    /// <param name="iterations"> Targeted iteration count </param>
    /// <param name="value"> Value of the flex growth </param>
    public static VisualElement SetParentFlexGrowRecursive(this VisualElement startElement, int iterations, bool value)
    {
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
    
    /// <summary> Find the root visual element of the <see cref="VisualElement"/> </summary>
    /// <param name="root"> Element the search starts </param>
    /// <returns> RootVisualElement </returns>
    public static VisualElement GetRootVisualElement(this VisualElement root)
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
    
    /// <summary> Add a click interaction (button like) to the visual element </summary>
    /// <param name="element"> Targeted <see cref="VisualElement"/> </param>
    /// <param name="action"> Action to be invoked on click </param>
    public static void AddClickInteraction(this VisualElement element, Action action)
    {
        StyleColor defaultColor = element.style.unityBackgroundImageTintColor;
        
        element.RegisterCallback<MouseDownEvent>(
            _ =>
            {
                defaultColor = element.style.unityBackgroundImageTintColor;
            }, 
            TrickleDown.TrickleDown);
        
        element.RegisterCallback<MouseUpEvent>(
            _ =>
            {
                element.style.unityBackgroundImageTintColor = defaultColor;
                action?.Invoke();
            }, 
            TrickleDown.TrickleDown);
    }
    
    /// <summary> Get the border width of the targeted element </summary>
    /// <param name="element"> Targeted element </param>
    /// <returns> Width of the targeted element </returns>
    public static BorderWidth GetBorderWidth(this VisualElement element)
    {
        return new BorderWidth
        {
            top = element.style.borderTopWidth.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderTopWidth,
            bottom = element.style.borderBottomWidth.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderBottomWidth,
            left = element.style.borderLeftWidth.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderLeftWidth,
            right = element.style.borderRightWidth.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderRightWidth,
        };
    }
    
    /// <summary> Set the border width of the targeted element </summary>
    /// <param name="element"> Targeted element </param>
    /// <param name="width"> Targeted border width </param>
    public static void SetBorderWidth(this VisualElement element, float width)
    {
        element.style.borderTopWidth = width;
        element.style.borderRightWidth = width;
        element.style.borderBottomWidth = width;
        element.style.borderLeftWidth = width;
    }
    
    /// <summary> Set the border width of the targeted element </summary>
    /// <param name="element"> Targeted element </param>
    /// <param name="width"> Targeted border width </param>
    public static void SetBorderWidth(this VisualElement element, BorderWidth width)
    {
        element.style.borderTopWidth = width.top.keyword == StyleKeyword.Null ? StyleKeyword.Null : width.top;
        element.style.borderBottomWidth = width.bottom.keyword == StyleKeyword.Null ? StyleKeyword.Null : width.bottom;
        element.style.borderLeftWidth = width.left.keyword == StyleKeyword.Null ? StyleKeyword.Null : width.left;
        element.style.borderRightWidth = width.right.keyword == StyleKeyword.Null ? StyleKeyword.Null : width.right;
    }
    
    /// <summary> Set the border width of the targeted element </summary>
    /// <param name="element"> Targeted element </param>
    /// <param name="color"> Targeted border color </param>
    public static void SetBorderColor(this VisualElement element, Color color)
    {
        SetBorderColor(element, (StyleColor)color);
    }

    public struct BorderColor
    {
        public StyleColor top;
        public StyleColor bottom;
        public StyleColor left;
        public StyleColor right;
    }

    public struct BorderRadius
    {
        public StyleLength topLeft;
        public StyleLength topRight;
        public StyleLength bottomLeft;
        public StyleLength bottomRight;
    }
    
    public struct BorderWidth
    {
        public StyleFloat top;
        public StyleFloat bottom;
        public StyleFloat left;
        public StyleFloat right;
    }

    public static BorderColor GetBorderColor(this VisualElement element)
    {
        return new BorderColor
        {
            top = element.style.borderTopColor.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderTopColor,
            bottom = element.style.borderBottomColor.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderBottomColor,
            left = element.style.borderLeftColor.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderLeftColor,
            right = element.style.borderRightColor.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderRightColor
        };
    }

    public static void SetBorderColor(this VisualElement element, BorderColor color)
    {
        element.style.borderTopColor = color.top.keyword == StyleKeyword.Null ? StyleKeyword.Null : color.top;
        element.style.borderBottomColor = color.bottom.keyword == StyleKeyword.Null ? StyleKeyword.Null : color.bottom;
        element.style.borderLeftColor = color.left.keyword == StyleKeyword.Null ? StyleKeyword.Null : color.left;
        element.style.borderRightColor = color.right.keyword == StyleKeyword.Null ? StyleKeyword.Null : color.right;
    }
    
    public static void SetBorderColor(this VisualElement element, StyleColor color)
    {
        element.style.borderTopColor = color;
        element.style.borderRightColor = color;
        element.style.borderBottomColor = color;
        element.style.borderLeftColor = color;
    }
    
    public static BorderRadius GetBorderRadius(this VisualElement element)
    {
        return new BorderRadius
        {
            topLeft = element.style.borderTopLeftRadius.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderTopLeftRadius,
            topRight = element.style.borderTopRightRadius.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderTopRightRadius,
            bottomLeft = element.style.borderBottomLeftRadius.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderBottomLeftRadius,
            bottomRight = element.style.borderBottomRightRadius.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderBottomRightRadius,
        };
    }
    
    public static void SetBorderRadius(this VisualElement element, float radius)
    {
        element.style.borderTopLeftRadius = radius;
        element.style.borderTopRightRadius = radius;
        element.style.borderBottomLeftRadius = radius;
        element.style.borderBottomRightRadius = radius;
    }
    
    public static void SetBorderRadius(this VisualElement element, BorderRadius radius)
    {
        element.style.borderTopLeftRadius = radius.topLeft.keyword == StyleKeyword.Null ? StyleKeyword.Null : radius.topLeft;
        element.style.borderTopRightRadius = radius.topRight.keyword == StyleKeyword.Null ? StyleKeyword.Null : radius.topRight;
        element.style.borderBottomLeftRadius = radius.bottomLeft.keyword == StyleKeyword.Null ? StyleKeyword.Null : radius.bottomLeft;
        element.style.borderBottomRightRadius = radius.bottomRight.keyword == StyleKeyword.Null ? StyleKeyword.Null : radius.bottomRight;
    }
    
    public static void SetMargin(this VisualElement element, float margin)
    {
        element.style.marginTop = margin;
        element.style.marginRight = margin;
        element.style.marginBottom = margin;
        element.style.marginLeft = margin;
    }
    
    public static void SetPadding(this VisualElement element, float padding)
    {
        element.style.paddingTop = padding;
        element.style.paddingRight = padding;
        element.style.paddingBottom = padding;
        element.style.paddingLeft = padding;
    }
}

}
#endif
