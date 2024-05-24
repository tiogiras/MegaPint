#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Utility
{

/// <summary> Partial utility class containing tooltip specific utility functions </summary>
public static partial class GUIUtility
{
    private static string s_tooltipPath;

    private static VisualElement s_tooltip;
    private static Label s_tooltipLabel;

    private static string _TooltipPath =>
        s_tooltipPath ??= Path.Combine(Constants.BasePackage.Resources.UserInterface.WindowsPath, "Tooltip");

    #region Public Methods

    /// <summary> Display a tooltip on the given element </summary>
    /// <param name="root"> Element the tooltip should be displayed on </param>
    /// <param name="tooltip"> Tooltip text </param>
    public static void DisplayTooltip(VisualElement root, string tooltip)
    {
        s_tooltip = Instantiate(Resources.Load <VisualTreeAsset>(_TooltipPath), root.GetRootVisualElement());
        s_tooltip.style.display = DisplayStyle.None;

        s_tooltipLabel = s_tooltip.Q <Label>();
        s_tooltipLabel.text = tooltip;

        s_tooltip.style.position = new StyleEnum <Position>(Position.Absolute);

        root.RegisterCallback <MouseMoveEvent>(TooltipMove);
    }

    /// <summary> Hide the tooltip from the given element </summary>
    /// <param name="root"> Element the tooltip should be removed from </param>
    public static void HideTooltip(CallbackEventHandler root)
    {
        if (s_tooltip == null)
            return;

        s_tooltip.Clear();
        s_tooltip.parent.Remove(s_tooltip);
        s_tooltip = null;

        root.UnregisterCallback <MouseMoveEvent>(TooltipMove);
    }

    #endregion

    #region Private Methods

    /// <summary> Move the tooltip </summary>
    /// <param name="evt"> The mouse event of the move </param>
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

    #endregion
}

}
#endif
