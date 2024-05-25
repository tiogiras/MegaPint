#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.Windows
{

/// <summary> Editor window to display the first steps in megapint </summary>
internal class FirstSteps : EditorWindowBase
{
    #region Private

    /// <summary> Loaded uxml references </summary>
    private VisualTreeAsset _baseWindow;

    #endregion

    #region Visual References

    private Button _createAsset;

    #endregion

    #region Public Methods

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "First Steps";

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return Path.Combine(Constants.BasePackage.Resources.UserInterface.WindowsPath, "First Steps");
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = rootVisualElement;

        VisualElement content = _baseWindow.Instantiate();

        content.style.flexGrow = 1f;
        content.style.flexShrink = 1f;

        #region References

        _createAsset = content.Q <Button>("BTN_Create");

        #endregion

        RegisterCallbacks();

        root.Add(content);
    }

    protected override bool LoadResources()
    {
        _baseWindow = Resources.Load <VisualTreeAsset>(BasePath());

        return _baseWindow != null;
    }

    protected override void RegisterCallbacks()
    {
        _createAsset.clicked += OnCreateSettingsAsset;
    }

    protected override void UnRegisterCallbacks()
    {
        _createAsset.clicked -= OnCreateSettingsAsset;
    }

    #endregion

    #region Private Methods

    /// <summary> Create a settings asset for megapint in the project </summary>
    private void OnCreateSettingsAsset()
    {
        var path = EditorUtility.SaveFilePanel(
            "MegaPint Settings",
            "Assets",
            "MegaPintSettings",
            "asset");

        if (!path.StartsWith(Application.dataPath))
        {
            EditorUtility.DisplayDialog(
                "MegaPint settings asset",
                $"Could not create asset at {path}.\nMake sure it is located in the assets folder.",
                "OK");

            return;
        }

        path = path.Replace(Application.dataPath, "Assets");

        Settings.MegaPintSettings.instance = CreateInstance <Settings.MegaPintSettings>();
        AssetDatabase.CreateAsset(Settings.MegaPintSettings.instance, path);

        EditorUtility.DisplayDialog(
            "MegaPint settings asset",
            Settings.MegaPintSettings.instance != null
                ? $"Successfully created a new MegaPint settings asset at {path}."
                : "Could not connect asset due to an unknown issue.",
            "OK");

        ContextMenu.Open();

        Close();
    }

    #endregion
}

}
#endif
