using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.Structure
{

public class Separator : VisualElement
{
    public new class UxmlFactory : UxmlFactory <Separator, UxmlTraits> { }

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
            
            element.style.height = _space.GetValueFromBag(attributes, context);
            
            element.AddToClassList(StyleSheetClasses.Background.Color.Separator);

            GUIUtility.SetMargin(element, 4f);
            GUIUtility.SetBorderRadius(element, 1f);
        }

        #endregion
    }
}

}
