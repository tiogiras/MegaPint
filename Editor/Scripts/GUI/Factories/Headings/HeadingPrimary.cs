#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.Headings
{

/// <summary>
///     Uxml factory to create a <see cref="VisualElement" /> with the settings of a heading using the primary text
///     color
/// </summary>
[UxmlElement]
internal partial class HeadingPrimary : Label
{
    public HeadingPrimary()
    {
        AddToClassList(StyleSheetClasses.Text.Color.Primary);   
    }
}

}
#endif
