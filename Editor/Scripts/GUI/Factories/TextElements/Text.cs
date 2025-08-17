#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.TextElements
{

/// <summary> Uxml factory to create a <see cref="VisualElement" /> with the settings of a primary text </summary>
[UxmlElement]
internal partial class Text : Label
{
    public Text()
    {
        AddToClassList(StyleSheetClasses.Text.Color.Primary);
    }
}

}
#endif
