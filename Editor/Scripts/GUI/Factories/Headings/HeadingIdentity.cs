using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.Headings
{

public class HeadingIdentity : Label
{
    public new class UxmlFactory : UxmlFactory <HeadingIdentity, UxmlTraits> { }

    public new class UxmlTraits : Label.UxmlTraits
    {
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);
            
            element.AddToClassList(StyleSheetClasses.Text.Color.Identity);
        }

        #endregion
    }
}

}
