#if UNITY_EDITOR
using Editor.Scripts.Base;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts
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

            root.Add(content);
        }

        protected override bool LoadResources()
        {
            _baseWindow = Resources.Load<VisualTreeAsset>(BasePath());
            return _baseWindow != null;
        }

        protected override void LoadSettings() { }

        #endregion

        [MenuItem("MegaPint/Open")]
        public static void Open() => GetWindow<MegaPintBaseWindow>().ShowWindow();
    }
}
#endif