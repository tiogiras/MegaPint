#if UNITY_EDITOR
using MegaPint.Editor.Scripts.Settings;

namespace Editor.Scripts
{

/// <summary> Stores references to settings values </summary>
internal static class SaveData
{
    private static SettingsBase s_settings;

    #region Public Methods

    /// <summary> Get if the developer mode is active </summary>
    /// <returns> DeveloperMode state </returns>
    public static bool DevMode()
    {
        s_settings ??= MegaPint.Editor.Scripts.Settings.Settings.instance.GetSetting("General");

        return s_settings.GetValue("devMode", false);
    }

    #endregion
}

}
#endif
