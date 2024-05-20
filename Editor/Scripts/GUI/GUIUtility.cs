using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.Windows;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Editor.Scripts.GUI
{

public static class GUIUtility
{
    private const string LinkCursorClassName = "link-cursor";

    private const string TooltipPath = "MegaPint/User Interface/Windows/Tooltip";

    private static bool s_linkCooldown;

    public static VisualElement Instantiate(VisualTreeAsset asset, VisualElement root = null)
    {
        TemplateContainer element = asset.Instantiate();

        root?.Add(element);

        return element;
    }
    
    public static void ActivateLinks(VisualElement root, EventCallback <PointerUpLinkTagEvent> linkCallback)
    {
        UQueryBuilder <TextElement> links = root.Query<TextElement>(className: "mp_link");

        links.ForEach(
            link =>
            {
                link.text = ColorLinks(link.text);
                HandleLink(link, linkCallback);
            });
        
        UQueryBuilder <Foldout> foldouts = root.Query<Foldout>(className: "mp_link");

        foldouts.ForEach(
            link =>
            {
                link.text = ColorLinks(link.text);
                HandleLink(link, linkCallback);
            });
    }

    private static void HandleLink(CallbackEventHandler link, EventCallback <PointerUpLinkTagEvent> linkCallback)
    {
        if (linkCallback != null)
        {
            link.RegisterCallback<PointerUpLinkTagEvent>(
                evt =>
                {
                    if (s_linkCooldown)
                        return;
                    
                    s_linkCooldown = true;
                    linkCallback.Invoke(evt);
                    
                    ReenableLinks();
                });
        }

        link.RegisterCallback<PointerOverLinkTagEvent>(HyperlinkOnPointerOver);
        link.RegisterCallback<PointerOutLinkTagEvent>(HyperlinkOnPointerOut);
    }

    private static async void ReenableLinks()
    {
        await Task.Delay(100);
        s_linkCooldown = false;
    }

    private static string ColorLinks(string str)
    {
        var linkStarts = str.Split("<link=");

        var builder = new StringBuilder(linkStarts[0]);

        // TODO Get colors from stylesheet
        var linkStart = $"<b><color=#{ColorUtility.ToHtmlStringRGB(Color.magenta)}><link=";
        var infoStart = $"<b><color=#{ColorUtility.ToHtmlStringRGB(Color.magenta)}><link=";
        
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
        if (s_tooltip == null)
            return;
        
        s_tooltip.style.display = DisplayStyle.Flex;

        Vector2 mousePos = evt.mousePosition;
        var contentWidth = s_tooltipLabel.contentRect.width;
        var contentHeight = s_tooltipLabel.contentRect.height;

        s_tooltip.style.opacity = contentWidth == 0 ? 0 : 1;
        
        if (contentWidth == 0)
            return;

        Vector2 targetPos = mousePos;
        
        if (contentWidth + mousePos.x > s_tooltip.parent.contentRect.width)
            targetPos -= new Vector2(contentWidth + 10, 0);

        if (contentHeight + mousePos.y > s_tooltip.parent.contentRect.height)
            targetPos -= new Vector2(0, contentHeight + 25);
        
        s_tooltip.transform.position = targetPos;
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
    
    public static BorderWidth GetBorderWidth(VisualElement element)
    {
        return new BorderWidth
        {
            top = element.style.borderTopWidth.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderTopWidth,
            bottom = element.style.borderBottomWidth.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderBottomWidth,
            left = element.style.borderLeftWidth.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderLeftWidth,
            right = element.style.borderRightWidth.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderRightWidth,
        };
    }
    
    public static void SetBorderWidth(VisualElement element, float width)
    {
        element.style.borderTopWidth = width;
        element.style.borderRightWidth = width;
        element.style.borderBottomWidth = width;
        element.style.borderLeftWidth = width;
    }
    
    public static void SetBorderWidth(VisualElement element, BorderWidth width)
    {
        element.style.borderTopWidth = width.top.keyword == StyleKeyword.Null ? StyleKeyword.Null : width.top;
        element.style.borderBottomWidth = width.bottom.keyword == StyleKeyword.Null ? StyleKeyword.Null : width.bottom;
        element.style.borderLeftWidth = width.left.keyword == StyleKeyword.Null ? StyleKeyword.Null : width.left;
        element.style.borderRightWidth = width.right.keyword == StyleKeyword.Null ? StyleKeyword.Null : width.right;
    }
    
    public static void SetBorderColor(VisualElement element, Color color)
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

    public static BorderColor GetBorderColor(VisualElement element)
    {
        return new BorderColor
        {
            top = element.style.borderTopColor.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderTopColor,
            bottom = element.style.borderBottomColor.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderBottomColor,
            left = element.style.borderLeftColor.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderLeftColor,
            right = element.style.borderRightColor.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderRightColor
        };
    }

    public static void SetBorderColor(VisualElement element, BorderColor color)
    {
        element.style.borderTopColor = color.top.keyword == StyleKeyword.Null ? StyleKeyword.Null : color.top;
        element.style.borderBottomColor = color.bottom.keyword == StyleKeyword.Null ? StyleKeyword.Null : color.bottom;
        element.style.borderLeftColor = color.left.keyword == StyleKeyword.Null ? StyleKeyword.Null : color.left;
        element.style.borderRightColor = color.right.keyword == StyleKeyword.Null ? StyleKeyword.Null : color.right;
    }
    
    public static void SetBorderColor(VisualElement element, StyleColor color)
    {
        element.style.borderTopColor = color;
        element.style.borderRightColor = color;
        element.style.borderBottomColor = color;
        element.style.borderLeftColor = color;
    }
    
    public static BorderRadius GetBorderRadius(VisualElement element)
    {
        return new BorderRadius
        {
            topLeft = element.style.borderTopLeftRadius.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderTopLeftRadius,
            topRight = element.style.borderTopRightRadius.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderTopRightRadius,
            bottomLeft = element.style.borderBottomLeftRadius.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderBottomLeftRadius,
            bottomRight = element.style.borderBottomRightRadius.keyword == StyleKeyword.Null ? StyleKeyword.Null : element.style.borderBottomRightRadius,
        };
    }
    
    public static void SetBorderRadius(VisualElement element, float radius)
    {
        element.style.borderTopLeftRadius = radius;
        element.style.borderTopRightRadius = radius;
        element.style.borderBottomLeftRadius = radius;
        element.style.borderBottomRightRadius = radius;
    }
    
    public static void SetBorderRadius(VisualElement element, BorderRadius radius)
    {
        element.style.borderTopLeftRadius = radius.topLeft.keyword == StyleKeyword.Null ? StyleKeyword.Null : radius.topLeft;
        element.style.borderTopRightRadius = radius.topRight.keyword == StyleKeyword.Null ? StyleKeyword.Null : radius.topRight;
        element.style.borderBottomLeftRadius = radius.bottomLeft.keyword == StyleKeyword.Null ? StyleKeyword.Null : radius.bottomLeft;
        element.style.borderBottomRightRadius = radius.bottomRight.keyword == StyleKeyword.Null ? StyleKeyword.Null : radius.bottomRight;
    }
    
    public static void SetMargin(VisualElement element, float margin)
    {
        element.style.marginTop = margin;
        element.style.marginRight = margin;
        element.style.marginBottom = margin;
        element.style.marginLeft = margin;
    }
    
    public static void SetPadding(VisualElement element, float padding)
    {
        element.style.paddingTop = padding;
        element.style.paddingRight = padding;
        element.style.paddingBottom = padding;
        element.style.paddingLeft = padding;
    }

    public static Action onForceRepaint;

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
}

}
