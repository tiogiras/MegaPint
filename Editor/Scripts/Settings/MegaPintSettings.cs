#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Editor.Scripts.Settings
{
    [InitializeOnLoad]
    internal class MegaPintSettings : ScriptableObject
    {
        public static MegaPintSettings instance;

        public static Action onLoaded;

        [SerializeField] private List<MegaPintSettingsBase> _settings;

        static MegaPintSettings()
        {
            Initialize();
        }

        private static async void Initialize()
        {
            await Task.Delay(10);

            if(Exists())
                onLoaded?.Invoke();
        }
        

        private void AddSetting(MegaPintSettingsBase setting)
        {
            _settings.Add(setting);
            Save();
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
            if (instance != null)
                return true;
            
            var search = AssetDatabase.FindAssets("t:MegaPintSettings", new[] { "Assets" });

            if (search.Length == 0)
                return false;
            
            instance = AssetDatabase.LoadAssetAtPath<MegaPintSettings>(AssetDatabase.GUIDToAssetPath(search[0]));

            return instance != null;
        }

        public static void Save()
        {
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssetIfDirty(instance);
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