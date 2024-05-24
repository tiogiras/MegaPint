using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Utility
{

public static partial class GUIUtility
{
    private static void SetParentFlexGrowRecursive(this VisualElement startElement, int iterations, bool value)
    {
        VisualElement element = startElement.parent;
        
        for (var i = 0; i < iterations; i++)
        {
            if (element == null)
                break;

            element.style.flexGrow = value ? 1 : 0;
            element = element.parent;
        }
    }
}

}
