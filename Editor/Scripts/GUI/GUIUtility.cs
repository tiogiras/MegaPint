using System;
using System.Collections.Generic;
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

    public static void ActivateLinks(VisualElement root, EventCallback <PointerUpLinkTagEvent> linkCallback)
    {
        UQueryBuilder <Label> links = root.Query<Label>(className: "link");

        links.ForEach(
            link =>
            {
                link.text = ColorLinks(link.text);
                
                link.RegisterCallback(linkCallback);
                link.RegisterCallback<PointerOverLinkTagEvent>(HyperlinkOnPointerOver);
                link.RegisterCallback<PointerOutLinkTagEvent>(HyperlinkOnPointerOut);
            });
    }

    private static string ColorLinks(string str)
    {
        var linkStarts = str.Split("<link=");

        var builder = new StringBuilder(linkStarts[0]);
        
        for (var i = 1; i < linkStarts.Length; i++)
        {
            builder.Append("<b><color=#D10072><link=");
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

    private static void HyperlinkOnPointerOver(PointerOverLinkTagEvent evt)
    {
        var target = (VisualElement)evt.target;
        target.AddToClassList(LinkCursorClassName);
    }
    
    private static void HyperlinkOnPointerOut(PointerOutLinkTagEvent evt)
    {
        var target = (VisualElement)evt.target;
        target.RemoveFromClassList(LinkCursorClassName);
    }

    private static VisualElement CreateSplashScreen(VisualElement root, out VisualElement[] loadingIcon, out VisualElement logo)
    {
        var treeAsset = Resources.Load <VisualTreeAsset>("MegaPint/User Interface/Windows/Splash Screen");

        VisualElement splashScreen = treeAsset.Instantiate();
        splashScreen.style.flexGrow = 1;
        splashScreen.style.flexShrink = 1;
        
        root.Add(splashScreen);
        
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

        while (!cacheRefreshed || fadeInProgress < 1)
        {
            if (fadeInProgress < 1)
            {
                fadeInProgress += speed;
                fadeInProgress = Mathf.Clamp01(fadeInProgress);
                logo.style.opacity = fadeInProgress;
            }

            loadingTime += refreshRate;
            HandleLoadingIcon(loadingIcon, loadingTime);

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
        
        element.RegisterCallback <MouseOverEvent>(
            evt =>
            {
                SetBorderColor(evt.target as VisualElement, RootElement.Colors.Primary);
            });

        element.RegisterCallback <MouseDownEvent>(
            evt =>
            {
                ((VisualElement)evt.target).style.backgroundColor =
                    RootElement.Colors.PrimaryInteracted;
            },
            TrickleDown.TrickleDown);
        
        element.RegisterCallback <MouseUpEvent>(
            evt =>
            {
                ((VisualElement)evt.target).style.backgroundColor =
                    defaultBackgroundColor;
            },
            TrickleDown.TrickleDown);

        element.RegisterCallback <MouseOutEvent>(
            evt =>
            {
                var target = (VisualElement)evt.target;
                target.Blur();
                
                target.style.backgroundColor = defaultBackgroundColor;
                SetBorderColor(target, defaultBorderColor);
            });
    }
    
    /*private void BeginHover(MouseOverEvent evt)
        {
            _hovered = true;

            var element = (VisualElement)evt.target;
            _defaultColor = element.style.backgroundColor;
            _defaultBorderColor = element.style.borderTopColor;

            Refresh(element);
        }

        private void EndHover(MouseOutEvent evt)
        {
            _hovered = false;
            _pressed = false;

            var element = (VisualElement)evt.target;
            element.Blur();

            Refresh(element);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            _pressed = true;
            Refresh(evt.target as VisualElement);
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            _pressed = false;
            Refresh(evt.target as VisualElement);
        }

        private void Refresh(VisualElement element)
        {
            GUIUtility.SetBorderColor(
                element,
                _hovered ? RootElement.Colors.Primary : _defaultBorderColor);

            element.style.backgroundColor =
                _pressed ? RootElement.Colors.PrimaryInteracted : _defaultColor;
        }*/
}

}
