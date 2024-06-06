#if UNITY_EDITOR
using MegaPint.Editor.Scripts.Settings;
using MegaPint.Editor.Scripts.Windows;
using UnityEditor;

namespace MegaPint.Editor.Scripts
{

/// <summary> Partial class used to store MenuItems </summary>
internal static partial class ContextMenu
{
    private const string MenuItemMegaPint = "MegaPint";
    public const string MenuItemPackages = MenuItemMegaPint + "/Packages";

    #region Public Methods

    [MenuItem(MenuItemMegaPint + "/Open", false, 0)]
    public static void Open()
    {
        TryOpen <BaseWindow>(false);
    }

    /// <summary> Try opening an editor window derived from the <see cref="EditorWindowBase" /> </summary>
    /// <param name="utility"> If the window should be an utility window </param>
    /// <param name="title"> Title of the window </param>
    /// <typeparam name="T"> Type of the wanted window </typeparam>
    /// <returns> Editor window of the selected parameters </returns>
    public static EditorWindowBase TryOpen <T>(bool utility, string title = "") where T : EditorWindowBase
    {
        if (typeof(T) == typeof(FirstSteps))
            return EditorWindow.GetWindow <T>(utility, title).ShowWindow();

        var exists = MegaPintSettings.Exists();

        return !exists
            ? EditorWindow.GetWindow <FirstSteps>(utility, title).ShowWindow()
            : EditorWindow.GetWindow <T>(utility, title).ShowWindow();
    }

    #endregion

    #region Private Methods

    [MenuItem(MenuItemMegaPint + "/PackageManager", false, 11)]
    private static void OpenImporter()
    {
        BaseWindow.OnOpenPackageManager();
    }

    #endregion
}

}
#endif
