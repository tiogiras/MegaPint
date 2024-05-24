using System.Collections.Generic;
using System.IO;
using MegaPint.Editor.Scripts;
using MegaPint.Editor.Scripts.GUI.Utility;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace Editor.Scripts.Windows.DevMode
{

public class InterfaceOverview : MegaPintEditorWindowBase
{
    private VisualTreeAsset _baseWindow;

    #region Public Methods

    public override MegaPintEditorWindowBase ShowWindow()
    {
        titleContent.text = "Interface Overview";

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return Path.Join(Constants.BasePackage.Resources.UserInterface.WindowsPath, "Development Mode", "Interface Overview");
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = GUIUtility.Instantiate(_baseWindow);
        root.style.flexGrow = 1f;
        root.style.flexShrink = 1f;
        
        root.ActivateLinks(_ => {});

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

    private void SetupAllLists(VisualElement root)
    {
        root.Query <ListView>().ForEach(ListSetup);
    }
    
    private static void ListSetup(ListView list)
    {
        list.makeItem = () => new VisualElement();
        
        list.bindItem = (e, i) =>
        {
            e.Add(new Label($"Item {i}"));
        };
        
        list.itemsSource = new List<int> {0, 1, 2, 3, 4, 5};
    }
}

}
