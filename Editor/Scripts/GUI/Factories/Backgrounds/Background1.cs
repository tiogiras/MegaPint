using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.Backgrounds
{

public class Background1 : VisualElement
{
    public new class UxmlFactory : UxmlFactory <Background1, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);
            
            element.style.backgroundColor = RootElement.Colors.Back;
        }

        #endregion
    }
}

}
