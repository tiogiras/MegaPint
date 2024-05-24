#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.Headings
{

/// <summary> Uxml factory to create a <see cref="VisualElement" /> with the settings of a heading using the identity color </summary>
public class HeadingIdentity : Label
{
    public new class UxmlFactory : UxmlFactory <HeadingIdentity, UxmlTraits>
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

            element.AddToClassList(StyleSheetClasses.Text.Color.Identity);
        }

        #endregion
    }
}

}
#endif
