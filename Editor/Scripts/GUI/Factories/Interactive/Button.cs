using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories.Interactive
{

public class Button : UnityEngine.UIElements.Button
{
    public new class UxmlFactory : UxmlFactory <Button, UxmlTraits> { }

    public new class UxmlTraits : UnityEngine.UIElements.Button.UxmlTraits
    {
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);

            element.AddToClassList(StyleSheetClasses.Button);

            element.style.color = new StyleColor(Color.red);
            element.style.backgroundColor = new StyleColor(Color.red);

            // TODO Remove below

            /*
            element.AddToClassList(RootElement.FontClass);

            element.style.backgroundColor = RootElement.Colors.Button;
            element.style.color = RootElement.Colors.Text;
            
            GUIUtility.SetBorderColor(element, RootElement.Colors.Separator);

            GUIUtility.SubscribeInteractable(element);*/
        }

        #endregion
    }
}

}
