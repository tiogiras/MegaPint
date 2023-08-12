#if UNITY_EDITOR
using Editor.Scripts.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Windows
{
    public class MegaPintFirstSteps : MegaPintEditorWindowBase
    {
        /// <summary> Loaded reference of the uxml </summary>
        private VisualTreeAsset _baseWindow;

        private Button _createAsset;
        
        #region Overrides

        protected override string BasePath() => "User Interface/First Steps/MegaPintFirstSteps";

        public override MegaPintEditorWindowBase ShowWindow()
        {
            titleContent.text = "First Steps";
            return this;
        }
        
        protected override void CreateGUI()
        {
            base.CreateGUI();

            var root = rootVisualElement;

            VisualElement content = _baseWindow.Instantiate();

            _createAsset = content.Q<Button>("BTN_Create");
            _createAsset.clicked += CreateSettingsAsset;
            
            root.Add(content);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _createAsset.clicked -= CreateSettingsAsset;
        }

        protected override bool LoadResources()
        {
            _baseWindow = Resources.Load<VisualTreeAsset>(BasePath());
            return _baseWindow != null;
        }

        protected override void LoadSettings() { }

        #endregion
        
        private void CreateSettingsAsset()
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
            
            MegaPintSettings.Instance = CreateInstance<MegaPintSettings>();
            AssetDatabase.CreateAsset(MegaPintSettings.Instance, path);

            EditorUtility.DisplayDialog(
                "MegaPint settings asset",
                MegaPintSettings.Instance != null
                    ? $"Successfully created a new MegaPint settings asset at {path}."
                    : "Could not connect asset due to an unknown issue.",
                "OK");
            
            Close();
        }
    }
}
#endif