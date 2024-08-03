#if UNITY_EDITOR
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace MegaPint.Editor.Scripts.GUI.Utility
{

/// <summary> Partial utility class containing link specific utility functions </summary>
internal static partial class GUIUtility
{
    private static bool s_linkCooldown;

    #region Public Methods

    /// <summary> Activate all links in any text with the "mp_link" class </summary>
    /// <param name="root"> Root element to start the search in </param>
    /// <param name="linkCallback"> Action to invoke when any link is clicked </param>
    public static void ActivateLinks(this VisualElement root, EventCallback <PointerUpLinkTagEvent> linkCallback)
    {
        var linkColorHtml = ColorUtility.ToHtmlStringRGB(StyleSheetValues.LinkColor);
        var infoColorHtml = ColorUtility.ToHtmlStringRGB(StyleSheetValues.InfoColor);

        UQueryBuilder <TextElement> links = root.Query <TextElement>(className: "mp_link");

        links.ForEach(
            link =>
            {
                link.text = ColorLinks(linkColorHtml, infoColorHtml, link.text);
                HandleLink(link, linkCallback);
            });

        UQueryBuilder <Foldout> foldouts = root.Query <Foldout>(className: "mp_link");

        foldouts.ForEach(
            link =>
            {
                link.text = ColorLinks(linkColorHtml, infoColorHtml, link.text);
                HandleLink(link, linkCallback);
            });
    }

    #endregion

    #region Private Methods

    /// <summary> Color all links in a string by adding color tags </summary>
    /// <param name="linkColor"> Color applied to links (html color) </param>
    /// <param name="infoColor"> Color applied to info links (html color) </param>
    /// <param name="str"> String to be colored </param>
    /// <returns> Colored string </returns>
    private static string ColorLinks(string linkColor, string infoColor, string str)
    {
        var linkStarts = str.Split("<link=");
        var linkTypes = new bool[linkStarts.Length + 1];

        var builder = new StringBuilder(linkStarts[0]);

        var linkStart = $"<b><color=#{linkColor}><link=";
        var infoStart = $"<u><color=#{infoColor}><link=";

        const string LinkEnd = "</link></color></b>";
        const string InfoEnd = "</link></color></u>";

        for (var i = 1; i < linkStarts.Length; i++)
        {
            var isInfo = linkStarts[i].StartsWith("\"info_");

            linkTypes[i] = isInfo;

            builder.Append(isInfo ? infoStart : linkStart);
            builder.Append(linkStarts[i]);
        }

        var linkEnds = builder.ToString().Split("</link>");

        builder.Clear();
        builder.Append(linkEnds[0]);

        for (var i = 1; i < linkEnds.Length; i++)
        {
            builder.Append(linkTypes[i] ? InfoEnd : LinkEnd);
            builder.Append(linkEnds[i]);
        }

        return builder.ToString();
    }

    /// <summary> Handle setup of a link </summary>
    /// <param name="link"> The link element </param>
    /// <param name="linkCallback"> Action to be invoked when the link is clicked </param>
    private static void HandleLink(CallbackEventHandler link, EventCallback <PointerUpLinkTagEvent> linkCallback)
    {
        if (linkCallback != null)
        {
            link.RegisterCallback <PointerUpLinkTagEvent>(
                evt =>
                {
                    if (s_linkCooldown)
                        return;

                    s_linkCooldown = true;
                    linkCallback.Invoke(evt);

                    ReenableLinks();
                });
        }

        link.RegisterCallback <PointerOverLinkTagEvent>(HyperlinkOnPointerOver);
        link.RegisterCallback <PointerOutLinkTagEvent>(HyperlinkOnPointerOut);
    }

    /// <summary> EventCallback for when the pointer leaves the hyperlink </summary>
    /// <param name="evt"> Hyperlink event </param>
    private static void HyperlinkOnPointerOut(PointerOutLinkTagEvent evt)
    {
        var target = (VisualElement)evt.target;

        HideTooltip(target);
        target.RemoveFromClassList(StyleSheetClasses.LinkCursor);
    }

    /// <summary> EventCallback for when the pointer enters the hyperlink </summary>
    /// <param name="evt"> Hyperlink event </param>
    private static void HyperlinkOnPointerOver(PointerOverLinkTagEvent evt)
    {
        var target = (VisualElement)evt.target;

        if (evt.linkID.StartsWith("info_"))
        {
            DisplayTooltip(target, InfoLinkData.Get(evt.linkID.Replace("\"", "").Replace("info_", "")));

            return;
        }

        target.AddToClassList(StyleSheetClasses.LinkCursor);
    }

    /// <summary> Reenable all links after a certain delay </summary>
    private static async void ReenableLinks()
    {
        await Task.Delay(100);
        s_linkCooldown = false;
    }

    #endregion
}

}
#endif
