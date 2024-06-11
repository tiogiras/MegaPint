#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using MegaPint.Editor.Scripts.GUI.Utility;
using MegaPint.Editor.Scripts.PackageManager;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Windows.DevMode
{

/// <summary> Editor window to access various development mode utility </summary>
internal class Center : EditorWindowBase
{
    private VisualTreeAsset _baseWindow;
    private Button _btnImportAll;
    private Button _btnInterfaceOverview;
    private Button _btnReloadDomain;
    private Button _btnRemoveAll;
    private Button _btnRepaint;
    private Button _btnToggle;

    #region Public Methods

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "Dev Center";

        minSize = new Vector2(350, 250);
        maxSize = minSize;

        if (!SaveValues.BasePackage.ApplyPSDevCenter)
            return this;

        this.CenterOnMainWin();
        SaveValues.BasePackage.ApplyPSDevCenter = false;

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return Constants.BasePackage.UserInterface.DevModeCenter;
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = GUIUtility.Instantiate(_baseWindow);
        root.style.flexGrow = 1f;
        root.style.flexShrink = 1f;

        rootVisualElement.Add(root);

        _btnToggle = root.Q <Button>("BTN_Toggle");
        _btnInterfaceOverview = root.Q <Button>("BTN_InterfaceOverview");
        _btnRepaint = root.Q <Button>("BTN_Repaint");
        _btnReloadDomain = root.Q <Button>("BTN_ReloadDomain");
        _btnImportAll = root.Q <Button>("BTN_ImportAll");
        _btnRemoveAll = root.Q <Button>("BTN_RemoveAll");

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
        _btnRepaint.clicked += OnRepaint;
        _btnReloadDomain.clicked += OnReloadDomain;
        _btnImportAll.clicked += OnImportAll;
        _btnRemoveAll.clicked += OnRemoveAll;
    }

    protected override void UnRegisterCallbacks()
    {
        _btnToggle.clicked -= OnToggle;
        _btnInterfaceOverview.clicked -= OnInterfaceOverview;
        _btnRepaint.clicked -= OnRepaint;
        _btnReloadDomain.clicked -= OnReloadDomain;
        _btnImportAll.clicked -= OnImportAll;
        _btnRemoveAll.clicked -= OnRemoveAll;
    }

    #endregion

    #region Private Methods

    /// <summary> Import all registered megaPint packages </summary>
    private static async void OnImportAll()
    {
        if (Utility.IsProductionProject())
            return;

        Debug.Log(PackageCache.GetAllMpPackages().Count);

        foreach (CachedPackage cachedPackage in PackageCache.GetAllMpPackages())
        {
            Debug.Log($"Importing: {cachedPackage.DisplayName}");
            await MegaPintPackageManager.AddEmbedded(cachedPackage);
        }
    }

    /// <summary> Open InterfaceOverview </summary>
    private static void OnInterfaceOverview()
    {
        ContextMenu.TryOpen <InterfaceOverview>(false);
    }

    /// <summary> Force reload the domain </summary>
    private static void OnReloadDomain()
    {
        EditorUtility.RequestScriptReload();
    }

    /// <summary> Remove all imported megaPint packages </summary>
    private static async void OnRemoveAll()
    {
        if (Utility.IsProductionProject())
            return;

        PackageCache.GetInstalledMpPackages(out List <CachedPackage> packages, out List <CachedVariation> variations);

        if (packages.Count > 0)
        {
            foreach (CachedPackage cachedPackage in packages)
                await MegaPintPackageManager.Remove(cachedPackage.Name);
        }

        if (variations.Count == 0)
            return;

        foreach (CachedVariation cachedVariation in variations)
            await MegaPintPackageManager.Remove(PackageCache.Get(cachedVariation.key).Name);
    }

    /// <summary> Call Force Repaint </summary>
    private static void OnRepaint()
    {
        GUIUtility.ForceRepaint();
    }

    /// <summary> Open Toggle </summary>
    private static void OnToggle()
    {
        ContextMenu.TryOpen <Toggle>(true);
    }

    #endregion
}

}
#endif
