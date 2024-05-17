using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.Backgrounds
{

public class BackgroundSecondary : VisualElement
{
    public new class UxmlFactory : UxmlFactory <BackgroundSecondary, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);
            
            element.AddToClassList(StyleSheetClasses.Background.Color.Secondary);
        }

        #endregion
    }
}

}
