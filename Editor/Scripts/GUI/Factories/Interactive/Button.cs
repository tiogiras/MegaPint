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

            element.AddToClassList(RootElement.FontClass);

            GUIUtility.SubscribeInteractable(element);
            
            /*element.RegisterCallback <MouseOverEvent>(BeginHover);
            element.RegisterCallback <MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
            element.RegisterCallback <MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
            element.RegisterCallback <MouseOutEvent>(EndHover);*/
        }

        #endregion
        
        #region Private Methods

        private void BeginHover(MouseOverEvent evt)
        {
            _hovered = true;

            var element = (VisualElement)evt.target;
            _defaultColor = element.style.backgroundColor;
            _defaultBorderColor = element.style.borderTopColor;

            Refresh(element);
        }

        private void EndHover(MouseOutEvent evt)
        {
            _hovered = false;
            _pressed = false;

            var element = (VisualElement)evt.target;
            element.Blur();

            Refresh(element);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            _pressed = true;
            Refresh(evt.target as VisualElement);
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            _pressed = false;
            Refresh(evt.target as VisualElement);
        }

        private void Refresh(VisualElement element)
        {
            GUIUtility.SetBorderColor(
                element,
                _hovered ? RootElement.Colors.Primary : _defaultBorderColor);

            element.style.backgroundColor =
                _pressed ? RootElement.Colors.PrimaryInteracted : _defaultColor;
        }

        #endregion
    }
}

}
