#if UNITY_EDITOR
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.Settings;
using Editor.Scripts.Windows;
using UnityEditor;
using UnityEngine;

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
    
    [MenuItem(MenuItemMegaPint + "/Test", false, 0)]
    public static void Test()
    {
        //PackageCache.Refresh();
        //EditorWindow.GetWindow<MegaPintPackageManagerWindow>().Close();

        var tex = EditorGUIUtility.Load("IN foldout on.png") as Texture2D;


        Debug.Log(AssetDatabase.GetAssetPath(tex));
        
        Debug.Log($"{tex.width} | {tex.height}");

        /*
        for (int i = 0; i < tex.height; i++)
        {
            var str = "";

            for (int j = 0; j < tex.width; j++)
            {
                str += tex.GetPixel(i, j);
            }
            Debug.Log(str);
        }*/
    }

    [MenuItem(MenuItemMegaPint + "/Open", false, 0)]
    public static void Open()
    {
        TryOpen <BaseWindow>(false);
    }

    [MenuItem(MenuItemMegaPint + "/PackageManager", false, 11)]
    private static void OpenImporter()
    {
        BaseWindow.OnOpenPackageManager();
    }

    #endregion
}

}
#endif
