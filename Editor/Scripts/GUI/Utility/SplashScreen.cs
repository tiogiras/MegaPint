#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Editor.Scripts.PackageManager.Cache;
using UnityEngine;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Utility
{

/// <summary> Partial utility class containing splash screen specific utility functions </summary>
public static partial class GUIUtility
{
    private static string s_splashScreenPath;

    private static string _SplashScreenPath =>
        s_splashScreenPath ??= Path.Combine(Constants.BasePackage.Resources.UserInterface.WindowsPath, "Splash Screen");

    #region Public Methods

    /// <summary> Display a splash screen on the given root element </summary>
    /// <param name="root"> Element the splash screen should be added to </param>
    /// <param name="method"> Action to be invoked after the splash screen </param>
    /// <param name="refreshRate"> RefreshRate of the splash screen </param>
    /// <param name="speed"> Animation speed of the splash screen </param>
    public static async void DisplaySplashScreen(
        VisualElement root,
        Action method,
        int refreshRate = 1,
        float speed = .05f)
    {
        root.Clear();

        VisualElement splashScreen = CreateSplashScreen(
            root,
            out VisualElement[] loadingIcon,
            out VisualElement logo);

        var cacheRefreshed = false;

        PackageCache.onCacheRefreshed += () => {cacheRefreshed = true;};

        var loadingTime = 0;
        var fadeInProgress = 0f;

        var targetProgress = 0f;
        var currentProgress = 0f;

        PackageCache.onCacheProgressChanged += progress => {targetProgress = progress;};

        var progressBar = splashScreen.Q <VisualElement>("Progress");
        progressBar.style.width = 0;

        var processText = splashScreen.Q <Label>("Process");

        PackageCache.onCacheProcessChanged += process => {processText.text = process;};

        while (!cacheRefreshed || fadeInProgress < 1 || Math.Abs(targetProgress - currentProgress) > 0.001f)
        {
            if (fadeInProgress < 1)
            {
                fadeInProgress += speed;
                fadeInProgress = Mathf.Clamp01(fadeInProgress);
                logo.style.opacity = fadeInProgress;
            }

            loadingTime += refreshRate;
            HandleLoadingIcon(loadingIcon, loadingTime);

            currentProgress = Mathf.Lerp(currentProgress, targetProgress, refreshRate * speed * 4);
            currentProgress = Mathf.Clamp01(currentProgress);

            progressBar.style.width = Length.Percent(currentProgress * 100);

            await Task.Delay(refreshRate);
        }

        splashScreen.RemoveFromHierarchy();

        method?.Invoke();
    }

    #endregion

    #region Private Methods

    /// <summary> Create a splash screen on the given root element </summary>
    /// <param name="root"> Element the splash screen should be added to </param>
    /// <param name="loadingIcon"> Loading icon elements of the splash screen </param>
    /// <param name="logo"> Logo of the splash screen </param>
    /// <returns> The created splash screen </returns>
    private static VisualElement CreateSplashScreen(
        VisualElement root,
        out VisualElement[] loadingIcon,
        out VisualElement logo)
    {
        var treeAsset = Resources.Load <VisualTreeAsset>(_SplashScreenPath);

        VisualElement splashScreen = Instantiate(treeAsset, root);
        splashScreen.style.flexGrow = 1;
        splashScreen.style.flexShrink = 1;

        loadingIcon = GetLoadingIcon(splashScreen.Q("Loading"));
        logo = splashScreen.Q("Logo");

        return splashScreen;
    }

    /// <summary> Collect all parts of the loading icon </summary>
    /// <param name="root"> Parent <see cref="VisualElement" /> containing the loading icon elements </param>
    /// <returns> Collected loading icon parts </returns>
    private static VisualElement[] GetLoadingIcon(VisualElement root)
    {
        VisualElement[] elements = new VisualElement[12];

        for (var i = 1; i <= 12; i++)
        {
            VisualElement element = root.Q($"Loading{i}");
            elements[i - 1] = element;
            element.style.opacity = 0;
        }

        return elements;
    }

    /// <summary> Handle the current status of the loading icon </summary>
    /// <param name="elements"> All parts of the loading icon </param>
    /// <param name="loadingTime"> Current loading time </param>
    private static void HandleLoadingIcon(IReadOnlyList <VisualElement> elements, int loadingTime)
    {
        const float CycleTime = 20f;

        for (var i = 0; i < 12; i++)
        {
            var individualTime = loadingTime + i * CycleTime / 6;

            VisualElement element = elements[i];

            var fadeIn = Mathf.CeilToInt(individualTime / CycleTime) % 2 == 0;

            var rest = individualTime % CycleTime;

            if (rest == 0)
                continue;

            var opacity = Mathf.Lerp(fadeIn ? 0 : 1, fadeIn ? 1 : 0, rest / CycleTime);

            element.style.opacity = opacity;
        }
    }

    #endregion
}

}
#endif
