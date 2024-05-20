using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.Interactive
{

public class Dropdown : DropdownField
{
    public new class UxmlFactory : UxmlFactory <Dropdown, UxmlTraits> { }

    public new class UxmlTraits : DropdownField.UxmlTraits
    {
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);

            VisualElement label = element.Q(className: "unity-base-field__label");
            
            element.style.color = new StyleColor(Color.red);
            element.style.backgroundColor = new StyleColor(Color.red);
            
            if (label == null)
                return;
            
            label.AddToClassList(StyleSheetClasses.Text.Color.Primary);

            VisualElement input = element.Q(className: "unity-base-field__input");
            input.AddToClassList(StyleSheetClasses.Dropdown);
            
            VisualElement textElement = input.Q(className: "unity-text-element");
            textElement.AddToClassList(StyleSheetClasses.Text.Color.Primary);
        }

        #endregion
    }
}

}
