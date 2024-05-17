#if UNITY_EDITOR
using Editor.Scripts.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Windows
{
    internal class MegaPintFirstSteps : MegaPintEditorWindowBase
    {
        #region Visual References

        private Button _createAsset;

        #endregion

        #region Private

        /// <summary> Loaded uxml references </summary>
        private VisualTreeAsset _baseWindow;

        #endregion

        #region Override Methods

        protected override string BasePath() => "MegaPint/User Interface/Windows/First Steps";

        public override MegaPintEditorWindowBase ShowWindow()
        {
            titleContent.text = "First Steps";
            return this;
        }
        
        protected override void CreateGUI()
        {
            base.CreateGUI();

            VisualElement root = rootVisualElement;

            VisualElement content = _baseWindow.Instantiate();

            content.style.flexGrow = 1f;
            content.style.flexShrink = 1f;

            #region References

            _createAsset = content.Q<Button>("BTN_Create");

            #endregion

            RegisterCallbacks();

            root.Add(content);
        }

        protected override bool LoadResources()
        {
            _baseWindow = Resources.Load<VisualTreeAsset>(BasePath());
            return _baseWindow != null;
        }

        protected override void RegisterCallbacks() => _createAsset.clicked += OnCreateSettingsAsset;

        protected override void UnRegisterCallbacks() => _createAsset.clicked -= OnCreateSettingsAsset;

        #endregion

        #region Callback Methods

        private void OnCreateSettingsAsset()
        {
            var path = EditorUtility.SaveFilePanel(
                "MegaPint Settings", 
                "Assets", 
                "MegaPintSettings",
                "asset");

            if (!path.StartsWith(Application.dataPath))
            {
                EditorUtility.DisplayDialog(
                    "MegaPint settings asset",
                    $"Could not create asset at {path}.\nMake sure it is located in the assets folder.",
                    "OK");
                
                return;
            }
                

            path = path.Replace(Application.dataPath, "Assets");
            
            MegaPintSettings.instance = CreateInstance<MegaPintSettings>();
            AssetDatabase.CreateAsset(MegaPintSettings.instance, path);

            EditorUtility.DisplayDialog(
                "MegaPint settings asset",
                MegaPintSettings.instance != null
                    ? $"Successfully created a new MegaPint settings asset at {path}."
                    : "Could not connect asset due to an unknown issue.",
                "OK");
            
            ContextMenu.Open();
            
            Close();
        }

        #endregion
    }
}
#endif