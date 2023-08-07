using System;
using System.Collections.Generic;

namespace Editor.Scripts.Base
{
    [Serializable]
    public class MegaPintSettingsBase
    {
        public string name;
        
        
        
        public MegaPintSerializableDictionary<string, object> ObjectValues;
        public Dictionary<string, string> StringValues;
        public Dictionary<string, float> FloatValues;
        public Dictionary<string, bool> BoolValues;
        public Dictionary<string, int> IntValues;

        public MegaPintSettingsBase(string name)
        {
            this.name = name;
            ObjectValues = new Dictionary<string, object>();
            StringValues = new Dictionary<string, string>();
            FloatValues = new Dictionary<string, float>();
            BoolValues = new Dictionary<string, bool>();
            IntValues = new Dictionary<string, int>();
        }

        public object GetValue(string key, object defaultValue)
        {
            ObjectValues ??= new Dictionary<string, object>();
            if (ObjectValues.TryGetValue(key, out var value))
                return value;
            
            SetValue(key, defaultValue);
            return defaultValue;
        }

        public string GetValue(string key, string defaultValue)
        {
            StringValues ??= new Dictionary<string, string>();
            if (StringValues.TryGetValue(key, out var value))
                return value;
            
            SetValue(key, defaultValue);
            return defaultValue;
        }
        
        public float GetValue(string key, float defaultValue)
        {
            FloatValues ??= new Dictionary<string, float>();
            if (FloatValues.TryGetValue(key, out var value))
                return value;
            
            SetValue(key, defaultValue);
            return defaultValue;
        }
        
        public bool GetValue(string key, bool defaultValue)
        {
            BoolValues ??= new Dictionary<string, bool>();
            if (BoolValues.TryGetValue(key, out var value))
                return value;
            
            SetValue(key, defaultValue);
            return defaultValue;
        }
        
        public int GetValue(string key, int defaultValue)
        {
            IntValues ??= new Dictionary<string, int>();
            if (IntValues.TryGetValue(key, out var value))
                return value;
            
            SetValue(key, defaultValue);
            return defaultValue;
        }
        
        public void SetValue(string key, object value)
        {
            ObjectValues ??= new Dictionary<string, object>();
            ObjectValues[key] = value;
        }
        
        public void SetValue(string key, string value)
        {
            StringValues ??= new Dictionary<string, string>();
            StringValues[key] = value;
        }
        
        public void SetValue(string key, float value)
        {
            FloatValues ??= new Dictionary<string, float>();
            FloatValues[key] = value;
        }
        
        public void SetValue(string key, bool value)
        {
            BoolValues ??= new Dictionary<string, bool>();
            BoolValues[key] = value;
        }
        
        public void SetValue(string key, int value)
        {
            IntValues ??= new Dictionary<string, int>();
            IntValues[key] = value;
        }
    }
}