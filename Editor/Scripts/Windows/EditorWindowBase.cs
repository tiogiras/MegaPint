#if UNITY_EDITOR
using MegaPint.Editor.Scripts.DevMode;
using MegaPint.Editor.Scripts.Settings;
using UnityEditor;

namespace MegaPint.Editor.Scripts.Windows
{

/// <summary> Abstract base class for editor windows </summary>
internal abstract class EditorWindowBase : EditorWindow
{
    #region Unity Event Functions

    protected virtual void OnDestroy()
    {
        UnRegisterCallbacks();
    }

    #endregion

    #region Public Methods

    /// <summary> Display the editor window </summary>
    /// <returns> This editor window as <see cref="EditorWindowBase" /> </returns>
    public abstract EditorWindowBase ShowWindow();

    #endregion

    #region Protected Methods

    /// <summary> Path to the base uxml file </summary>
    /// <returns> Path to the selected uxml file </returns>
    protected abstract string BasePath();

    protected virtual void CreateGUI()
    {
        if (!LoadResources())
        {
            DevLog.LogError($"Could not load resources for window: {GetType().Name}");

            return;
        }

        LoadSettings();
    }

    /// <summary> Load all needed uxml and resource references </summary>
    /// <returns> True when all resources can be loaded </returns>
    protected abstract bool LoadResources();

    /// <summary> Check if the settings exist and load needed settings </summary>
    /// <returns> If all needed settings can be loaded </returns>
    protected virtual bool LoadSettings()
    {
        return MegaPintMainSettings.Exists();
    }

    /// <summary> Register all callbacks </summary>
    protected abstract void RegisterCallbacks();

    /// <summary> Unregister all callbacks </summary>
    protected abstract void UnRegisterCallbacks();

    #endregion
}

}
#endif
