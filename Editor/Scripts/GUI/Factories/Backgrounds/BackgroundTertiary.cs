#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.Backgrounds
{

/// <summary> Uxml factory to create a <see cref="VisualElement" /> with the settings of a tertiary background </summary>
public class BackgroundTertiary : VisualElement
{
    public new class UxmlFactory : UxmlFactory <BackgroundTertiary, UxmlTraits>
    {
    }

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
#endif
