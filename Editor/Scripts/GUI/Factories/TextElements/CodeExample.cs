#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.GUIUtility;

namespace MegaPint.Editor.Scripts.GUI.Factories.TextElements
{

/// <summary> Uxml factory to create a <see cref="VisualElement" /> that can hold, color and copy the given code </summary>
public class CodeExample : VisualElement
{
    public new class UxmlFactory : UxmlFactory <CodeExample, UxmlTraits>
    {
    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private readonly UxmlStringAttributeDescription _text = new() {name = "_string-attr"};
        private string _basicColorString;

        private string _classColorString;
        private string _commentColorString;
        private string _methodColorString;
        private string _stringColorString;

        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);

            _classColorString = $"#{ColorUtility.ToHtmlStringRGB(StyleSheetValues.CodeClassColor)}";
            _methodColorString = $"#{ColorUtility.ToHtmlStringRGB(StyleSheetValues.CodeMethodColor)}";
            _basicColorString = $"#{ColorUtility.ToHtmlStringRGB(StyleSheetValues.CodeBasicColor)}";
            _commentColorString = $"#{ColorUtility.ToHtmlStringRGB(StyleSheetValues.CodeCommentColor)}";
            _stringColorString = $"#{ColorUtility.ToHtmlStringRGB(StyleSheetValues.CodeStringColor)}";

            var codeExample = (CodeExample)element;
            codeExample._stringAttr = _text.GetValueFromBag(attributes, context);

            element.AddToClassList(StyleSheetClasses.Background.Color.Secondary);
            element.AddToClassList(StyleSheetClasses.Border.Color.Black);

            GUIUtility.SetPadding(element, 5f);
            GUIUtility.SetBorderWidth(element, 1f);
            GUIUtility.SetBorderRadius(element, 3f);

            element.Clear();

            var label = new Label(codeExample._stringAttr);
            element.Add(label);

            label.style.whiteSpace = WhiteSpace.Normal;
            label.AddToClassList(StyleSheetClasses.Text.Color.Primary);

            var cleanText = CleanText(label.text);

            label.text = ApplySpacing(ColorText(label.text));

            var button = new Button(
                () => {EditorGUIUtility.systemCopyBuffer = cleanText;});

            element.Add(button);

            button.text = "";
            button.tooltip = "Copy to clipboard";

            button.style.backgroundImage = Resources.Load <Texture2D>(
                Path.Combine(Constants.BasePackage.Resources.Images.IconsPath, "Copy To Clipboard"));

            button.style.position = Position.Absolute;
            button.style.right = 5f;
            button.style.bottom = 5f;
            button.style.width = 20f;
            button.style.height = 20f;

            button.AddToClassList(StyleSheetClasses.Button);

            GUIUtility.SetMargin(button, 0f);
            GUIUtility.SetPadding(button, 0f);
        }

        #endregion

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
            GUIUtility.RemoveTag(ref text, "c");
            GUIUtility.RemoveTag(ref text, "m");
            GUIUtility.RemoveTag(ref text, "b");
            GUIUtility.RemoveTag(ref text, "cc");
            GUIUtility.RemoveTag(ref text, "s");

            text = text.Replace("\\n", Environment.NewLine);

            return ApplySpacing(text);
        }

        /// <summary> Color the text based on the tags </summary>
        /// <param name="text"> Text that should be colored </param>
        /// <returns> Colored text </returns>
        private string ColorText(string text)
        {
            GUIUtility.ColorByTag(ref text, "c", _classColorString);
            GUIUtility.ColorByTag(ref text, "m", _methodColorString);
            GUIUtility.ColorByTag(ref text, "b", _basicColorString);
            GUIUtility.ColorByTag(ref text, "cc", _commentColorString);
            GUIUtility.ColorByTag(ref text, "s", _stringColorString);

            return text;
        }

        #endregion
    }

    private string _stringAttr {get; set;}
}

}
#endif
