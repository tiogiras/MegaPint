namespace MegaPint.Editor.Scripts.GUI.Utility
{

public static partial class GUIUtility
{
    public static void ColorByTag(ref string text, string tag, string color)
    {

    }

    private static void ColorByTag(this string text, string tag, string color)
    {
        var startTag = $"<{tag}>";
        var endTag = $"</{tag}>";
        
        var temp = text.Replace(startTag, $"<color={color}>");
        temp = temp.Replace(endTag, "</color>");

        text = temp;

        text = text.Replace(startTag, $"<color={color}>");
        text = text.Replace(endTag, "</color>");
    }
}

}
