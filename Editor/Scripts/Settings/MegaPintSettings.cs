#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.Settings
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SettingsData", order = 1)]
    public class MegaPintSettings : ScriptableObject
    {
        public static MegaPintSettings Instance;
        
        private List<MegaPintSettingsBase> _settings;

        private void AddSetting(MegaPintSettingsBase setting)
        {
            _settings.Add(setting);
            EditorUtility.SetDirty(this);
        }

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
        
        public static bool Exists()
        {
            if (Instance != null)
                return true;
            
            var search = AssetDatabase.FindAssets("t:MegaPintSettings", new[] { "Assets" });

            if (search.Length == 0)
                return false;
            
            Instance = AssetDatabase.LoadAssetAtPath<MegaPintSettings>(AssetDatabase.GUIDToAssetPath(search[0]));

            return Instance != null;
        }
    }
}
#endif