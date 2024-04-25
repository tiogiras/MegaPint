#if UNITY_EDITOR
using System;

namespace Editor.Scripts.Settings
{

[Serializable]
public class MegaPintSettingsBase
{
    public string name;

    public MegaPintSerializableDictionary <string, MegaPintSettingsStruct> structValues;
    public MegaPintSerializableDictionary <string, string> stringValues;
    public MegaPintSerializableDictionary <string, float> floatValues;
    public MegaPintSerializableDictionary <string, bool> boolValues;
    public MegaPintSerializableDictionary <string, int> intValues;

    public MegaPintSettingsBase(string name)
    {
        this.name = name;
        structValues = new MegaPintSerializableDictionary <string, MegaPintSettingsStruct>();
        stringValues = new MegaPintSerializableDictionary <string, string>();
        floatValues = new MegaPintSerializableDictionary <string, float>();
        boolValues = new MegaPintSerializableDictionary <string, bool>();
        intValues = new MegaPintSerializableDictionary <string, int>();
    }

    #region Struct

    public MegaPintSettingsStruct GetStruct(string key, MegaPintSettingsStruct defaultValue)
    {
        if (TryGetStruct(key, out MegaPintSettingsStruct value))
            return value;

        SetStruct(key, defaultValue);

        return defaultValue;
    }

    private bool TryGetStruct(string key, out MegaPintSettingsStruct structValue)
    {
        structValues ??= new MegaPintSerializableDictionary <string, MegaPintSettingsStruct>();

        return structValues.TryGetValue(key, out structValue);
    }

    public void SetStruct(string key, MegaPintSettingsStruct value)
    {
        structValues ??= new MegaPintSerializableDictionary <string, MegaPintSettingsStruct>();
        structValues.SetValue(key, value);

        MegaPintSettings.Save();
    }

    public void SetStructValue(string structKey, string valueKey, string value)
    {
        MegaPintSettingsStruct structValue = GetStruct(structKey, new MegaPintSettingsStruct());
        structValue.SetValue(valueKey, value);
    }

    public string GetStructValue(string structKey, string valueKey, string defaultValue)
    {
        if (TryGetStruct(structKey, out MegaPintSettingsStruct structValue))
            return structValue.GetValue(valueKey, defaultValue);

        SetStructValue(structKey, valueKey, defaultValue);

        return defaultValue;
    }

    public void SetStructValue(string structKey, string valueKey, float value)
    {
        MegaPintSettingsStruct structValue = GetStruct(structKey, new MegaPintSettingsStruct());
        structValue.SetValue(valueKey, value);
    }

    public float GetStructValue(string structKey, string valueKey, float defaultValue)
    {
        if (TryGetStruct(structKey, out MegaPintSettingsStruct structValue))
            return structValue.GetValue(valueKey, defaultValue);

        SetStructValue(structKey, valueKey, defaultValue);

        return defaultValue;
    }

    public void SetStructValue(string structKey, string valueKey, bool value)
    {
        MegaPintSettingsStruct structValue = GetStruct(structKey, new MegaPintSettingsStruct());
        structValue.SetValue(valueKey, value);
    }

    public bool GetStructValue(string structKey, string valueKey, bool defaultValue)
    {
        if (TryGetStruct(structKey, out MegaPintSettingsStruct structValue))
            return structValue.GetValue(valueKey, defaultValue);

        SetStructValue(structKey, valueKey, defaultValue);

        return defaultValue;
    }

    public void SetStructValue(string structKey, string valueKey, int value)
    {
        MegaPintSettingsStruct structValue = GetStruct(structKey, new MegaPintSettingsStruct());
        structValue.SetValue(valueKey, value);
    }

    public int GetStructValue(string structKey, string valueKey, int defaultValue)
    {
        if (TryGetStruct(structKey, out MegaPintSettingsStruct structValue))
            return structValue.GetValue(valueKey, defaultValue);

        SetStructValue(structKey, valueKey, defaultValue);

        return defaultValue;
    }

    #endregion

    #region Simple Values

    public string GetValue(string key, string defaultValue)
    {
        stringValues ??= new MegaPintSerializableDictionary <string, string>();

        if (stringValues.TryGetValue(key, out var value))
            return value;

        SetValue(key, defaultValue);

        return defaultValue;
    }

    public float GetValue(string key, float defaultValue)
    {
        floatValues ??= new MegaPintSerializableDictionary <string, float>();

        if (floatValues.TryGetValue(key, out var value))
            return value;

        SetValue(key, defaultValue);

        return defaultValue;
    }

    public bool GetValue(string key, bool defaultValue)
    {
        boolValues ??= new MegaPintSerializableDictionary <string, bool>();

        if (boolValues.TryGetValue(key, out var value))
            return value;

        SetValue(key, defaultValue);

        return defaultValue;
    }

    public int GetValue(string key, int defaultValue)
    {
        intValues ??= new MegaPintSerializableDictionary <string, int>();

        if (intValues.TryGetValue(key, out var value))
            return value;

        SetValue(key, defaultValue);

        return defaultValue;
    }

    public void SetValue(string key, string value, bool suppressSaving = false)
    {
        stringValues ??= new MegaPintSerializableDictionary <string, string>();
        stringValues.SetValue(key, value);

        if (!suppressSaving)
            MegaPintSettings.Save();
    }

    public void SetValue(string key, float value, bool suppressSaving = false)
    {
        floatValues ??= new MegaPintSerializableDictionary <string, float>();
        floatValues.SetValue(key, value);
        
        if (!suppressSaving)
            MegaPintSettings.Save();
    }

    public void SetValue(string key, bool value, bool suppressSaving = false)
    {
        boolValues ??= new MegaPintSerializableDictionary <string, bool>();
        boolValues.SetValue(key, value);

        if (!suppressSaving)
            MegaPintSettings.Save();
    }

    public void SetValue(string key, int value, bool suppressSaving = false)
    {
        intValues ??= new MegaPintSerializableDictionary <string, int>();
        intValues.SetValue(key, value);

        if (!suppressSaving)
            MegaPintSettings.Save();
    }

    #endregion
}

}
#endif
