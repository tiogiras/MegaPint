#if UNITY_EDITOR
using System.Collections.Generic;

namespace MegaPint.Editor.Scripts.GUI
{

/// <summary> Lookup table for information tooltips </summary>
public static class InfoLinkData
{
    private static readonly Dictionary <string, string> s_data = new()
    {
        {
            "notePad",
            "NotePad is a class that can be added to a MonoBehaviour to hold notes and display them in the inspector.\n\n<b>Assembly:</b>tiogiras.megapint.notepad.runtime"
        },
        {"gameObject", "GameObjects are the fundamentals of every object in your game."},
        {
            "monoBehaviour",
            "MonoBehaviours are scripts that inherit from the base class MonoBehaviour.\n\nOnly MonoBehaviours can be added to GameObjects."
        },
        {
            "alphaButton",
            "AlphaButton is a class derived from the standard Button class that includes a slider for the alpha threshold."
        },
        {"button", "The Button component is a default ui element of the unity engine."},
        {
            "shortcut",
            "Shortcuts can be applied to menu items. Whenever the shortcut is pressed, the corresponding menu item is executed.\n\n<b>You can set shortcuts via Edit/Shortcuts...</b>"
        },
        {
            "cameraCapture",
            "CameraCapture is a class that can render and export a camera's view via the inspector or the designated shortcut.\n\n<b>Assembly:</b>tiogiras.megapint.screenshot.runtime"
        },
        {"camera", "Camera's are default unity components for rendering an image of the game."},
        {
            "scriptableValidationRequirement",
            "ScriptableValidationRequirement is a base class for requirements that can be used in the validation process of a ValidatableMonoBehaviour.\n\n<b>Assembly:</b>tiogiras.megapint.validators.runtime"
        },
        {
            "validatableMonoBehaviour",
            "ValidatableMonoBehaviour is a base class derived from the MonoBehaviour class. It holds certain requirements that if not met, display issues on the GameObject.\n\n<b>Assembly:</b>tiogiras.megapint.validators.runtime"
        },
        {
            "requirement",
            "Requirements are validations that will be run on MonoBehaviours.\n\n <b>Created via ScriptableValidationRequirement</b>"
        },
        {
            "validationStatus",
            "ValidatableMonoBehaviourStatus is a MonoBehaviour added by a ValidatableMonoBehaviour. It collects all behaviours of this GameObject and displays their status and possible fixes."
        },
        {
            "fixAction",
            "FixActions are methods added to the occured issue. They should contain an automatic fix for the issue."
        },
        {"test", "This is a test tooltip for development purposes."}

        // {"",""},
    };

    #region Public Methods

    /// <summary> Get the information tooltip text for a given key </summary>
    /// <param name="infoKey"> Key corresponding to the wanted information tooltip </param>
    /// <returns> Text for the information tooltip </returns>
    public static string Get(string infoKey)
    {
        return !s_data.ContainsKey(infoKey) ? $"No info found for [{infoKey}]" : s_data[infoKey];
    }

    #endregion
}

}
#endif
