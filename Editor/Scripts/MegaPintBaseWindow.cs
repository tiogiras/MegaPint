using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts
{
    public class MegaPintBaseWindow : MegaPintEditorWindowBase
    {
        private const string BasePath = "User Interface/MegaPintBaseWindow";

        private static MegaPintBaseWindow _instance;
        private static VisualTreeAsset _baseWindow;

        [MenuItem("MegaPint/Open")]
        private static void ShowWindow()
        {
            if (!LoadResourceContent())
                return;

            _instance ??= CreateInstance<MegaPintBaseWindow>();
            _instance.titleContent.text = "MegaPint";
            _instance.Show();
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private static bool LoadResourceContent()
        {
            Debug.Log("loading...");
            _baseWindow = Resources.Load<VisualTreeAsset>(BasePath);
            return _baseWindow != null;
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;

            VisualElement content = _baseWindow.Instantiate();

            root.Add(content);
        }
    }
}