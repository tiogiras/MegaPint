#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.Backgrounds
{

/// <summary> Uxml factory to create a <see cref="VisualElement" /> with the settings of a primary background </summary>
[UxmlElement]
internal partial class BackgroundPrimary : VisualElement
{
    public BackgroundPrimary()
    {
        AddToClassList(StyleSheetClasses.Background.Color.Primary);
    }
}

}
#endif
