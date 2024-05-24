#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.TextElements
{

/// <summary> Uxml factory to create a <see cref="VisualElement" /> with the settings of a secondary text </summary>
internal class TextSecondary : Label
{
    public new class UxmlFactory : UxmlFactory <TextSecondary, UxmlTraits>
    {
    }

    public new class UxmlTraits : Label.UxmlTraits
    {
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);

            element.AddToClassList(StyleSheetClasses.Text.Color.Secondary);
        }

        #endregion
    }
}

}
#endif
