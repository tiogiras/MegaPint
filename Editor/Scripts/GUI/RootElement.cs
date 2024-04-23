using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.GUI
{

public static class RootElement
{

    public static string FontClass => "mp_font";
    
    public static class Colors
    {
        public static Color Link => s_linkColor.Value();
        
        public static Color Info => s_infoColor.Value();

        public static Color Primary => s_primary.Value();

        public static Color PrimaryInteracted => s_primaryInteracted.Value();

        public static Color Text => s_primaryText.Value();

        public static Color TextSecondary => s_secondaryText.Value();
        
        public static Color TextRed => s_textRed.Value();
        
        public static Color TextGreen => s_textGreen.Value();
        
        public static Color TextOrange => s_textOrange.Value();

        public static Color Bg1 => s_primaryBack.Value();

        public static Color Bg2 => s_secondaryBack.Value();

        public static Color Bg3 => s_tertiaryBack.Value();
        
        public static Color Separator => s_separatorColor.Value();
        
        public static Color Button => s_buttonColor.Value();
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
    
    private static readonly ThemedColor s_infoColor = new()
    {
        darkColor = new Color(0.82f, 0.32f, 0.54f),
        lightColor = new Color(0.82f, 0.32f, 0.54f)
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
        darkColor = new Color(0.63f, 0.63f, 0.63f),
        lightColor = new Color(0.63f, 0.63f, 0.63f)
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
    
    private static readonly ThemedColor s_buttonColor = new()
    {
        darkColor = new Color(0.35f, 0.35f, 0.35f),
        lightColor = new Color(0.35f, 0.35f, 0.35f)
    };
    
    private static readonly ThemedColor s_textRed = new()
    {
        darkColor = new Color(0.69f, 0.12f, 0.12f),
        lightColor = new Color(0.69f, 0.12f, 0.12f)
    };
    
    private static readonly ThemedColor s_textOrange = new()
    {
        darkColor = new Color(0.69f, 0.42f, 0.12f),
        lightColor = new Color(0.69f, 0.42f, 0.12f)
    };
    
    private static readonly ThemedColor s_textGreen = new()
    {
        darkColor = new Color(0.12f, 0.69f, 0.12f),
        lightColor = new Color(0.12f, 0.69f, 0.12f)
    };

    public static readonly Dictionary <string, Action <List <VisualElement>>> Overwrites = new()
    {
        // Text Color
        {Overwrite.mp_textColor_primary.ToString(), elements => {OverwriteTextColor(elements, Colors.Primary);}},
        {Overwrite.mp_textColor_normal.ToString(), elements => {OverwriteTextColor(elements, Colors.Text);}},
        {Overwrite.mp_textColor_secondary.ToString(), elements => {OverwriteTextColor(elements, Colors.TextSecondary);}},
        {Overwrite.mp_textColor_red.ToString(), elements => {OverwriteTextColor(elements, Colors.TextRed);}},
        {Overwrite.mp_textColor_green.ToString(), elements => {OverwriteTextColor(elements, Colors.TextGreen);}},
        {Overwrite.mp_textColor_orange.ToString(), elements => {OverwriteTextColor(elements, Colors.TextOrange);}},
        
        // Border Color
        {Overwrite.mp_borderColor_primary.ToString(), elements => {OverwriteBorderColor(elements, Colors.Primary);}},
        {Overwrite.mp_borderColor_bg1.ToString(), elements => {OverwriteBorderColor(elements, Colors.Bg1);}},
        {Overwrite.mp_borderColor_separator.ToString(), elements => {OverwriteBorderColor(elements, Colors.Separator);}},
        
        // Background Color
        {Overwrite.mp_bg1.ToString(), elements => {OverwriteBackgroundColor(elements, Colors.Bg1);}},
        {Overwrite.mp_bg2.ToString(), elements => {OverwriteBackgroundColor(elements, Colors.Bg2);}},
        {Overwrite.mp_bg3.ToString(), elements => {OverwriteBackgroundColor(elements, Colors.Bg3);}},
        {Overwrite.mp_bg_primary.ToString(), elements => {OverwriteBackgroundColor(elements, Colors.Primary);}},
        
        // Image Tint
        {Overwrite.mp_imageTint_primary.ToString(), elements => {OverwriteImageTint(elements, Colors.Primary);}},
        {Overwrite.mp_imageTint_textSecondary.ToString(), elements => {OverwriteImageTint(elements, Colors.TextSecondary);}},

        // Interaction
        {Overwrite.mp_interaction.ToString(), OverwriteInteraction},
        {Overwrite.mp_interaction_imageOnly.ToString(), OverwriteInteractionImageOnly},
        {Overwrite.mp_useCustomTooltip.ToString(), OverwriteTooltip},
        
        // Others
        {Overwrite.mp_listSelection_primary.ToString(), elements => {OverwriteListSelection(elements, Colors.PrimaryInteracted);}},
        {Overwrite.mp_foldout.ToString(), OverwriteFoldout},
    };

    private static void OverwriteTooltip(List <VisualElement> elements)
    {
        foreach (VisualElement element in elements)
        {
            var tooltip = "";
            
            element.RegisterCallback<MouseEnterEvent>(
                _ =>
                {
                    tooltip = element.tooltip;
                    element.tooltip = "";
                    GUIUtility.DisplayTooltip(element, tooltip);
                },
                TrickleDown.TrickleDown);
            
            element.RegisterCallback<MouseOutEvent>(
                _ =>
                {
                    element.tooltip = tooltip;
                    GUIUtility.HideTooltip(element);
                },
                TrickleDown.TrickleDown);
        }
    }

    private static void OverwriteFoldout(List <VisualElement> elements)
    {
        foreach (VisualElement element in elements)
        {
            element.Q(className: "unity-toggle").focusable = false;
            element.Q(className: "unity-toggle__input").focusable = false;
            element.Q(className: "unity-toggle__text").focusable = false;
            element.Q(className: "unity-toggle__checkmark").focusable = false;
        }   
    }
    
    private static void OverwriteListSelection(List <VisualElement> elements, Color color)
    {
        foreach (VisualElement element in elements)
        {
            if (element is not ListView list)
                continue;

            List <VisualElement> lastSelected = new();
            
            list.selectedIndicesChanged += _ =>
            {
                UQueryBuilder <VisualElement> items = element.Query(className: "unity-collection-view__item--selected");

                if (lastSelected.Count > 0)
                {
                    foreach (VisualElement visualElement in lastSelected)
                    {
                        visualElement.style.backgroundColor = new Color(0, 0, 0, 0);
                    }
                    
                    lastSelected.Clear();
                }

                items.ForEach(
                    ve =>
                    {
                        lastSelected.Add(ve);
                        ve.style.backgroundColor = color;
                    });
            };
        }
    }

    private static void OverwriteTextColor(List <VisualElement> elements, Color color)
    {
        foreach (VisualElement element in elements)
        {
            element.style.color = color;
        }
    }
    
    private static void OverwriteBorderColor(List <VisualElement> elements, Color color)
    {
        foreach (VisualElement element in elements)
        {
            GUIUtility.SetBorderColor(element, color);
        }
    }
    
    private static void OverwriteBackgroundColor(List <VisualElement> elements, Color color)
    {
        foreach (VisualElement element in elements)
        {
            element.style.backgroundColor = color;
        }
    }
    
    private static void OverwriteImageTint(List <VisualElement> elements, Color color)
    {
        foreach (VisualElement element in elements)
        {
            element.style.unityBackgroundImageTintColor = color;
        }
    }

    private static void OverwriteInteraction(List <VisualElement> elements)
    {
        foreach (VisualElement element in elements)
        {
            GUIUtility.SubscribeInteractable(element);
        }
    }
    
    private static void OverwriteInteractionImageOnly(List <VisualElement> elements)
    {
        foreach (VisualElement element in elements)
        {
            GUIUtility.SubscribeInteractableImageOnly(element);
        }
    }
}

}
