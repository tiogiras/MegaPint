#if UNITY_EDITOR
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories
{

/// <summary>
///     Uxml factory to create a <see cref="VisualElement" />> that manages the theme of the child objects by
///     changing the added theme class
/// </summary>
[UxmlElement]
internal partial class Spinner : VisualElement
{
    [UxmlAttribute]
    private int _refreshRate = 1;
    
    private VisualElement[] _spinner;
    
    public Spinner()
    {
        name = "Spinner";

        TemplateContainer spinner = Resources.Load <VisualTreeAsset>(Constants.BasePackage.UserInterface.Spinner).Instantiate();
        spinner.style.flexGrow = 1;
        
        Add(spinner);
        
        _spinner = GetSpinner(spinner.Q("Loading"));

        RegisterCallback<AttachToPanelEvent>(evt => _ = UpdateSpinner());
    }

    private async Task UpdateSpinner()
    {
        var loadingTime = 0;
        
        while (panel != null)
        {
            loadingTime += _refreshRate;
            HandleLoadingIcon(_spinner, loadingTime);

            await Task.Delay(_refreshRate);
        }
    }
    
    /// <summary> Handle the current status of the spinner </summary>
    /// <param name="elements"> All parts of the spinner </param>
    /// <param name="loadingTime"> Current loading time </param>
    private static void HandleLoadingIcon(IReadOnlyList <VisualElement> elements, int loadingTime)
    {
        const float CycleTime = 20f;

        for (var i = 0; i < 12; i++)
        {
            var individualTime = loadingTime + i * CycleTime / 6;

            VisualElement element = elements[i];

            var fadeIn = Mathf.CeilToInt(individualTime / CycleTime) % 2 == 0;

            var rest = individualTime % CycleTime;

            if (rest == 0)
                continue;

            var opacity = Mathf.Lerp(fadeIn ? 0 : 1, fadeIn ? 1 : 0, rest / CycleTime);

            element.style.opacity = opacity;
        }
    }
    
    /// <summary> Collect all parts of the spinner </summary>
    /// <param name="root"> Parent <see cref="VisualElement" /> containing the spinner elements </param>
    /// <returns> Collected spinner parts </returns>
    private static VisualElement[] GetSpinner(VisualElement root)
    {
        VisualElement[] elements = new VisualElement[12];

        for (var i = 1; i <= 12; i++)
        {
            VisualElement element = root.Q($"Loading{i}");
            elements[i - 1] = element;
            element.style.opacity = 0;
        }

        return elements;
    }
}
}
#endif
