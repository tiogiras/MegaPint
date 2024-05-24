#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.Backgrounds
{

/// <summary> Uxml factory to create a <see cref="VisualElement" /> with the settings of a quaternary background </summary>
internal class BackgroundQuaternary : VisualElement
{
    public new class UxmlFactory : UxmlFactory <BackgroundQuaternary, UxmlTraits>
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

            element.AddToClassList(StyleSheetClasses.Background.Color.Quaternary);
        }

        #endregion
    }
}

}
#endif
