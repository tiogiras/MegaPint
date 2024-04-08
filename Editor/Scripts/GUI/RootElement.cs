using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.GUI
{

public static class RootElement
{

    public static string FontClass => "mpFont";
    
    public static class Colors
    {
        public static Color Link => s_linkColor.Value();

        public static Color Primary => s_primary.Value();

        public static Color PrimaryInteracted => s_primaryInteracted.Value();

        public static Color Text => s_primaryText.Value();

        public static Color SecondaryText => s_secondaryText.Value();

        public static Color Back => s_primaryBack.Value();

        public static Color SecondaryBack => s_secondaryBack.Value();

        public static Color TertiaryBack => s_tertiaryBack.Value();
        
        public static Color Separator => s_separatorColor.Value();
    }

    private class ThemedColor
    {
        public Color darkColor;
        public Color lightColor;
        #region Public Methods

        public Color Value()
        {
            // TODO check if light theme
            return darkColor;
        }

        #endregion
    }

    private static readonly ThemedColor s_linkColor = new()
    {
        darkColor = new Color(0.82f, 0f, 0.45f),
        lightColor = new Color(0.82f, 0f, 0.45f)
    };

    private static readonly ThemedColor s_primary = new()
    {
        darkColor = new Color(0.82f, 0f, 0.45f),
        lightColor = new Color(0.82f, 0f, 0.45f)
    };

    private static readonly ThemedColor s_primaryInteracted = new()
    {
        darkColor = new Color(0.66f, 0f, 0.33f),
        lightColor = new Color(0.66f, 0f, 0.33f)
    };

    private static readonly ThemedColor s_primaryText = new()
    {
        darkColor = new Color(0.96f, 0.96f, 0.96f),
        lightColor = new Color(0.96f, 0.96f, 0.96f)
    };

    private static readonly ThemedColor s_secondaryText = new()
    {
        darkColor = new Color(0.84f, 0.84f, 0.84f),
        lightColor = new Color(0.84f, 0.84f, 0.84f)
    };

    private static readonly ThemedColor s_primaryBack = new()
    {
        darkColor = new Color(0.1f, 0.1f, 0.1f),
        lightColor = new Color(0.1f, 0.1f, 0.1f)
    };

    private static readonly ThemedColor s_secondaryBack = new()
    {
        darkColor = new Color(0.15f, 0.15f, 0.15f),
        lightColor = new Color(0.15f, 0.15f, 0.15f)
    };

    private static readonly ThemedColor s_tertiaryBack = new()
    {
        darkColor = new Color(0.19f, 0.19f, 0.19f),
        lightColor = new Color(0.19f, 0.19f, 0.19f)
    };

    private static readonly ThemedColor s_separatorColor = new()
    {
        darkColor = new Color(0.35f, 0.35f, 0.35f),
        lightColor = new Color(0.35f, 0.35f, 0.35f)
    };

    public static readonly Dictionary <string, Action <List <VisualElement>>> Overwrites = new()
    {
        {Overwrite.mpPrimaryColorText.ToString(), OverwritePrimaryColorText},
        {Overwrite.mpPrimaryColorBorder.ToString(), OverwritePrimaryColorBorder},
        {Overwrite.mpTextColor.ToString(), OverwriteTextColor},
        {Overwrite.mpTextColorSecondary.ToString(), OverwriteTextColorSecondary},
        {Overwrite.mpBackground1.ToString(), OverwriteBackgroundColor1},
        {Overwrite.mpBackground2.ToString(), OverwriteBackgroundColor2},
        {Overwrite.mpBackground3.ToString(), OverwriteBackgroundColor3},
        {Overwrite.mpInteraction.ToString(), OverwriteInteraction},
        {Overwrite.mpBg1AsBorderColor.ToString(), OverwriteBg1AsBorderColor}
    };

    private static void OverwriteBg1AsBorderColor(List <VisualElement> elements)
    {
        Color color = Colors.Back;

        foreach (VisualElement element in elements)
        {
            GUIUtility.SetBorderColor(element, color);
        }
    }

    private static void OverwritePrimaryColorText(List <VisualElement> elements)
    {
        Color color = Colors.Primary;

        foreach (VisualElement element in elements)
        {
            element.style.color = color;
        }
    }
    
    private static void OverwritePrimaryColorBorder(List <VisualElement> elements)
    {
        Color color = Colors.Primary;

        foreach (VisualElement element in elements)
        {
            GUIUtility.SetBorderColor(element, color);
        }
    }
    
    private static void OverwriteTextColor(List <VisualElement> elements)
    {
        Color color = Colors.Text;

        foreach (VisualElement element in elements)
        {
            element.style.color = color;
        }
    }
    
    private static void OverwriteTextColorSecondary(List <VisualElement> elements)
    {
        Color color = Colors.SecondaryText;

        foreach (VisualElement element in elements)
        {
            element.style.color = color;
        }
    }    
    
    private static void OverwriteBackgroundColor1(List <VisualElement> elements)
    {
        Color color = Colors.Back;

        foreach (VisualElement element in elements)
        {
            element.style.backgroundColor = color;
        }
    }
    
    private static void OverwriteBackgroundColor2(List <VisualElement> elements)
    {
        Color color = Colors.SecondaryBack;

        foreach (VisualElement element in elements)
        {
            element.style.backgroundColor = color;
        }
    }
    
    private static void OverwriteBackgroundColor3(List <VisualElement> elements)
    {
        Color color = Colors.TertiaryBack;

        foreach (VisualElement element in elements)
        {
            element.style.backgroundColor = color;
        }
    }

    private static void OverwriteInteraction(List <VisualElement> elements)
    {
        foreach (VisualElement element in elements)
        {
            GUIUtility.SubscribeInteractable(element);
        }
    }
}

}
