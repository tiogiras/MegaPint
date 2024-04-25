using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.Headings
{

public class MpHeading2 : Label
{
    public new class UxmlFactory : UxmlFactory <MpHeading2, UxmlTraits> { }

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
