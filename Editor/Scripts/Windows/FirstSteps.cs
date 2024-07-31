#if UNITY_EDITOR
using MegaPint.Editor.Scripts.GUI.Utility;
using MegaPint.Editor.Scripts.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.Windows
{

/// <summary> Editor window to display the first steps in megapint </summary>
internal class FirstSteps : EditorWindowBase
{
    [InitializeOnLoad]
    private static class Initializer
    {
        static Initializer()
        {
            if (PlayerPrefs.HasKey("MegaPint_Initialized"))
                return;

            PlayerPrefs.SetInt("MegaPint_Initialized", 1);
            ContextMenu.BasePackage.OpenBaseWindow();
        }
    }

    #region Private

    /// <summary> Loaded uxml references </summary>
    private VisualTreeAsset _baseWindow;

    #endregion

    private Button _btnCreateAsset;

    private Button _btnNext;

    private VisualElement _tab0;
    private VisualElement _tab1;

    #region Public Methods

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "First Steps";

        minSize = new Vector2(515, 375);
        maxSize = new Vector2(515, 375);

        this.CenterOnMainWin();

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return Constants.BasePackage.UserInterface.FirstSteps;
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = rootVisualElement;

        VisualElement content = _baseWindow.Instantiate();

        content.style.flexGrow = 1f;
        content.style.flexShrink = 1f;

        #region References

        _tab0 = content.Q <VisualElement>("Tab0");
        _tab1 = content.Q <VisualElement>("Tab1");

        _btnNext = content.Q <Button>("BTN_Next");
        _btnCreateAsset = content.Q <Button>("BTN_Create");

        #endregion

        _tab0.style.display = DisplayStyle.Flex;
        _tab1.style.display = DisplayStyle.None;

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
        _btnNext.clicked += OnNext;
        _btnCreateAsset.clicked += OnBtnCreateSettingsAsset;
    }

    protected override void UnRegisterCallbacks()
    {
        if (_btnNext == null)
            return;
        
        _btnNext.clicked -= OnNext;
        _btnCreateAsset.clicked -= OnBtnCreateSettingsAsset;
    }

    #endregion

    #region Private Methods

    /// <summary> Create a settings asset for megapint in the project </summary>
    private void OnBtnCreateSettingsAsset()
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

        MegaPintMainSettings.instance = CreateInstance <MegaPintMainSettings>();
        AssetDatabase.CreateAsset(MegaPintMainSettings.instance, path);

        EditorUtility.DisplayDialog(
            "MegaPint settings asset",
            MegaPintMainSettings.instance != null
                ? $"Successfully created a new MegaPint settings asset at {path}."
                : "Could not connect asset due to an unknown issue.",
            "OK");

        ContextMenu.BasePackage.OpenBaseWindow();

        Close();
    }

    /// <summary> Button Callback </summary>
    private void OnNext()
    {
        _tab0.style.display = DisplayStyle.None;
        _tab1.style.display = DisplayStyle.Flex;
    }

    #endregion
}

}
#endif
