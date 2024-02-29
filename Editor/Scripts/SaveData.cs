#if UNITY_EDITOR
using Editor.Scripts.Settings;

namespace Editor.Scripts
{

/// <summary> Stores references to settings values </summary>
internal static class SaveData
{
    private static MegaPintSettingsBase s_settings;

    #region Public Methods

    /// <summary> Get if the developer mode is active </summary>
    /// <returns> DeveloperMode state </returns>
    public static bool DevMode()
    {
        s_settings ??= MegaPintSettings.instance.GetSetting("General");

        return s_settings.GetValue("devMode", false);
    }

    #endregion
}

}
#endif
