#if UNITY_EDITOR
using System;
using UnityEditor;

namespace Editor.Scripts.Windows
{
    public abstract class MegaPintEditorWindowBase : EditorWindow
    {
        public Action<MegaPintEditorWindowBase> OnCreate;
        public Action<MegaPintEditorWindowBase> OnClose;

        protected virtual void CreateGUI()
        {
            if (!LoadResources())
                return;

            LoadSettings();
            OnCreate?.Invoke(this);
        }

        protected virtual void OnDestroy() => OnClose?.Invoke(this);
        protected abstract string BasePath();
        public abstract MegaPintEditorWindowBase ShowWindow();
        protected abstract bool LoadResources();
        protected abstract void LoadSettings();
    }
}
#endif