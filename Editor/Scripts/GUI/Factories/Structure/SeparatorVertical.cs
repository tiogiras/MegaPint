using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.Structure
{

public class SeparatorVertical : VisualElement
{
    public new class UxmlFactory : UxmlFactory <SeparatorVertical, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private readonly UxmlFloatAttributeDescription _space = new()
        {
            name = "Space",
            defaultValue = 1.5f
        };
        
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);
            
            element.style.width = _space.GetValueFromBag(attributes, context);
            
            element.AddToClassList(StyleSheetClasses.Background.Color.Separator);

            GUIUtility.SetMargin(element, 4f);
            GUIUtility.SetBorderRadius(element, 1f);
        }

        #endregion
    }
}

}
