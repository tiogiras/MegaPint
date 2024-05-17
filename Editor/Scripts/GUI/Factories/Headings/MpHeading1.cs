using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.Headings
{

public class MpHeading1 : Label
{
    public new class UxmlFactory : UxmlFactory <MpHeading1, UxmlTraits> { }

    public new class UxmlTraits : Label.UxmlTraits
    {
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);

            element.style.color = Color.red;
        }

        #endregion
    }
}

}
