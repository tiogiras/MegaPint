using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.Backgrounds
{

public class BackgroundTertiary : VisualElement
{
    public new class UxmlFactory : UxmlFactory <BackgroundTertiary, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);
            
            element.AddToClassList(StyleSheetClasses.Background.Color.Tertiary);
        }

        #endregion
    }
}

}
