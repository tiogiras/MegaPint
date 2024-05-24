#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.TextElements
{

/// <summary> Uxml factory to create a <see cref="VisualElement" /> with the settings of a primary text </summary>
public class Text : Label
{
    public new class UxmlFactory : UxmlFactory <Text, UxmlTraits>
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

            element.AddToClassList(StyleSheetClasses.Text.Color.Primary);
        }

        #endregion
    }
}

}
#endif
