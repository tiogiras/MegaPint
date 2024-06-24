#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Windows.BaseWindowContent.SettingsTabContent
{

/// <summary> Contains logic for displaying the settings tabs </summary>
internal static class SettingsTabDisplay
{
    private static string s_basePath;

    private static int s_editorThemeIndex = -1;

    private static string _BasePath =>
        Path.Combine(Constants.BasePackage.UserInterface.SettingsContent, "xxx");

    #region Public Methods

    /// <summary> Display the setting based on the given key </summary>
    /// <param name="root"> Root <see cref="VisualElement" /> the setting is added to </param>
    /// <param name="key"> Key corresponding to the target setting </param>
    public static void Display(VisualElement root, SettingsTabData.SettingsKey key)
    {
        GUIUtility.Instantiate(Load(key), root);
        ActivateLogic(key, root);
    }

    #endregion

    #region Private Methods

    /// <summary> Invoked when the setting is added to it's parent </summary>
    /// <param name="key"> Key of the added setting </param>
    /// <param name="root"> Setting as <see cref="VisualElement" /> </param>
    private static void ActivateLogic(SettingsTabData.SettingsKey key, VisualElement root)
    {
        switch (key)
        {
            case SettingsTabData.SettingsKey.Theme:
                ThemeLogic(root);

                break;

            case SettingsTabData.SettingsKey.Feedback:
                FeedbackLogic(root);
                
                break;
                
            case SettingsTabData.SettingsKey.Testing:
                TestingLogic(root);
                
                break;
            
            default:
                return;
        }
    }

    /// <summary> Load the uxml file of the selected setting </summary>
    /// <param name="key"> Key corresponding to the targeted setting </param>
    /// <returns> Loaded uxml file </returns>
    private static VisualTreeAsset Load(SettingsTabData.SettingsKey key)
    {
        return Resources.Load <VisualTreeAsset>(_BasePath.Replace("xxx", key.ToString()));
    }

    /// <summary> Logic of the theme setting </summary>
    /// <param name="root"> Setting as <see cref="VisualElement" /> </param>
    private static void ThemeLogic(VisualElement root)
    {
        var dropdown = root.Q <DropdownField>("EditorTheme");

        if (s_editorThemeIndex < 0)
            s_editorThemeIndex = SaveValues.BasePackage.EditorTheme;

        dropdown.SetValueWithoutNotify(dropdown.choices[s_editorThemeIndex]);

        dropdown.RegisterValueChangedCallback(
            _ =>
            {
                s_editorThemeIndex = dropdown.index;
                SaveValues.BasePackage.EditorTheme = dropdown.index;
            });
    }
    
    /// <summary> Logic of the feedback setting </summary>
    /// <param name="root"> Setting as <see cref="VisualElement" /> </param>
    private static void FeedbackLogic(VisualElement root)
    {
        var sendFeedbackField = root.Q <Toggle>("SendFeedback");
        
        sendFeedbackField.SetValueWithoutNotify(SaveValues.BasePackage.SendFeedback);

        sendFeedbackField.RegisterValueChangedCallback(evt =>
        {
            SaveValues.BasePackage.SendFeedback = evt.newValue;
        });
    }

    /// <summary> Logic of the testing setting </summary>
    /// <param name="root"> Setting as <see cref="VisualElement" /> </param>
    private static void TestingLogic(VisualElement root)
    {
        var tokenField = root.Q <TextField>("Token");
        var btnSave = root.Q <Button>("BTN_Save");
        var invalid = root.Q <VisualElement>("Invalid");
        var valid = root.Q <VisualElement>("Valid");

        btnSave.style.display = DisplayStyle.None;
        
        btnSave.clickable = new Clickable(() =>
        {
            SaveValues.BasePackage.TesterToken = tokenField.value;
            btnSave.style.display = DisplayStyle.None;
            
            UpdateTestingDisplay(tokenField, invalid, valid);
        });

        tokenField.RegisterValueChangedCallback(evt =>
        {
            btnSave.style.display = DisplayStyle.Flex;
        });
        
        UpdateTestingDisplay(tokenField, invalid, valid);
    }

    /// <summary> Update the visuals of the testing setting </summary>
    /// <param name="tokenField"> TextField of the token </param>
    /// <param name="invalid"> Invalid visualElement </param>
    /// <param name="valid"> Valid visualElement </param>
    private static void UpdateTestingDisplay(TextField tokenField, VisualElement invalid, VisualElement valid)
    {
        tokenField.SetValueWithoutNotify(SaveValues.BasePackage.TesterToken);
 
        var validToken = Utility.ValidateTesterToken();

        invalid.style.display = validToken ? DisplayStyle.None : DisplayStyle.Flex;
        valid.style.display = validToken ? DisplayStyle.Flex : DisplayStyle.None;
    }

    #endregion
}

}
#endif
