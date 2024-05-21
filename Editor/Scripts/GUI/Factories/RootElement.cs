using System;
using System.Collections.Generic;
using Editor.Scripts.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories
{

public class RootElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory <RootElement, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);
            
            GUIUtility.ApplyRootElementTheme(element);

            element.schedule.Execute(
                () =>
                {
                    ExecuteForAllOfClass("unity-color-field", element, ColorFieldOverwrite);
                });
        }

        #endregion

        private static void ExecuteForAllOfClass(string className, VisualElement element, Action<VisualElement> action)
        {
            List <VisualElement> elements = element.Query(className: className).ToList();

            if (elements.Count == 0)
                return;

            foreach (VisualElement visualElement in elements)
            {
                action?.Invoke(visualElement);
            }
        }

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
                ColorFieldOnGUIHandler(ref currentCheck, ref hasEyeUpdate, ref wouldRemove, fieldNameIdentifier, action);
            }; 
        }

        private static void ColorFieldOnGUIHandler(ref int currentCheck, ref bool hasEyeUpdate, ref bool wouldRemove, string fieldNameIdentifier, Action action)
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
            {
                currentCheck++;
            }

            UnityEngine.GUI.SetNextControlName(fieldNameIdentifier);
            action?.Invoke();
        }
    }
}

}
