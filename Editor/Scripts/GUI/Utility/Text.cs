#if UNITY_EDITOR

namespace MegaPint.Editor.Scripts.GUI.Utility
{

/// <summary> Partial utility class containing text specific utility functions </summary>
internal static partial class GUIUtility
{
    #region Public Methods

    /// <summary> Color the text by tags </summary>
    /// <param name="text"> The text to be colored </param>
    /// <param name="tag"> The tag to be replaced </param>
    /// <param name="color"> The color to be inserted  </param>
    /// <returns> The text containing the color tags </returns>
    public static string ColorByTag(this string text, string tag, string color)
    {
        var startTag = $"<{tag}>";
        var endTag = $"</{tag}>";

        text = text.Replace(startTag, $"<color={color}>");
        text = text.Replace(endTag, "</color>");

        return text;
    }

    /// <summary> Remove a tag from the given text </summary>
    /// <param name="text"> The text to be cleaned </param>
    /// <param name="tag"> The tag to be removed </param>
    /// <returns> Cleaned text </returns>
    public static string RemoveTag(this string text, string tag)
    {
        var startTag = $"<{tag}>";
        var endTag = $"</{tag}>";

        text = text.Replace(startTag, "");
        text = text.Replace(endTag, "");

        return text;
    }

    #endregion
}

}
#endif
