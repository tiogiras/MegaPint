using System;
using System.Collections.Generic;
using Editor.Scripts.Settings;
using UnityEditor;
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

    private static MegaPintSettingsBase s_settings;

    private static MegaPintSettingsBase _Settings
    {
        get
        {
            Debug.Log(MegaPintSettings.instance); // TODO THIS IS NULL

            return s_settings ??= MegaPintSettings.instance.GetSetting("General");
        }
    }

    public static string CaretColorClass =>
        _Settings.GetValue("EditorTheme", 0) switch
        {
            0 => EditorGUIUtility.isProSkin ? "mp_caretColor_dark" : "mp_caretColor_light",
            1 => "mp_caretColor_dark",
            2 => "mp_caretColor_light",
            var _ => "mp_caretColor_dark"
        };

    private class ThemedColor
    {
        public Color darkColor;
        public Color lightColor;
        #region Public Methods

        public Color Value()
        {
            Debug.Log(_Settings.GetValue("EditorTheme", 0));
            
            return _Settings.GetValue("EditorTheme", 0) switch
                   {
                       0 => EditorGUIUtility.isProSkin ? darkColor : lightColor,
                       1 => darkColor,
                       2 => lightColor,
                       var _ => darkColor
                   };
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
        darkColor = new Color(0.94f, 0.94f, 0.94f),
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
        lightColor = new Color(0.54f, 0.54f, 0.54f)
    };

    private static readonly ThemedColor s_secondaryBack = new()
    {
        darkColor = new Color(0.15f, 0.15f, 0.15f),
        lightColor = new Color(0.65f, 0.65f, 0.65f)
    };

    private static readonly ThemedColor s_tertiaryBack = new()
    {
        darkColor = new Color(0.19f, 0.19f, 0.19f),
        lightColor = new Color(0.78f, 0.78f, 0.78f)
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
        {Overwrite.mp_listSelection_bg3.ToString(), elements => {OverwriteListSelection(elements, Colors.Bg3);}},
        {Overwrite.mp_foldout.ToString(), OverwriteFoldout},
        {Overwrite.mp_toggle.ToString(), OverwriteToggle},
        {Overwrite.mp_dropdown.ToString(), OverwriteDropdown},
        {Overwrite.mp_inputField.ToString(), OverwriteInputField},
        {Overwrite.mp_colorField.ToString(), OverwriteColorField},
        {Overwrite.mp_objectField.ToString(), OverwriteObjectField},
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
        /*foreach (VisualElement element in elements)
        {
            element.Q(className: "unity-toggle").focusable = false;
            element.Q(className: "unity-toggle__input").focusable = false;
            element.Q(className: "unity-toggle__text").focusable = false;
            element.Q(className: "unity-toggle__checkmark").focusable = false;
        }  */ 
    }  
    
    private static void OverwriteDropdown(List <VisualElement> elements)
    {
        /*foreach (VisualElement element in elements)
        {
            element.focusable = false;

            VisualElement label = element.Q(className: "unity-base-field__label");
            
            if (label == null)
                continue;

            label.style.color = Colors.TextSecondary;
            label.focusable = false;
        }  */ 
    }     
    
    private static void OverwriteInputField(List <VisualElement> elements)
    {
        /*foreach (VisualElement element in elements)
        {
            VisualElement label = element.Q(className: "unity-base-field__label");
            VisualElement inputElement = element.Q(className: "unity-base-text-field__input");
            
            switch (element)
            {
                case IntegerField intField:
                    intField.selectAllOnFocus = false;

                    break;

                case FloatField floatField:
                    floatField.selectAllOnFocus = false;

                    break;

                case TextField textField:
                    textField.selectAllOnFocus = false;

                    break;
            }

            inputElement.focusable = false;
            label.focusable = false;

            GUIUtility.BorderColor defaultBorderColor = GUIUtility.GetBorderColor(inputElement);
            
            label.style.color = Colors.TextSecondary;

            var focused = false;
            var hovered = false;
            
            element.RegisterCallback<PointerEnterEvent>(
                evt =>
                {
                    hovered = true;

                    GUIUtility.SetBorderColor(inputElement, Colors.Primary);
                });
            
            element.RegisterCallback<FocusEvent>(
                evt =>
                {
                    focused = true;
                    
                    GUIUtility.SetBorderColor(inputElement, Colors.Primary);
                });
            
            element.RegisterCallback<PointerLeaveEvent>(
                evt =>
                {
                    hovered = false;
                    
                    if (focused)
                        return;
                    
                    GUIUtility.SetBorderColor(inputElement, defaultBorderColor);
                });
            
            element.RegisterCallback<BlurEvent>(
                evt =>
                {
                    focused = false;
                    
                    if (hovered)
                        return;
                    
                    GUIUtility.SetBorderColor(inputElement, defaultBorderColor);
                });
        }   */
    }

    private static void OverwriteObjectField(List <VisualElement> elements)
    {
        /*foreach (VisualElement element in elements)
        {
            VisualElement label = element.Q(className: "unity-base-field__label");
            VisualElement inputElement = element.Q(className: "unity-base-field__input");

            label.style.color = Colors.TextSecondary;
            
            GUIUtility.BorderColor defaultBorderColor = GUIUtility.GetBorderColor(inputElement);
            
            element.RegisterCallback <PointerEnterEvent>(
                _ =>
                {
                    GUIUtility.SetBorderColor(inputElement, Colors.Primary);
                });

            element.RegisterCallback<PointerLeaveEvent>(
                _ =>
                {
                    GUIUtility.SetBorderColor(inputElement, defaultBorderColor);
                });
        }*/
    }
    
    private static void OverwriteColorField(List <VisualElement> elements)
    {
        /*foreach (VisualElement element in elements)
        {
            VisualElement label = element.Q(className: "unity-base-field__label");
            VisualElement inputElement = element.Q(className: "unity-base-field__input");

            inputElement.style.height = 18;

            var fieldNameIdentifier = $"MegaPintColorField{elements.IndexOf(element)}";
            var target = (IMGUIContainer)inputElement;

            var currentCheck = 0;
            var hasEyeUpdate = false;
            var wouldRemove = false;
            
            Action action = target.onGUIHandler;

            target.onGUIHandler = () =>
            {
                if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "EyeDropperUpdate")
                    hasEyeUpdate = true;

                if (currentCheck == 3)
                {
                    if (UnityEngine.GUI.GetNameOfFocusedControl() == fieldNameIdentifier && !hasEyeUpdate)
                    {
                        if (wouldRemove)
                        {
                            UnityEngine.GUI.FocusControl(null);
                            
                            wouldRemove = false;
                        }
                        else
                            wouldRemove = true;
                    }

                    hasEyeUpdate = false;
                    currentCheck = 0;
                }
                else
                {
                    currentCheck++;
                }

                UnityEngine.GUI.SetNextControlName(fieldNameIdentifier);
                action?.Invoke();
            };

            label.style.color = Colors.TextSecondary;
            
            GUIUtility.SetBorderWidth(inputElement, 1);
            GUIUtility.SetBorderRadius(inputElement, 2);
            GUIUtility.SetBorderColor(inputElement, Colors.Bg1);

            element.RegisterCallback <PointerEnterEvent>(
                _ =>
                {
                    GUIUtility.SetBorderColor(inputElement, Colors.Primary);
                });

            element.RegisterCallback<PointerLeaveEvent>(
                _ =>
                {
                    GUIUtility.SetBorderColor(inputElement, Colors.Bg1);
                });
        }   */
    }  
    
    private static void OverwriteToggle(List <VisualElement> elements)
    {
        /*foreach (VisualElement element in elements)
        {
            VisualElement input = element.Q(className: "unity-toggle__input");
            VisualElement checkmark = input.Q(className: "unity-toggle__checkmark");
            VisualElement label = element.Q(className: "unity-toggle__label");

            input.focusable = false;
            label.focusable = false;
            
            label.style.color = Colors.TextSecondary;

            GUIUtility.BorderColor defaultBorderColor = GUIUtility.GetBorderColor(checkmark);

            var pressed = false;
            var hovered = false;
            
            element.RegisterCallback<PointerEnterEvent>(
                evt =>
                {
                    GUIUtility.SetBorderWidth(checkmark, 1);
                    GUIUtility.SetBorderRadius(checkmark, 2);
                    GUIUtility.SetBorderColor(checkmark, Colors.Primary);

                    hovered = true;
                });
            
            element.RegisterCallback<PointerLeaveEvent>(
                evt =>
                {
                    hovered = false;
                    
                    if (pressed)
                        return;

                    GUIUtility.SetBorderWidth(checkmark, 0);
                    GUIUtility.SetBorderRadius(checkmark, 0);
                    GUIUtility.SetBorderColor(checkmark, defaultBorderColor);
                });
            
            element.RegisterCallback<PointerDownEvent>(
                evt =>
                {
                    GUIUtility.SetBorderColor(checkmark, Colors.PrimaryInteracted);
                    
                    pressed = true;
                }, TrickleDown.TrickleDown);
            
            element.RegisterCallback<PointerUpEvent>(
                evt =>
                {
                    pressed = false;

                    if (hovered)
                    {
                        GUIUtility.SetBorderColor(checkmark, Colors.Primary);
                        return;   
                    }

                    GUIUtility.SetBorderWidth(checkmark, 0);
                    GUIUtility.SetBorderRadius(checkmark, 0);
                    GUIUtility.SetBorderColor(checkmark, defaultBorderColor);
                });
        }*/
    }

    private static void OverwriteListSelection(List <VisualElement> elements, Color color)
    {
        /*foreach (VisualElement element in elements)
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
                        visualElement.style.backgroundColor = StyleKeyword.Null;
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
        }*/
    }

    private static void OverwriteTextColor(List <VisualElement> elements, Color color)
    {
        /*foreach (VisualElement element in elements)
        {
            element.style.color = color;
        }*/
    }
    
    private static void OverwriteBorderColor(List <VisualElement> elements, Color color)
    {
        /*foreach (VisualElement element in elements)
        {
            GUIUtility.SetBorderColor(element, color);
        }*/
    }
    
    private static void OverwriteBackgroundColor(List <VisualElement> elements, Color color)
    {
        /*foreach (VisualElement element in elements)
        {
            element.style.backgroundColor = color;
        }*/
    }
    
    private static void OverwriteImageTint(List <VisualElement> elements, Color color)
    {
        /*foreach (VisualElement element in elements)
        {
            element.style.unityBackgroundImageTintColor = color;
        }*/
    }

    private static void OverwriteInteraction(List <VisualElement> elements)
    {
        /*foreach (VisualElement element in elements)
        {
            GUIUtility.SubscribeInteractable(element);
        }*/
    }
    
    private static void OverwriteInteractionImageOnly(List <VisualElement> elements)
    {
        /*foreach (VisualElement element in elements)
        {
            GUIUtility.SubscribeInteractableImageOnly(element);
        }*/
    }
}

}
