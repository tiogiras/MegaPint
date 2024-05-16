using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.Interactive
{

public class SearchField : ToolbarSearchField
{
    public new class UxmlFactory : UxmlFactory <SearchField, UxmlTraits> { }

    public new class UxmlTraits : ToolbarSearchField.UxmlTraits
    {
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);
            
            element.AddToClassList("mp_interaction");
            element.AddToClassList("mp_interaction_onlyLoseFocusOnBlur");
            element.AddToClassList("mp_interaction_checkColorOnMouseUp");
            element.AddToClassList("mp_borderColor_bg1");
            element.AddToClassList("mp_bg3");

            VisualElement textInput = element.Q(className: "unity-base-text-field__input");
            VisualElement textElement = textInput.Q(className: "unity-text-element");
            
            textInput.parent.AddToClassList(RootElement.CaretColorClass);
            
            textInput.AddToClassList("mp_bg3");
            textElement.AddToClassList("mp_bg3");
            
            textElement.AddToClassList(RootElement.FontClass);
            textElement.style.color = RootElement.Colors.Text;
        }

        #endregion
    }
}

}
