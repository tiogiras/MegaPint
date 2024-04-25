using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories
{

public class Text : Label
{
    public new class UxmlFactory : UxmlFactory <Text, UxmlTraits> { }

    public new class UxmlTraits : Label.UxmlTraits
    {
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);
            
            element.AddToClassList(RootElement.FontClass);
            element.style.color = RootElement.Colors.Text;
        }

        #endregion
    }
}

}
