#if UNITY_EDITOR
using System;
using Editor.Scripts.Settings;
using UnityEditor;

namespace Editor.Scripts.Windows
{
    public abstract class MegaPintEditorWindowBase : EditorWindow
    {
        #region Public

        public Action<MegaPintEditorWindowBase> onCreate;
        public Action<MegaPintEditorWindowBase> onClose;

        #endregion

        protected virtual void CreateGUI()
        {
            if (!LoadResources())
                return;

            LoadSettings();
            onCreate?.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            UnRegisterCallbacks();
            onClose?.Invoke(this);
        }
        
        protected abstract string BasePath();
        
        public abstract MegaPintEditorWindowBase ShowWindow();
        
        protected abstract bool LoadResources();
        
        protected virtual bool LoadSettings() => MegaPintSettings.Exists();
        
        protected abstract void RegisterCallbacks();
        
        protected abstract void UnRegisterCallbacks();
    }
}
#endif