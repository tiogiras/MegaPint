#if UNITY_EDITOR
using Editor.Scripts.Settings;
using Editor.Scripts.Windows;
using UnityEditor;

namespace Editor.Scripts
{
    public static partial class ContextMenu
    {
        [MenuItem("MegaPint/Open", false, 0)]
        private static void Open() => TryOpen<MegaPintBaseWindow>(false);

        [MenuItem("MegaPint/PackageManager", false, 11)]
        private static void OpenImporter() => MegaPintBaseWindow.OpenImporter();

        public static MegaPintEditorWindowBase TryOpen<T>(bool utility) where T : MegaPintEditorWindowBase
        {
            if (typeof(T) == typeof(MegaPintFirstSteps)) return EditorWindow.GetWindow<T>(utility).ShowWindow();

            var exists = MegaPintSettings.Exists();
            
            return ! exists
                ? EditorWindow.GetWindow<MegaPintFirstSteps>(utility).ShowWindow() 
                : EditorWindow.GetWindow<T>(utility).ShowWindow();
        }
    }
}
#endif