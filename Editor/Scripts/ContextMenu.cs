#if UNITY_EDITOR
using Editor.Scripts.Settings;
using Editor.Scripts.Windows;
using UnityEditor;

namespace Editor.Scripts
{

internal static partial class ContextMenu
{
    private const string MenuItemMegaPint = "MegaPint";
    private const string MenuItemPackages = MenuItemMegaPint + "/Packages";

    #region Public Methods

    public static MegaPintEditorWindowBase TryOpen <T>(bool utility, string title = "") where T : MegaPintEditorWindowBase
    {
        if (typeof(T) == typeof(MegaPintFirstSteps))
            return EditorWindow.GetWindow <T>(utility, title).ShowWindow();

        var exists = MegaPintSettings.Exists();

        return !exists
            ? EditorWindow.GetWindow <MegaPintFirstSteps>(utility, title).ShowWindow()
            : EditorWindow.GetWindow <T>(utility, title).ShowWindow();
    }

    #endregion

    #region Private Methods

    [MenuItem(MenuItemMegaPint + "/Open", false, 0)]
    public static void Open()
    {
        TryOpen <MegaPintBaseWindow>(false);
    }

    [MenuItem(MenuItemMegaPint + "/PackageManager", false, 11)]
    private static void OpenImporter()
    {
        MegaPintBaseWindow.OnOpenPackageManager();
    }

    #endregion
}

}
#endif
