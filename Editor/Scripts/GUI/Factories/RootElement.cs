#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.GUIUtility;

namespace MegaPint.Editor.Scripts.GUI.Factories
{

/// <summary>
///     Uxml factory to create a <see cref="VisualElement" />> that manages the theme of the child objects by
///     changing the added theme class
/// </summary>
public class RootElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory <RootElement, UxmlTraits>
    {
    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private struct InitializationArgs
        {
            public VisualElement element;
            public VisualElement linkColorElement;
            public VisualElement infoColorElement;
            public VisualElement codeBasicColorElement;
            public VisualElement codeClassColorElement;
            public VisualElement codeMethodColorElement;
            public VisualElement codeCommentColorElement;
            public VisualElement codeStringColorElement;
        }

        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);

            element.name = "RootElement";

            GUIUtility.ApplyRootElementTheme(element, TryInitializeStyleSheetValues);

            element.schedule.Execute(
                () => {ExecuteForAllOfClass("unity-color-field", element, ColorFieldOverwrite);});

            TryInitializeStyleSheetValues(element);
        }

        #endregion

        #region Private Methods

        /// <summary> Action to overwrite the built in onGUIHandler of the color field </summary>
        /// <param name="currentCheck"> The index of the current check </param>
        /// <param name="hasEyeUpdate"> If the eyeDropper is active </param>
        /// <param name="wouldRemove"> If the focus would be removed </param>
        /// <param name="fieldNameIdentifier"> Identifier for the responding event </param>
        /// <param name="action"> A reference to the built in action </param>

        // ReSharper disable once CognitiveComplexity
        private static void ColorFieldOnGUIHandler(
            ref int currentCheck,
            ref bool hasEyeUpdate,
            ref bool wouldRemove,
            string fieldNameIdentifier,
            Action action)
        {
            if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "EyeDropperUpdate")
                hasEyeUpdate = true;

            if (currentCheck == 3)
            {
                if (UnityEngine.GUI.GetNameOfFocusedControl() == fieldNameIdentifier && !hasEyeUpdate)
                {
                    if (wouldRemove)
                    {
                        UnityEngine.GUI.FocusControl(null);

                        wouldRemove = false;
                    }
                    else
                        wouldRemove = true;
                }

                hasEyeUpdate = false;
                currentCheck = 0;
            }
            else
                currentCheck++;

            UnityEngine.GUI.SetNextControlName(fieldNameIdentifier);
            action?.Invoke();
        }

        /// <summary> Action called on all built in color fields </summary>
        /// <param name="element"> ColorField to be overwritten </param>
        private static void ColorFieldOverwrite(VisualElement element)
        {
            var fieldNameIdentifier = $"MegaPintColorField{element.GetHashCode()}";
            var target = (IMGUIContainer)element.Q(className: "unity-base-field__input");

            var currentCheck = 0;
            var hasEyeUpdate = false;
            var wouldRemove = false;

            Action action = target.onGUIHandler;

            target.onGUIHandler = () =>
            {
                ColorFieldOnGUIHandler(
                    ref currentCheck,
                    ref hasEyeUpdate,
                    ref wouldRemove,
                    fieldNameIdentifier,
                    action);
            };
        }

        /// <summary> Create a <see cref="VisualElement" /> to store the wanted color </summary>
        /// <param name="root"> The parent the created element is added to </param>
        /// <param name="colorClass"> The uss class overwriting the wanted color </param>
        /// <returns> Created <see cref="VisualElement" /> </returns>
        private static VisualElement CreateColorElement(VisualElement root, string colorClass)
        {
            var element = new VisualElement();
            root.Add(element);

            element.AddToClassList(colorClass);

            return element;
        }

        /// <summary> Execute an action for all child elements of the selected class </summary>
        /// <param name="className"> Name of the selected uss class </param>
        /// <param name="element"> Root element to start the search in </param>
        /// <param name="action"> Action to be invoked on all found elements </param>
        private static void ExecuteForAllOfClass(string className, VisualElement element, Action <VisualElement> action)
        {
            List <VisualElement> elements = element.Query(className: className).ToList();

            if (elements.Count == 0)
                return;

            foreach (VisualElement visualElement in elements)
                action?.Invoke(visualElement);
        }

        /// <summary> Get the color variable of the resolvedStyle of the <see cref="VisualElement" />> </summary>
        /// <param name="element"> Targeted element </param>
        /// <param name="redo"> True when the color was not initialized yet on the <see cref="VisualElement" /> </param>
        /// <returns> Color variable of the targeted element </returns>
        private static Color GetColorFromElement(VisualElement element, ref bool redo)
        {
            Color color = element.resolvedStyle.color;

            if (color == Color.black)
                redo = true;

            return color;
        }

        /// <summary> Initialize the <see cref="StyleSheetValues" /> colors with the colors of the current theme </summary>
        /// <param name="args"> Arguments for the initialization </param>
        private static void InitializeStyleSheetValues(InitializationArgs args)
        {
            var redo = false;

            Color linkColor = GetColorFromElement(args.linkColorElement, ref redo);
            Color infoColor = Color.black;

            Color codeBasicColor = Color.black;

            Color codeClassColor = Color.black;

            Color codeMethodColor = Color.black;

            Color codeCommentColor = Color.black;

            Color codeStringColor = Color.black;

            if (!redo)
                infoColor = GetColorFromElement(args.infoColorElement, ref redo);

            if (!redo)
                codeBasicColor = GetColorFromElement(args.codeBasicColorElement, ref redo);

            if (!redo)
                codeClassColor = GetColorFromElement(args.codeClassColorElement, ref redo);

            if (!redo)
                codeMethodColor = GetColorFromElement(args.codeMethodColorElement, ref redo);

            if (!redo)
                codeCommentColor = GetColorFromElement(args.codeCommentColorElement, ref redo);

            if (!redo)
                codeStringColor = GetColorFromElement(args.codeStringColorElement, ref redo);

            if (redo)
            {
                ScheduleStyleSheetInitialization(args);

                return;
            }

            StyleSheetValues.Initialize(
                linkColor,
                infoColor,
                codeBasicColor,
                codeClassColor,
                codeMethodColor,
                codeCommentColor,
                codeStringColor);

            args.linkColorElement.RemoveFromHierarchy();
            args.infoColorElement.RemoveFromHierarchy();

            args.codeBasicColorElement.RemoveFromHierarchy();
            args.codeClassColorElement.RemoveFromHierarchy();
            args.codeMethodColorElement.RemoveFromHierarchy();
            args.codeCommentColorElement.RemoveFromHierarchy();
            args.codeStringColorElement.RemoveFromHierarchy();
        }

        /// <summary> Schedule to initialize the <see cref="StyleSheetValues" /> on the given root <see cref="VisualElement" />> </summary>
        /// <param name="args"> Arguments for the initialization </param>
        private static void ScheduleStyleSheetInitialization(InitializationArgs args)
        {
            args.element.schedule.Execute(() => InitializeStyleSheetValues(args));
        }

        /// <summary> Try to initialize the <see cref="StyleSheetValues" />> </summary>
        /// <param name="element"> Root <see cref="VisualElement" />> </param>
        private static void TryInitializeStyleSheetValues(VisualElement element)
        {
            if (StyleSheetValues.Initialized)
                return;

            VisualElement linkColorElement = CreateColorElement(element, StyleSheetClasses.Text.Color.Link);
            VisualElement infoColorElement = CreateColorElement(element, StyleSheetClasses.Text.Color.Info);

            VisualElement codeBasicColorElement = CreateColorElement(element, StyleSheetClasses.Code.Color.Basic);
            VisualElement codeClassColorElement = CreateColorElement(element, StyleSheetClasses.Code.Color.Class);
            VisualElement codeMethodColorElement = CreateColorElement(element, StyleSheetClasses.Code.Color.Method);
            VisualElement codeCommentColorElement = CreateColorElement(element, StyleSheetClasses.Code.Color.Comment);
            VisualElement codeStringColorElement = CreateColorElement(element, StyleSheetClasses.Code.Color.String);

            ScheduleStyleSheetInitialization(
                new InitializationArgs
                {
                    element = element,
                    linkColorElement = linkColorElement,
                    infoColorElement = infoColorElement,
                    codeBasicColorElement = codeBasicColorElement,
                    codeClassColorElement = codeClassColorElement,
                    codeMethodColorElement = codeMethodColorElement,
                    codeCommentColorElement = codeCommentColorElement,
                    codeStringColorElement = codeStringColorElement
                });
        }

        #endregion
    }
}

}
#endif
