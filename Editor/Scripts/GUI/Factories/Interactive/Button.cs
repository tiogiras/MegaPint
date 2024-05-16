using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.Interactive
{

public class Button : UnityEngine.UIElements.Button
{
    public new class UxmlFactory : UxmlFactory <Button, UxmlTraits> { }

    public new class UxmlTraits : UnityEngine.UIElements.Button.UxmlTraits
    {
        private StyleColor _defaultBorderColor;
        private StyleColor _defaultColor;
        
        private bool _hovered;
        private bool _pressed;
        
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);

            element.AddToClassList(StyleSheetClasses.Button);
            
            // TODO Remove below
            
            /*
            element.AddToClassList(RootElement.FontClass);

            element.style.backgroundColor = RootElement.Colors.Button;
            element.style.color = RootElement.Colors.Text;
            
            GUIUtility.SetBorderColor(element, RootElement.Colors.Separator);

            GUIUtility.SubscribeInteractable(element);*/
        }

        #endregion
    }
}

}
