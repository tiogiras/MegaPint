#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Editor.Scripts.Settings
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SettingsData", order = 1)]
    public class MegaPintSettings : ScriptableObject
    {
        [SerializeField] private List<MegaPintSettingsBase> settings;

        private void AddSetting(MegaPintSettingsBase setting) => settings.Add(setting);

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

        public static MegaPintSettings Get() => Resources.Load<MegaPintSettings>("MegaPintSettings");
    }
}
#endif