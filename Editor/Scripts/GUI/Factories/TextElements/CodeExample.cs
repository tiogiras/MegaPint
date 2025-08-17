#if UNITY_EDITOR
using System;
using System.Text;
using MegaPint.Editor.Scripts.GUI.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.TextElements
{
/// <summary> Uxml factory to create a <see cref="VisualElement" /> that can hold, color and copy the given code </summary>
[UxmlElement]
internal partial class CodeExample : VisualElement
{
    private readonly string _basicColorString;
    private readonly string _classColorString;
    private readonly string _commentColorString;
    private readonly string _methodColorString;
    private readonly string _stringColorString;

    public CodeExample()
    {
        _classColorString = $"#{ColorUtility.ToHtmlStringRGB(StyleSheetValues.CodeClassColor)}";
        _methodColorString = $"#{ColorUtility.ToHtmlStringRGB(StyleSheetValues.CodeMethodColor)}";
        _basicColorString = $"#{ColorUtility.ToHtmlStringRGB(StyleSheetValues.CodeBasicColor)}";
        _commentColorString = $"#{ColorUtility.ToHtmlStringRGB(StyleSheetValues.CodeCommentColor)}";
        _stringColorString = $"#{ColorUtility.ToHtmlStringRGB(StyleSheetValues.CodeStringColor)}";

        RegisterCallback <AttachToPanelEvent>(_ => BuildUI());
    }

    [UxmlAttribute("_string-attr")] public string Text {get; set;}

    private void BuildUI()
    {
        AddToClassList(StyleSheetClasses.Background.Color.Secondary);
        AddToClassList(StyleSheetClasses.Border.Color.Black);

        this.SetPadding(5f);
        this.SetBorderWidth(1f);
        this.SetBorderRadius(3f);

        Clear();

        var label = new Label(Text);
        Add(label);

        label.style.whiteSpace = WhiteSpace.Normal;
        label.AddToClassList(StyleSheetClasses.Text.Color.Primary);

        var cleanText = CleanText(label.text);

        // Replace any \n that cannot be detected automatically and set it to an actual \n
        label.text = label.text.Replace("\\n", "\n");

        label.text = ApplySpacing(ColorText(label.text));

        var button = new Button(() => {EditorGUIUtility.systemCopyBuffer = cleanText;});

        Add(button);

        button.text = "";
        button.tooltip = "Copy to clipboard";

        button.style.backgroundImage = Resources.Load <Texture2D>(Constants.BasePackage.Images.CopyToClipboard);

        button.style.position = Position.Absolute;
        button.style.right = 5f;
        button.style.bottom = 5f;
        button.style.width = 20f;
        button.style.height = 20f;

        button.AddToClassList(StyleSheetClasses.Button);

        button.SetMargin(0f);
        button.SetPadding(0f);
    }

    #region Private Methods

    /// <summary> Change the designed spacing characters of the text into real spacing </summary>
    /// <param name="text"> Text to be changed </param>
    /// <param name="forClipboard"> If the text should be prepared for the clipboard </param>
    /// <returns> Text with corrected spacing </returns>
    private static string ApplySpacing(string text, bool forClipboard = false)
    {
        var parts = text.Split("[/");

        var output = new StringBuilder(parts[0]);

        for (var i = 1; i < parts.Length; i++)
        {
            var part = parts[i];

            var args = part.Split("/]");

            if (!int.TryParse(args[0], out var indentLevel))
                return text;

            var count = indentLevel * (forClipboard ? 2 : 4);

            var space = new StringBuilder(count);

            for (var j = 0; j < count; j++)
                space.Append(forClipboard ? "/t" : " ");

            output.Append(space);

            output.Append(args[1]);
        }

        return output.ToString();
    }

    /// <summary> Clean the text of all tags </summary>
    /// <param name="text"> Text that should be cleaned </param>
    /// <returns> Cleaned text </returns>
    private static string CleanText(string text)
    {
        text = text.RemoveTag("c");
        text = text.RemoveTag("m");
        text = text.RemoveTag("b");
        text = text.RemoveTag("cc");
        text = text.RemoveTag("s");

        text = text.Replace("\\n", Environment.NewLine);

        return ApplySpacing(text);
    }

    /// <summary> Color the text based on the tags </summary>
    /// <param name="text"> Text that should be colored </param>
    /// <returns> Colored text </returns>
    private string ColorText(string text)
    {
        text = text.ColorByTag("c", _classColorString);
        text = text.ColorByTag("m", _methodColorString);
        text = text.ColorByTag("b", _basicColorString);
        text = text.ColorByTag("cc", _commentColorString);
        text = text.ColorByTag("s", _stringColorString);

        return text;
    }

    #endregion
}
}
#endif
