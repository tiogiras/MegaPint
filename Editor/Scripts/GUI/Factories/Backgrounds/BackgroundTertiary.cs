#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.Backgrounds
{

/// <summary> Uxml factory to create a <see cref="VisualElement" /> with the settings of a tertiary background </summary>
[UxmlElement]
internal partial class BackgroundTertiary : VisualElement
{
    public BackgroundTertiary()
    {
        AddToClassList(StyleSheetClasses.Background.Color.Tertiary);
    }
}

}
#endif
