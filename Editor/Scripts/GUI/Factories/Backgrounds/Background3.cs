using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.Backgrounds
{

public class Background3 : VisualElement
{
    public new class UxmlFactory : UxmlFactory <Background3, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);
            
            element.style.backgroundColor = RootElement.Colors.TertiaryBack;
        }

        #endregion
    }
}

}
