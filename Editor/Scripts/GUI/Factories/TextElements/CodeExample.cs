using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.TextElements
{

public class CodeExample : VisualElement
{
    public new class UxmlFactory : UxmlFactory <CodeExample, UxmlTraits> { }

    private string _stringAttr { get; set; }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        #region Public Methods

        // <c></c> = purple
        // <m></m> = green
        // <b></b> = blue

        private readonly UxmlStringAttributeDescription _text = new() {name = "_string-attr"};
        
        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);
            
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
            
            var cleanText = CleanText(label.text);
            
            label.text = ApplySpacing(ColorText(label.text));

            var button = new Button(
                () =>
                {
                    EditorGUIUtility.systemCopyBuffer = cleanText;
                });

            element.Add(button);

            button.text = "";
            button.tooltip = "Copy to clipboard";
            button.style.backgroundImage = Resources.Load <Texture2D>("MegaPint/Images/Icons/Copy To Clipboard");
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
                {
                    space.Append(forClipboard ? "/t" : " ");
                }

                output.Append(space);

                output.Append(args[1]);
            }

            return output.ToString();
        }

        private static string CleanText(string text)
        {
            RemoveTag(ref text, "c");
            RemoveTag(ref text, "m");
            RemoveTag(ref text, "b");
            RemoveTag(ref text, "cc");
            RemoveTag(ref text, "s");

            text = text.Replace("\\n", Environment.NewLine);

            return ApplySpacing(text);
        }

        private static void RemoveTag(ref string text, string tag)
        {
            var startTag = $"<{tag}>";
            var endTag = $"</{tag}>";

            text = text.Replace(startTag, "");
            text = text.Replace(endTag, "");
        }
        
        private static string ColorText(string text)
        {
            ColorByTag(ref text, "c", "#BC7CAA");
            ColorByTag(ref text, "m", "#38C88C");
            ColorByTag(ref text, "b", "#4D7EE3");
            ColorByTag(ref text, "cc", "#7ABA59");
            ColorByTag(ref text, "s", "#C88847");

            return text;
        }

        private static void ColorByTag(ref string text, string tag, string color)
        {
            var startTag = $"<{tag}>";
            var endTag = $"</{tag}>";
            
            text = text.Replace(startTag, $"<color={color}>");
            text = text.Replace(endTag, "</color>");
        }
    }
}

}
