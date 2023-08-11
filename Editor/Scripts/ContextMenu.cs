#if UNITY_EDITOR
using Editor.Scripts.Windows;
using UnityEditor;

namespace Editor.Scripts
{
    public static partial class ContextMenu
    {
        [MenuItem("MegaPint/Open", false, 0)]
        public static void Open() => EditorWindow.GetWindow<MegaPintBaseWindow>().ShowWindow();

        [MenuItem("MegaPint/PackageManager", false, 11)]
        private static void OpenImporter() => MegaPintBaseWindow.OpenImporter();
    }
}
#endif