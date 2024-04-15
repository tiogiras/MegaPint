#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Editor.Scripts.Factories;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.PackageManager.Packages;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = Editor.Scripts.GUI.GUIUtility;

namespace Editor.Scripts.Windows
{

internal class Gallery : MegaPintEditorWindowBase
{
    private const string ImageFolderPath = "MegaPint/Images/Packages/xxx/";
    
    private VisualTreeAsset _baseWindow;

    private AspectRatioPanel _image;
    private VisualElement _left;
    private VisualElement _right;

    private List <Texture2D> _images = new();
    private int _currentImage;

    #region Public Methods

    public override MegaPintEditorWindowBase ShowWindow()
    {
        titleContent.text = "Gallery";

        return this;
    }

    public void Initialize(CachedPackage package)
    {
        _images.Clear();
        var folderPath = ImageFolderPath.Replace("xxx", package.Key.ToString());

        foreach (var image in package.Images)
        {
            _images.Add(Resources.Load<Texture2D>(Path.Combine(folderPath, image)));
        }
        
        SetImage();
        
        if (_images.Count <= 1)
            return;

        _left.style.display = DisplayStyle.Flex;
        _right.style.display = DisplayStyle.Flex;
    }

    #endregion

    private void SetImage()
    {
        Texture2D image = _images[_currentImage];
        _image.style.backgroundImage = image;
        _image.aspectRatioX = image.width;
        _image.aspectRatioY = image.height;
        _image.FitToParent();
    }
    
    #region Protected Methods

    protected override string BasePath()
    {
        return "MegaPint/User Interface/Windows/Package Manager/Gallery";
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = rootVisualElement;

        VisualElement content = GUIUtility.Instantiate(_baseWindow, root);
        content.style.flexGrow = 1f;

        _image = content.Q <AspectRatioPanel>();

        _left = content.Q <VisualElement>("Left");
        _right = content.Q <VisualElement>("Right");

        _left.style.display = DisplayStyle.None;
        _right.style.display = DisplayStyle.None;
        
        RegisterCallbacks();
    }

    protected override bool LoadResources()
    {
        _baseWindow = Resources.Load <VisualTreeAsset>(BasePath());

        return _baseWindow != null;
    }

    protected override void RegisterCallbacks()
    {
        _left.RegisterCallback<MouseEnterEvent>(MouseEnter);
        _left.RegisterCallback<MouseOutEvent>(MouseOut);
        _left.RegisterCallback<MouseUpEvent>(Left, TrickleDown.TrickleDown);
        
        _right.RegisterCallback<MouseEnterEvent>(MouseEnter);
        _right.RegisterCallback<MouseOutEvent>(MouseOut);
        _right.RegisterCallback<MouseUpEvent>(Right, TrickleDown.TrickleDown);
    }
    
    private void Left(MouseUpEvent evt)
    {
        if (_currentImage == 0)
            _currentImage = _images.Count - 1;
        else
            _currentImage--;
        
        SetImage();
    }
    
    private void Right(MouseUpEvent evt)
    {
        if (_currentImage == _images.Count - 1)
            _currentImage = 0;
        else
            _currentImage++;
        
        SetImage();
    }

    private void MouseEnter(MouseEnterEvent evt)
    {
        ((VisualElement)evt.target).style.opacity = .75f;
    }
    
    private void MouseOut(MouseOutEvent evt)
    {
        ((VisualElement)evt.target).style.opacity = .3f;
    }

    protected override void UnRegisterCallbacks()
    {

    }
    
    

    #endregion
}

}
#endif
