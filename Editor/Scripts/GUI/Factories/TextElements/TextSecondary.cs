#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.TextElements
{

/// <summary> Uxml factory to create a <see cref="VisualElement" /> with the settings of a secondary text </summary>
[UxmlElement]
internal partial class TextSecondary : Label
{
    public TextSecondary()
    {
        AddToClassList(StyleSheetClasses.Text.Color.Secondary);
    }
}

}
#endif
