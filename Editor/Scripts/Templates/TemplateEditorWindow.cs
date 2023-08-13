using Editor.Scripts.Windows;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Templates
{
    public class TemplateEditorWindow : MegaPintEditorWindowBase
    {
        #region Const

        // TEMPLATE [ Store const variables here ]

        #endregion 

        #region Static

        // TEMPLATE [ Store static variables here ]

        #endregion

        #region Public

        // TEMPLATE [ Store public variables here ]

        #endregion

        #region Visual References

        // TEMPLATE [ Store references to visual elements here ] 

        #endregion

        #region Private

        /// <summary> Loaded uxml references </summary>
        private VisualTreeAsset _baseWindow;
        
        // TEMPLATE [ Store private variables here ]

        #endregion

        #region Override Methods
        
        protected override string BasePath() => ""; // TEMPLATE [ Change to actual uxml path inside of Resources ]

        public override MegaPintEditorWindowBase ShowWindow()
        {
            titleContent.text = ""; // TEMPLATE [ Change to name of window ]
            return this;
        }

        protected override void CreateGUI()
        {
            base.CreateGUI();
            
            var root = rootVisualElement;

            VisualElement content = _baseWindow.Instantiate();

            #region References

            // TEMPLATE [ Get all reference to visual elements that are needed ]

            #endregion
            
            // TEMPLATE [ Insert GUI logic here ]
            
            RegisterCallbacks();
            
            root.Add(content);
        }

        protected override bool LoadResources()
        {
            // TEMPLATE [ Load additional References if needed ]
            _baseWindow = Resources.Load<VisualTreeAsset>(BasePath());
            return _baseWindow != null;
        }

        protected override void RegisterCallbacks()
        {
            // TEMPLATE [ Register all callbacks ]
            // Needs to be called in CreateGUI()
        }

        protected override void UnRegisterCallbacks()
        {
            // TEMPLATE [ UnRegister all callbacks ]
            // Is automatically called in base.OnDestroy()
        }

        #endregion

        #region Public Methods

        // TEMPLATE [ Store public methods here ]

        #endregion

        #region Callback Methods

        // TEMPLATE [ Store callback methods here ]

        #endregion
        
        #region Internal Methods

        // TEMPLATE [ Store private methods here ]

        #endregion
    }
}