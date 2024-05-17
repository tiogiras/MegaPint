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

            element.AddToClassList("mp");
            
            element.AddToClassList("mp_theme--dark"); // TODO add light theme if in light mode
        }

        #endregion
    }
}

}
