using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Editor.Scripts.PackageManager.Cache;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Editor.Scripts.GUI
{

public static class GUIUtility
{
    private static string s_linkCursorClassName = "link-cursor";
    
    public static void ActivateLinks(VisualElement root, EventCallback <PointerUpLinkTagEvent> linkCallback)
    {
        UQueryBuilder <Label> links = root.Query<Label>(className: "link");

        links.ForEach(
            link =>
            {
                link.RegisterCallback(linkCallback);
                link.RegisterCallback<PointerOverLinkTagEvent>(HyperlinkOnPointerOver);
                link.RegisterCallback<PointerOutLinkTagEvent>(HyperlinkOnPointerOut);
            });
    }

    private static void HyperlinkOnPointerOver(PointerOverLinkTagEvent evt)
    {
        var target = (VisualElement)evt.target;
        target.AddToClassList(s_linkCursorClassName);
    }
    
    private static void HyperlinkOnPointerOut(PointerOutLinkTagEvent evt)
    {
        var target = (VisualElement)evt.target;
        target.RemoveFromClassList(s_linkCursorClassName);
    }

    private static VisualElement CreateSplashScreen(VisualElement root, out VisualElement[] loadingIcon, out VisualElement logo)
    {
        var treeAsset = Resources.Load <VisualTreeAsset>("User Interface/Splash Screen");

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
}

}
