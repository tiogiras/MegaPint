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
        
        [SerializeField] private List<MegaPintSettingsBase> settings;

        private void AddSetting(MegaPintSettingsBase setting)
        {
            settings.Add(setting);
            Save();
        }

        public MegaPintSettingsBase GetSetting(string settingName)
        {
            settings ??= new List<MegaPintSettingsBase>();
            
            if (settings.Count > 0)
            {
                foreach (var setting in settings.Where(setting => setting.name.Equals(settingName)))
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

        public static void Save()
        {
            EditorUtility.SetDirty(Instance);
            AssetDatabase.SaveAssetIfDirty(Instance);
        }
    }

    [CustomEditor(typeof(MegaPintSettings))]
    public class MegaPintSettingsDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("This asset is used to store all MegaPint settings.");
        }
    }
}
#endif