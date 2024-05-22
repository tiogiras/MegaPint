﻿using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = Editor.Scripts.GUI.GUIUtility;

namespace Editor.Scripts.Windows.DevMode
{

public class Center : MegaPintEditorWindowBase
{
    private VisualTreeAsset _baseWindow;
    
    private Button _btnToggle;
    private Button _btnInterfaceOverview;

    #region Public Methods

    public override MegaPintEditorWindowBase ShowWindow()
    {
        titleContent.text = "Dev Center";

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return Path.Join(Constants.BasePackage.PathWindows, "Development Mode", "Center");
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = GUIUtility.Instantiate(_baseWindow);
        root.style.flexGrow = 1f;
        root.style.flexShrink = 1f;

        rootVisualElement.Add(root);
        
        _btnToggle = root.Q<Button>("BTN_Toggle");
        _btnInterfaceOverview = root.Q<Button>("BTN_InterfaceOverview");
        
        RegisterCallbacks();
    }

    protected override bool LoadResources()
    {
        _baseWindow = Resources.Load <VisualTreeAsset>(BasePath());

        return _baseWindow != null;
    }

    protected override void RegisterCallbacks()
    {
        _btnToggle.clicked += OnToggle;
        _btnInterfaceOverview.clicked += OnInterfaceOverview;
    }

    protected override void UnRegisterCallbacks()
    {
        _btnToggle.clicked -= OnToggle;
        _btnInterfaceOverview.clicked -= OnInterfaceOverview;
    }

    #endregion

    private static void OnToggle()
    {
        ContextMenu.TryOpen <Toggle>(false);
    }
    
    private static void OnInterfaceOverview()
    {
        ContextMenu.TryOpen <InterfaceOverview>(false);
    }
}

}