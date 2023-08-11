#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Windows
{
    public class MegaPintBaseWindow : MegaPintEditorWindowBase
    {
        /// <summary> Loaded reference of the uxml </summary>
        private VisualTreeAsset _baseWindow;

        #region Overrides

        protected override string BasePath() => "User Interface/MegaPintBaseWindow";
        
        public override MegaPintEditorWindowBase ShowWindow()
        {
            titleContent.text = "MegaPint";
            return this;
        }

        protected override void CreateGUI()
        {
            base.CreateGUI();

            var root = rootVisualElement;

            VisualElement content = _baseWindow.Instantiate();

            content.Q<Button>("OpenImporter").clicked += OpenImporter;

            root.Add(content);
        }

        protected override bool LoadResources()
        {
            _baseWindow = Resources.Load<VisualTreeAsset>(BasePath());
            return _baseWindow != null;
        }

        protected override void LoadSettings() { }

        #endregion
        
        public static void OpenImporter() => GetWindow<MegaPintPackageManagerWindow>(true).ShowWindow();
    }
}
#endif