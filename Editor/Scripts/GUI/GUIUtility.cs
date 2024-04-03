using System.Threading.Tasks;
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

    public static async void DisplaySplashScreen(VisualElement root, float speed = .05f)
    {
        var treeAsset = Resources.Load <VisualTreeAsset>("User Interface/Splash Screen");

        VisualElement splashScreen = treeAsset.Instantiate();
        splashScreen.style.flexGrow = 1;
        splashScreen.style.flexShrink = 1;
        
        root.Add(splashScreen);

        VisualElement logo = splashScreen.Q("Logo");
        VisualElement[] loadingIcon = GetLoadingIcon(splashScreen.Q("Loading"));

        const int RefreshRate = 1;
        
        var loadingTime = 0f;
        var fadeInProgress = 0f;
        
        while (splashScreen != null)
        {
            if (fadeInProgress < 1)
                fadeInProgress += speed;
            
            logo.style.opacity = fadeInProgress;

            loadingTime += RefreshRate;
            HandleLoadingIcon(loadingIcon, loadingTime);

            await Task.Delay(RefreshRate);
        }
    }
    
    private static void HandleLoadingIcon(VisualElement[] elements, float loadingTime)
    {
        var cycleTime = 20f;

        for (var i = 0; i < 12; i++)
        {
            var individualTime = loadingTime + i * cycleTime / 6;
            
            /*if (individualTime < 0)
                continue;*/
            
            VisualElement element = elements[i];
            
            var fadeIn = Mathf.CeilToInt(individualTime / cycleTime) % 2 == 0;
            
            
            var rest = individualTime % cycleTime;
            
            if (rest == 0)
                continue;
            
            var opacity = Mathf.Lerp(fadeIn ? 0 : 1, fadeIn ? 1 : 0, rest / cycleTime);

            Debug.Log($"{fadeIn}: op: {opacity} mod:{individualTime % cycleTime}");

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
