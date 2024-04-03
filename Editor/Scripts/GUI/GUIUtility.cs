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
}

}
