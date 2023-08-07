using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.Base;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SettingsData", order = 1)]
    public class MegaPintSettings : ScriptableObject
    {
        [SerializeField] private List<MegaPintSettingsBase> _settings;

        public void AddSetting(MegaPintSettingsBase settings) => _settings.Add(settings);

        public MegaPintSettingsBase GetSetting(string settingName)
        {
            _settings ??= new List<MegaPintSettingsBase>();
            
            if (_settings.Count > 0)
            {
                foreach (var setting in _settings.Where(setting => setting.name.Equals(settingName)))
                {
                    return setting;
                }
            }

            var newSetting = new MegaPintSettingsBase(settingName);
            AddSetting(newSetting);

            return newSetting;
        }

        public static MegaPintSettings Get() => Resources.Load<MegaPintSettings>("MegaPintSettings");

        [MenuItem("MegaPint/Log")]
        public static void Log()
        {
            var settings = Get();
            foreach (var setting in settings._settings)    
            {
                Debug.Log(setting.name);
                if (setting.intValues is { Count: > 0 })
                {
                    foreach (var value in setting.intValues.entries)
                    {
                        Debug.Log($"{value.key} | {value.value}");
                    }
                }
            }
        }
    }
}