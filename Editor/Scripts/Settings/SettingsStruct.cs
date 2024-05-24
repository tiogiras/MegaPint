#if UNITY_EDITOR
using System;

namespace MegaPint.Editor.Scripts.Settings
{

[Serializable]
internal class SettingsStruct
{
    public SerializableDictionary <string, string> stringValues;
    public SerializableDictionary <string, float> floatValues;
    public SerializableDictionary <string, bool> boolValues;
    public SerializableDictionary <string, int> intValues;

    public SettingsStruct()
    {
        stringValues = new SerializableDictionary <string, string>();
        floatValues = new SerializableDictionary <string, float>();
        boolValues = new SerializableDictionary <string, bool>();
        intValues = new SerializableDictionary <string, int>();
    }

    #region Public Methods

    public string GetValue(string key, string defaultValue)
    {
        stringValues ??= new SerializableDictionary <string, string>();

        if (stringValues.TryGetValue(key, out var value))
            return value;

        SetValue(key, defaultValue);

        return defaultValue;
    }

    public float GetValue(string key, float defaultValue)
    {
        floatValues ??= new SerializableDictionary <string, float>();

        if (floatValues.TryGetValue(key, out var value))
            return value;

        SetValue(key, defaultValue);

        return defaultValue;
    }

    public bool GetValue(string key, bool defaultValue)
    {
        boolValues ??= new SerializableDictionary <string, bool>();

        if (boolValues.TryGetValue(key, out var value))
            return value;

        SetValue(key, defaultValue);

        return defaultValue;
    }

    public int GetValue(string key, int defaultValue)
    {
        intValues ??= new SerializableDictionary <string, int>();

        if (intValues.TryGetValue(key, out var value))
            return value;

        SetValue(key, defaultValue);

        return defaultValue;
    }

    public void RemoveBoolEntry(string key)
    {
        boolValues ??= new SerializableDictionary <string, bool>();
        boolValues.RemoveEntry(key);

        Settings.Save();
    }

    public void RemoveFloatEntry(string key)
    {
        floatValues ??= new SerializableDictionary <string, float>();
        floatValues.RemoveEntry(key);

        Settings.Save();
    }

    public void RemoveIntEntry(string key)
    {
        intValues ??= new SerializableDictionary <string, int>();
        intValues.RemoveEntry(key);

        Settings.Save();
    }

    public void RemoveStringEntry(string key)
    {
        stringValues ??= new SerializableDictionary <string, string>();
        stringValues.RemoveEntry(key);

        Settings.Save();
    }

    public void SetValue(string key, string value)
    {
        stringValues ??= new SerializableDictionary <string, string>();
        stringValues.SetValue(key, value);

        Settings.Save();
    }

    public void SetValue(string key, float value)
    {
        floatValues ??= new SerializableDictionary <string, float>();
        floatValues.SetValue(key, value);

        Settings.Save();
    }

    public void SetValue(string key, bool value)
    {
        boolValues ??= new SerializableDictionary <string, bool>();
        boolValues.SetValue(key, value);

        Settings.Save();
    }

    public void SetValue(string key, int value)
    {
        intValues ??= new SerializableDictionary <string, int>();
        intValues.SetValue(key, value);

        Settings.Save();
    }

    #endregion
}

}
#endif
