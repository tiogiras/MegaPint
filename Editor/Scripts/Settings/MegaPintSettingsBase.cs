#if UNITY_EDITOR
using System;

namespace Editor.Scripts.Settings
{
    [Serializable]
    public class MegaPintSettingsBase
    {
        public string name;
        
        public MegaPintSerializableDictionary<string, object> objectValues;
        public MegaPintSerializableDictionary<string, string> stringValues;
        public MegaPintSerializableDictionary<string, float> floatValues;
        public MegaPintSerializableDictionary<string, bool> boolValues;
        public MegaPintSerializableDictionary<string, int> intValues;

        public MegaPintSettingsBase(string name)
        {
            this.name = name;
            objectValues = new MegaPintSerializableDictionary<string, object>();
            stringValues = new MegaPintSerializableDictionary<string, string>();
            floatValues = new MegaPintSerializableDictionary<string, float>();
            boolValues = new MegaPintSerializableDictionary<string, bool>();
            intValues = new MegaPintSerializableDictionary<string, int>();
        }

        public object GetValue(string key, object defaultValue)
        {
            objectValues ??= new MegaPintSerializableDictionary<string, object>();
            if (objectValues.TryGetValue(key, out var value))
                return value;

            SetValue(key, defaultValue);
            return defaultValue;
        }

        public string GetValue(string key, string defaultValue)
        {
            stringValues ??= new MegaPintSerializableDictionary<string, string>();
            if (stringValues.TryGetValue(key, out var value))
                return value;

            SetValue(key, defaultValue);
            return defaultValue;
        }
        
        public float GetValue(string key, float defaultValue)
        {
            floatValues ??= new MegaPintSerializableDictionary<string, float>();
            if (floatValues.TryGetValue(key, out var value))
                return value;

            SetValue(key, defaultValue);
            return defaultValue;
        }
        
        public bool GetValue(string key, bool defaultValue)
        {
            boolValues ??= new MegaPintSerializableDictionary<string, bool>();
            if (boolValues.TryGetValue(key, out var value))
                return value;

            SetValue(key, defaultValue);
            return defaultValue;
        }
        
        public int GetValue(string key, int defaultValue)
        {
            intValues ??= new MegaPintSerializableDictionary<string, int>();
            if (intValues.TryGetValue(key, out var value))
                return value;

            SetValue(key, defaultValue);
            return defaultValue;
        }
        
        public void SetValue(string key, object value)
        {
            objectValues ??= new MegaPintSerializableDictionary<string, object>();
            objectValues.SetValue(key, value);
            
            MegaPintSettings.Save();
        }
        
        public void SetValue(string key, string value)
        {
            stringValues ??= new MegaPintSerializableDictionary<string, string>();
            stringValues.SetValue(key, value);
            
            MegaPintSettings.Save();
        }
        
        public void SetValue(string key, float value)
        {
            floatValues ??= new MegaPintSerializableDictionary<string, float>();
            floatValues.SetValue(key, value);
            
            MegaPintSettings.Save();
        }
        
        public void SetValue(string key, bool value)
        {
            boolValues ??= new MegaPintSerializableDictionary<string, bool>();
            boolValues.SetValue(key, value);
            
            MegaPintSettings.Save();
        }
        
        public void SetValue(string key, int value)
        {
            intValues ??= new MegaPintSerializableDictionary<string, int>();
            intValues.SetValue(key, value);
            
            MegaPintSettings.Save();
        }
    }
}
#endif