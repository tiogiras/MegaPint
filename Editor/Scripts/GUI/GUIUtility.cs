using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor.Scripts.PackageManager.Cache;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Editor.Scripts.GUI
{

public static class GUIUtility
{
    private const string LinkCursorClassName = "link-cursor";

    private const string TooltipPath = "MegaPint/User Interface/Windows/Tooltip";

    public static VisualElement Instantiate(VisualTreeAsset asset, VisualElement root = null)
    {
        TemplateContainer element = asset.Instantiate();
        ApplyTheme(element);

        root?.Add(element);

        return element;
    }
    
    public static void ActivateLinks(VisualElement root, EventCallback <PointerUpLinkTagEvent> linkCallback)
    {
        UQueryBuilder <Label> links = root.Query<Label>(className: "mp_link");

        links.ForEach(
            link =>
            {
                link.text = ColorLinks(link.text);
                
                if (linkCallback != null)
                    link.RegisterCallback(linkCallback);
                
                link.RegisterCallback<PointerOverLinkTagEvent>(HyperlinkOnPointerOver);
                link.RegisterCallback<PointerOutLinkTagEvent>(HyperlinkOnPointerOut);
            });
    }

    private static string ColorLinks(string str)
    {
        var linkStarts = str.Split("<link=");

        var builder = new StringBuilder(linkStarts[0]);

        var linkStart = $"<b><color=#{ColorUtility.ToHtmlStringRGB(RootElement.Colors.Link)}><link=";
        var infoStart = $"<b><color=#{ColorUtility.ToHtmlStringRGB(RootElement.Colors.Info)}><link=";
        
        for (var i = 1; i < linkStarts.Length; i++)
        {
            builder.Append(linkStarts[i].StartsWith("\"info_") ? infoStart : linkStart);
            builder.Append(linkStarts[i]);
        }
        
        var linkEnds = builder.ToString().Split("</link>");

        builder.Clear();
        builder.Append(linkEnds[0]);

        for (var i = 1; i < linkEnds.Length; i++)
        {
            builder.Append("</link></color></b>");
            builder.Append(linkEnds[i]);
        }

        return builder.ToString();
    }

    public static void DisplayTooltip(VisualElement root, string tooltip)
    {
        s_tooltip = Instantiate(Resources.Load <VisualTreeAsset>(TooltipPath), GetRootVisualElement(root));
        s_tooltip.style.display = DisplayStyle.None;
        
        s_tooltipLabel = s_tooltip.Q <Label>();
        s_tooltipLabel.text = tooltip;
        
        s_tooltip.style.position = new StyleEnum <Position>(Position.Absolute);
        
        root.RegisterCallback<MouseMoveEvent>(TooltipMove);
    }

    public static void HideTooltip(CallbackEventHandler root)
    {
        if (s_tooltip == null)
            return;
        
        s_tooltip.Clear();
        s_tooltip.parent.Remove(s_tooltip);
        s_tooltip = null;
        
        root.UnregisterCallback<MouseMoveEvent>(TooltipMove);
    }

    private static void TooltipMove(MouseMoveEvent evt)
    {
        s_tooltip.style.display = DisplayStyle.Flex;

        Vector2 mousePos = evt.mousePosition;
        var contentWidth = s_tooltipLabel.contentRect.width;

        s_tooltip.style.opacity = contentWidth == 0 ? 0 : 1;
        
        if (contentWidth == 0)
            return;

        if (contentWidth + mousePos.x > s_tooltip.parent.contentRect.width)
            s_tooltip.transform.position = mousePos - new Vector2(contentWidth, 0);
        else
            s_tooltip.transform.position = mousePos;
    }

    private static VisualElement s_tooltip;
    private static Label s_tooltipLabel;

    private static VisualElement GetRootVisualElement(VisualElement root)
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

    private static void HyperlinkOnPointerOver(PointerOverLinkTagEvent evt)
    {
        var target = (VisualElement)evt.target;
        
        if (evt.linkID.StartsWith("info_"))
        {
            DisplayTooltip(target, InfoLinkData.Get(evt.linkID.Replace("\"", "").Replace("info_", "")));
            return;
        }

        target.AddToClassList(LinkCursorClassName);
    }
    
    private static void HyperlinkOnPointerOut(PointerOutLinkTagEvent evt)
    {
        var target = (VisualElement)evt.target;
        
        HideTooltip(target);
        target.RemoveFromClassList(LinkCursorClassName);
    }

    private static VisualElement CreateSplashScreen(VisualElement root, out VisualElement[] loadingIcon, out VisualElement logo)
    {
        var treeAsset = Resources.Load <VisualTreeAsset>("MegaPint/User Interface/Windows/Splash Screen");

        VisualElement splashScreen = Instantiate(treeAsset, root);
        splashScreen.style.flexGrow = 1;
        splashScreen.style.flexShrink = 1;

        loadingIcon = GetLoadingIcon(splashScreen.Q("Loading"));
        logo = splashScreen.Q("Logo");

        return splashScreen;
    }
    
    public static async void DisplaySplashScreen(VisualElement root, Action method, int refreshRate = 1, float speed = .05f)
    {
        root.Clear();
        
        VisualElement splashScreen = CreateSplashScreen(
            root,
            out VisualElement[] loadingIcon,
            out VisualElement logo);

        var cacheRefreshed = false;
        
        PackageCache.onCacheRefreshed += () => {cacheRefreshed = true;};

        var loadingTime = 0;
        var fadeInProgress = 0f;
        
        var targetProgress = 0f;
        var currentProgress = 0f;
        
        PackageCache.onCacheProgressChanged += progress => {targetProgress = progress;};
        
        var progressBar = splashScreen.Q <VisualElement>("Progress");
        progressBar.style.width = 0;

        var processText = splashScreen.Q <Label>("Process");

        PackageCache.onCacheProcessChanged += process => {processText.text = process;};

        while (!cacheRefreshed || fadeInProgress < 1 || Math.Abs(targetProgress - currentProgress) > 0.001f)
        {
            if (fadeInProgress < 1)
            {
                fadeInProgress += speed;
                fadeInProgress = Mathf.Clamp01(fadeInProgress);
                logo.style.opacity = fadeInProgress;
            }

            loadingTime += refreshRate;
            HandleLoadingIcon(loadingIcon, loadingTime);

            currentProgress = Mathf.Lerp(currentProgress, targetProgress, refreshRate * speed * 4);
            currentProgress = Mathf.Clamp01(currentProgress);
            
            progressBar.style.width = Length.Percent(currentProgress * 100);

            await Task.Delay(refreshRate);
        }

        splashScreen.RemoveFromHierarchy();

        method?.Invoke();
    }

    private static void HandleLoadingIcon(IReadOnlyList <VisualElement> elements, int loadingTime)
    {
        const float CycleTime = 20f;

        for (var i = 0; i < 12; i++)
        {
            var individualTime = loadingTime + i * CycleTime / 6;

            VisualElement element = elements[i];
            
            var fadeIn = Mathf.CeilToInt(individualTime / CycleTime) % 2 == 0;
            
            
            var rest = individualTime % CycleTime;
            
            if (rest == 0)
                continue;
            
            var opacity = Mathf.Lerp(fadeIn ? 0 : 1, fadeIn ? 1 : 0, rest / CycleTime);

            element.style.opacity = opacity;
        }
    }

    private static VisualElement[] GetLoadingIcon(VisualElement root)
    {
        VisualElement[] elements = new VisualElement[12];

        for (var i = 1; i <= 12; i++)
        {
            VisualElement element = root.Q($"Loading{i}");
            elements[i - 1] = element;
            element.style.opacity = 0;
        }

        return elements;
    }

    public static void AddClickInteraction(VisualElement element, Action action)
    {
        StyleColor defaultColor = element.style.unityBackgroundImageTintColor;
        
        element.RegisterCallback<MouseDownEvent>(
            _ =>
            {
                defaultColor = element.style.unityBackgroundImageTintColor;
                element.style.unityBackgroundImageTintColor = RootElement.Colors.PrimaryInteracted;
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
    
    public static void SetBorderWidth(VisualElement element, float width)
    {
        element.style.borderTopWidth = width;
        element.style.borderRightWidth = width;
        element.style.borderBottomWidth = width;
        element.style.borderLeftWidth = width;
    }
    
    public static void SetBorderColor(VisualElement element, Color color)
    {
        SetBorderColor(element, (StyleColor)color);
    }
    
    public static void SetBorderColor(VisualElement element, StyleColor color)
    {
        element.style.borderTopColor = color;
        element.style.borderRightColor = color;
        element.style.borderBottomColor = color;
        element.style.borderLeftColor = color;
    }
    
    public static void SetBorderRadius(VisualElement element, float radius)
    {
        element.style.borderTopRightRadius = radius;
        element.style.borderTopLeftRadius = radius;
        element.style.borderBottomRightRadius = radius;
        element.style.borderBottomLeftRadius = radius;
    }
    
    public static void SetMargin(VisualElement element, float margin)
    {
        element.style.marginTop = margin;
        element.style.marginRight = margin;
        element.style.marginBottom = margin;
        element.style.marginLeft = margin;
    }

    public static void ApplyTheme(VisualElement root)
    {
        foreach (var overwrite in Enum.GetNames(typeof(Overwrite)))
        {
            List <VisualElement> result = root.Query(className: overwrite).ToList();

            if (result.Count == 0)
                continue;
            
            RootElement.Overwrites[overwrite]?.Invoke(result);
        }
    }

    public static void SubscribeInteractable(VisualElement element)
    {
        StyleColor defaultBackgroundColor = element.style.backgroundColor;
        StyleColor defaultBorderColor = element.style.color;

        var hovered = false;
        var pressed = false;
        var focused = false;
        var interacted = false;

        var onlyLoseFocusOnBlur = element.ClassListContains("mp_interaction_onlyLoseFocusOnBlur");
        var checkColorOnMouseUp = element.ClassListContains("mp_interaction_checkColorOnMouseUp");
        
        element.RegisterCallback <MouseOverEvent>(
            evt =>
            {
                var target = (VisualElement)evt.target;

                if ((!onlyLoseFocusOnBlur || !focused) && !hovered)
                {
                    defaultBackgroundColor = target.style.backgroundColor;
                    defaultBorderColor = target.style.borderTopColor;   
                }
                
                hovered = true;
                interacted = false;

                if (pressed)
                    target.style.backgroundColor = RootElement.Colors.PrimaryInteracted;
                
                SetBorderColor(target, RootElement.Colors.Primary);
            });

        element.RegisterCallback <MouseDownEvent>(
            evt =>
            {
                pressed = true;

                ((VisualElement)evt.target).style.backgroundColor =
                    RootElement.Colors.PrimaryInteracted;
            },
            TrickleDown.TrickleDown);
        
        element.RegisterCallback <MouseUpEvent>(
            evt =>
            {
                pressed = false;
                focused = true;
                interacted = true;

                var target = (VisualElement)evt.target;
                
                target.style.backgroundColor = defaultBackgroundColor;

                if (!hovered && checkColorOnMouseUp)
                    SetBorderColor(target, defaultBorderColor);
            },
            TrickleDown.TrickleDown);

        element.RegisterCallback <MouseOutEvent>(
            evt =>
            {
                if (onlyLoseFocusOnBlur && focused)
                    return;
                
                var target = (VisualElement)evt.target;
                target.Blur();

                hovered = false;
                
                if (!target.ClassListContains("mp_interaction_dontChangeColorAfterInteract") || !interacted)
                    target.style.backgroundColor = defaultBackgroundColor;
                
                SetBorderColor(target, defaultBorderColor);
            });
        
        if (!onlyLoseFocusOnBlur)
            return;
        
        element.RegisterCallback <BlurEvent>(
            evt =>
            {
                if (!focused)
                    return;
                
                focused = false;
                hovered = false;

                var target = (VisualElement)evt.target;
                
                target.style.backgroundColor = defaultBackgroundColor;

                SetBorderColor(target, defaultBorderColor);
            });
    }

    public static void SubscribeInteractableImageOnly(VisualElement element)
    {
        StyleColor defaultColor = element.style.unityBackgroundImageTintColor;
        
        element.RegisterCallback <MouseEnterEvent>(
            evt =>
            {
                ((VisualElement)evt.target).style.unityBackgroundImageTintColor =
                    RootElement.Colors.Primary;
            });

        element.RegisterCallback <MouseOutEvent>(
            evt =>
            {
                ((VisualElement)evt.target).style.unityBackgroundImageTintColor = defaultColor;
            });
    }
}

}
