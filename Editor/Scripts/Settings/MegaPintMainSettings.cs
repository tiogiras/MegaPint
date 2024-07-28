#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor.Scripts.Settings
{

[InitializeOnLoad]
internal class MegaPintMainSettings : ScriptableObject
{
    public static MegaPintMainSettings instance;

    public static Action onLoaded;

    [SerializeField] private List <SettingsBase> _settings;

    static MegaPintMainSettings()
    {
        Initialize();
    }

    #region Public Methods

    /// <summary> Check if the Settings file exists </summary>
    /// <returns> If it exists </returns>
    public static bool Exists()
    {
        if (instance != null)
            return true;

        var search = AssetDatabase.FindAssets($"t:{nameof(MegaPintMainSettings)}", new[] {"Assets"});

        if (search.Length == 0)
            return false;

        instance = AssetDatabase.LoadAssetAtPath <MegaPintMainSettings>(AssetDatabase.GUIDToAssetPath(search[0]));

        return instance != null;
    }

    /// <summary> Save the settings file </summary>
    public static void Save()
    {
        EditorUtility.SetDirty(instance);
        AssetDatabase.SaveAssetIfDirty(instance);
    }

    /// <summary> Get a setting </summary>
    /// <param name="settingName"> Name of the setting </param>
    /// <returns> Found setting </returns>
    public SettingsBase GetSetting(string settingName)
    {
        _settings ??= new List <SettingsBase>();

        if (_settings.Count > 0)
        {
            foreach (SettingsBase setting in _settings.Where(setting => setting.name.Equals(settingName)))
                return setting;
        }

        var newSetting = new SettingsBase(settingName);
        AddSetting(newSetting);

        return newSetting;
    }

    #endregion

    #region Private Methods

    /// <summary> Initialize the settings file </summary>
    private static async void Initialize()
    {
        await Task.Delay(10);

        if (!Exists())
            return;

        onLoaded?.Invoke();

#pragma warning disable CS4014
        Utility.ValidateTesterToken();
#pragma warning restore CS4014
    }

    /// <summary> Add a setting </summary>
    /// <param name="setting"> New setting </param>
    private void AddSetting(SettingsBase setting)
    {
        _settings.Add(setting);
        Save();
    }

    #endregion
}

[CustomEditor(typeof(MegaPintMainSettings))]
public class MegaPintSettingsDrawer : UnityEditor.Editor
{
    #region Public Methods

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("This asset is used to store all MegaPint settings.");
    }

    #endregion
}

}
#endif
