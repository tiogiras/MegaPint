#if UNITY_EDITOR
using MegaPint.Editor.Scripts.GUI.Utility;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.Structure
{

/// <summary> Uxml factory to create a <see cref="VisualElement" /> that acts as a separator between other elements </summary>
[UxmlElement]
internal partial class SeparatorVertical : VisualElement
{
    [UxmlAttribute("Space")]
    public float Space { get; set; } = 1.5f;

    public SeparatorVertical()
    {
        RegisterCallback<AttachToPanelEvent>(_ => BuildUI());
    }
    
    private void BuildUI()
    {
        style.width = Space;

        AddToClassList(StyleSheetClasses.Background.Color.Separator);

        this.SetMargin(4f);
        this.SetBorderRadius(1f);
    }
}

}
#endif
