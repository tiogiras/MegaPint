#if UNITY_EDITOR
using System.IO;
using System.Linq;
using Editor.Scripts.PackageManager.Cache;
using Editor.Scripts.Settings;
using Editor.Scripts.Windows;
using Editor.Scripts.Windows.DevMode;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

namespace Editor.Scripts
{

internal static partial class ContextMenu
{
    private const string MenuItemMegaPint = "MegaPint";
    private const string MenuItemPackages = MenuItemMegaPint + "/Packages";
    private const string MenuItemDevMode = MenuItemMegaPint + "/DevMode";

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
    private static void Test()
    {
        //PackageCache.Refresh();
        
        foreach (Object o in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(o);

            if (!Directory.Exists(path))
                continue;

            Debug.Log($"Searching in {path}");
            
            var files = Directory.GetFiles(path, "*.uxml", SearchOption.AllDirectories);

            Debug.Log($"Found{files.Length}");
            
            foreach (var file in files)
            {
                var text = File.ReadAllText(file);

                if (text.Contains("MegaPint.Editor.Scripts.GUI.Factories"))
                    continue;
                
                text = text.Replace("Editor.Scripts.GUI.Factories", "MegaPint.Editor.Scripts.GUI.Factories");
                
                File.WriteAllText(file, text);
            }
            
            AssetDatabase.Refresh();
        }
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
