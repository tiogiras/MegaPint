using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Utility
{

public static partial class GUIUtility
{
    public static VisualElement Instantiate(VisualTreeAsset asset, VisualElement root = null)
    {
        TemplateContainer element = asset.Instantiate();

        root?.Add(element);

        return element;
    }
}

}
