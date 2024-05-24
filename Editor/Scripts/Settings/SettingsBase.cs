#if UNITY_EDITOR
using System;

namespace MegaPint.Editor.Scripts.Settings
{

[Serializable]
internal class SettingsBase
{
    public string name;

    public SerializableDictionary <string, SettingsStruct> structValues;
    public SerializableDictionary <string, string> stringValues;
    public SerializableDictionary <string, float> floatValues;
    public SerializableDictionary <string, bool> boolValues;
    public SerializableDictionary <string, int> intValues;

    public SettingsBase(string name)
    {
        this.name = name;
        structValues = new SerializableDictionary <string, SettingsStruct>();
        stringValues = new SerializableDictionary <string, string>();
        floatValues = new SerializableDictionary <string, float>();
        boolValues = new SerializableDictionary <string, bool>();
        intValues = new SerializableDictionary <string, int>();
    }

    #region Struct

    public SettingsStruct GetStruct(string key, SettingsStruct defaultValue)
    {
        if (TryGetStruct(key, out SettingsStruct value))
            return value;

        SetStruct(key, defaultValue);

        return defaultValue;
    }

    private bool TryGetStruct(string key, out SettingsStruct structValue)
    {
        structValues ??= new SerializableDictionary <string, SettingsStruct>();

        return structValues.TryGetValue(key, out structValue);
    }

    public void SetStruct(string key, SettingsStruct value)
    {
        structValues ??= new SerializableDictionary <string, SettingsStruct>();
        structValues.SetValue(key, value);

        Settings.Save();
    }

    public void SetStructValue(string structKey, string valueKey, string value)
    {
        SettingsStruct structValue = GetStruct(structKey, new SettingsStruct());
        structValue.SetValue(valueKey, value);
    }

    public string GetStructValue(string structKey, string valueKey, string defaultValue)
    {
        if (TryGetStruct(structKey, out SettingsStruct structValue))
            return structValue.GetValue(valueKey, defaultValue);

        SetStructValue(structKey, valueKey, defaultValue);

        return defaultValue;
    }

    public void SetStructValue(string structKey, string valueKey, float value)
    {
        SettingsStruct structValue = GetStruct(structKey, new SettingsStruct());
        structValue.SetValue(valueKey, value);
    }

    public float GetStructValue(string structKey, string valueKey, float defaultValue)
    {
        if (TryGetStruct(structKey, out SettingsStruct structValue))
            return structValue.GetValue(valueKey, defaultValue);

        SetStructValue(structKey, valueKey, defaultValue);

        return defaultValue;
    }

    public void SetStructValue(string structKey, string valueKey, bool value)
    {
        SettingsStruct structValue = GetStruct(structKey, new SettingsStruct());
        structValue.SetValue(valueKey, value);
    }

    public bool GetStructValue(string structKey, string valueKey, bool defaultValue)
    {
        if (TryGetStruct(structKey, out SettingsStruct structValue))
            return structValue.GetValue(valueKey, defaultValue);

        SetStructValue(structKey, valueKey, defaultValue);

        return defaultValue;
    }

    public void SetStructValue(string structKey, string valueKey, int value)
    {
        SettingsStruct structValue = GetStruct(structKey, new SettingsStruct());
        structValue.SetValue(valueKey, value);
    }

    public int GetStructValue(string structKey, string valueKey, int defaultValue)
    {
        if (TryGetStruct(structKey, out SettingsStruct structValue))
            return structValue.GetValue(valueKey, defaultValue);

        SetStructValue(structKey, valueKey, defaultValue);

        return defaultValue;
    }

    #endregion

    #region Simple Values

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

    public void SetValue(string key, string value, bool suppressSaving = false)
    {
        stringValues ??= new SerializableDictionary <string, string>();
        stringValues.SetValue(key, value);

        if (!suppressSaving)
            Settings.Save();
    }

    public void SetValue(string key, float value, bool suppressSaving = false)
    {
        floatValues ??= new SerializableDictionary <string, float>();
        floatValues.SetValue(key, value);

        if (!suppressSaving)
            Settings.Save();
    }

    public void SetValue(string key, bool value, bool suppressSaving = false)
    {
        boolValues ??= new SerializableDictionary <string, bool>();
        boolValues.SetValue(key, value);

        if (!suppressSaving)
            Settings.Save();
    }

    public void SetValue(string key, int value, bool suppressSaving = false)
    {
        intValues ??= new SerializableDictionary <string, int>();
        intValues.SetValue(key, value);

        if (!suppressSaving)
            Settings.Save();
    }

    #endregion
}

}
#endif
