#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.Backgrounds
{

/// <summary> Uxml factory to create a <see cref="VisualElement" /> with the settings of a quaternary background </summary>
[UxmlElement]
internal partial class BackgroundQuaternary : VisualElement
{
    public BackgroundQuaternary()
    {
        AddToClassList(StyleSheetClasses.Background.Color.Quaternary);
    }
}

}
#endif
