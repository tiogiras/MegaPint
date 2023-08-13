#if UNITY_EDITOR
using System;
using Editor.Scripts.Settings;
using UnityEditor;

namespace Editor.Scripts.Windows
{
    public abstract class MegaPintEditorWindowBase : EditorWindow
    {
        #region Public

        public Action<MegaPintEditorWindowBase> OnCreate;
        public Action<MegaPintEditorWindowBase> OnClose;

        #endregion

        protected virtual void CreateGUI()
        {
            if (!LoadResources())
                return;

            LoadSettings();
            OnCreate?.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            UnRegisterCallbacks();
            OnClose?.Invoke(this);
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