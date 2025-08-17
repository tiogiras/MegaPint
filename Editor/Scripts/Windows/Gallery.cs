#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using MegaPint.Editor.Scripts.GUI.Factories.Structure;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Windows
{

/// <summary> Editor window to display a collection of images </summary>
internal class Gallery : EditorWindowBase
{
    private static string s_imagesPath;

    private static string _ImageFolderPath =>
        s_imagesPath ??= Path.Combine(Constants.BasePackage.Images.PackageImages, "xxx");

    private readonly List <Texture2D> _images = new();

    private VisualTreeAsset _baseWindow;
    private int _currentImage;

    private AspectRatioPanel _image;
    private VisualElement _left;
    private VisualElement _right;

    #region Public Methods

    /// <summary> Initialize the gallery with the targeted package </summary>
    /// <param name="package"> Targeted package </param>
    public void Initialize(CachedPackage package)
    {
        _images.Clear();
        var folderPath = _ImageFolderPath.Replace("xxx", package.Key.ToString());

        foreach (var image in package.Images)
            _images.Add(Resources.Load <Texture2D>(Path.Combine(folderPath, image)));

        SetImage();

        if (_images.Count <= 1)
            return;

        _left.style.display = DisplayStyle.Flex;
        _right.style.display = DisplayStyle.Flex;
    }

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "Gallery";

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return Constants.BasePackage.UserInterface.Gallery;
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
        _left.RegisterCallback <MouseEnterEvent>(MouseEnter);
        _left.RegisterCallback <MouseOutEvent>(MouseOut);
        _left.RegisterCallback <MouseUpEvent>(Left, TrickleDown.TrickleDown);

        _right.RegisterCallback <MouseEnterEvent>(MouseEnter);
        _right.RegisterCallback <MouseOutEvent>(MouseOut);
        _right.RegisterCallback <MouseUpEvent>(Right, TrickleDown.TrickleDown);
    }

    protected override void UnRegisterCallbacks()
    {
    }

    #endregion

    #region Private Methods

    /// <summary> Mouse enter callback </summary>
    /// <param name="evt"> Mouse callback </param>
    private static void MouseEnter(MouseEnterEvent evt)
    {
        ((VisualElement)evt.target).style.opacity = .75f;
    }

    /// <summary> Mouse out callback </summary>
    /// <param name="evt"> Mouse callback </param>
    private static void MouseOut(MouseOutEvent evt)
    {
        ((VisualElement)evt.target).style.opacity = .3f;
    }

    /// <summary> Show the previous image </summary>
    /// <param name="evt"> Mouse event </param>
    private void Left(MouseUpEvent evt)
    {
        if (_currentImage == 0)
            _currentImage = _images.Count - 1;
        else
            _currentImage--;

        SetImage();
    }

    /// <summary> Show the next image </summary>
    /// <param name="evt"> Mouse event </param>
    private void Right(MouseUpEvent evt)
    {
        if (_currentImage == _images.Count - 1)
            _currentImage = 0;
        else
            _currentImage++;

        SetImage();
    }

    /// <summary> Set the image to the current image </summary>
    private void SetImage()
    {
        Texture2D image = _images[_currentImage];
        _image.style.backgroundImage = image;
        _image.AspectRatioX = image.width;
        _image.AspectRatioY = image.height;
        _image.FitToParent();
    }

    #endregion
}

}
#endif
