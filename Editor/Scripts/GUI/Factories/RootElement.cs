#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using MegaPint.Editor.Scripts.GUI.Utility;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.GUI.Factories
{

/// <summary>
///     Uxml factory to create a <see cref="VisualElement" />> that manages the theme of the child objects by
///     changing the added theme class
/// </summary>
[UxmlElement]
internal partial class RootElement : VisualElement
{
    public RootElement()
    {
        name = "RootElement";

        this.ApplyRootElementTheme(TryInitializeStyleSheetValues);

        schedule.Execute(() => {ExecuteForAllOfClass(StyleSheetClasses.Tooltip, this, TooltipOverwrite);});

        TryInitializeStyleSheetValues(this);
    }

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

    #region Private Methods

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

    /// <summary> Action called on all elements with the custom tooltip class </summary>
    /// <param name="element"> Targeted element </param>
    private static void TooltipOverwrite(VisualElement element)
    {
        var tooltip = element.tooltip;

        element.RegisterCallback <PointerEnterEvent>(_ =>
        {
            if (string.IsNullOrEmpty(element.tooltip))
                return;

            tooltip = element.tooltip;
            element.tooltip = "";

            GUIUtility.DisplayTooltip(element, tooltip);
        });

        element.RegisterCallback <PointerLeaveEvent>(_ =>
        {
            element.tooltip = tooltip;
            GUIUtility.HideTooltip(element);
        });
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
#endif
