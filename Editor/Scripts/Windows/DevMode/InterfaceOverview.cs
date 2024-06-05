#if UNITY_EDITOR
using System.Collections.Generic;
using MegaPint.Editor.Scripts.GUI.Utility;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Windows.DevMode
{

/// <summary> Editor window to display various combinations of interface elements </summary>
public class InterfaceOverview : EditorWindowBase
{
    private VisualTreeAsset _baseWindow;

    #region Public Methods

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "Interface Overview";

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return Constants.BasePackage.UserInterface.InterfaceOverview;
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = GUIUtility.Instantiate(_baseWindow);
        root.style.flexGrow = 1f;
        root.style.flexShrink = 1f;

        root.ActivateLinks(_ => { });

        rootVisualElement.Add(root);

        SetupAllLists(root);
    }

    protected override bool LoadResources()
    {
        _baseWindow = Resources.Load <VisualTreeAsset>(BasePath());

        return _baseWindow != null;
    }

    protected override void RegisterCallbacks()
    {
    }

    protected override void UnRegisterCallbacks()
    {
    }

    #endregion

    #region Private Methods

    /// <summary> Execute setup for a list view </summary>
    /// <param name="list"> Targeted list view </param>
    private static void ListSetup(ListView list)
    {
        list.makeItem = () => new VisualElement();

        list.bindItem = (e, i) => {e.Add(new Label($"Item {i}"));};

        list.itemsSource = new List <int>
        {
            0,
            1,
            2,
            3,
            4,
            5
        };
    }

    /// <summary> Call setup for all list views </summary>
    /// <param name="root"> Root element containing the list views </param>
    private static void SetupAllLists(VisualElement root)
    {
        root.Query <ListView>().ForEach(ListSetup);
    }

    #endregion
}

}
#endif
