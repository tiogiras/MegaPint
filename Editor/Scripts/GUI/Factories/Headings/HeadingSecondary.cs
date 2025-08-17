#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.Headings
{

/// <summary>
///     Uxml factory to create a <see cref="VisualElement" /> with the settings of a heading using the secondary text
///     color
/// </summary>
[UxmlElement]
internal partial class HeadingSecondary : Label
{
    public HeadingSecondary()
    {
        AddToClassList(StyleSheetClasses.Text.Color.Secondary);
    }
}

}
#endif
