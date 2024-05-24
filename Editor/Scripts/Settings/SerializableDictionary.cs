#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaPint.Editor.Scripts.Settings
{

[Serializable]
internal class SerializableDictionary <TKey, TValue>
{
    [Serializable]
    public struct KeyValueEntry
    {
        public TKey key;
        public TValue value;
    }

    public int Count => entries.Count;

    public List <TKey> Keys => entries.Select(keyValueEntry => keyValueEntry.key).ToList();

    public List <TValue> Values => entries.Select(keyValueEntry => keyValueEntry.value).ToList();

    public List <KeyValueEntry> entries = new();

    #region Public Methods

    public bool HasKey(TKey key)
    {
        return Keys.Contains(key);
    }

    public void RemoveEntry(TKey key)
    {
        entries ??= new List <KeyValueEntry>();

        for (var i = 0; i < entries.Count; i++)
        {
            KeyValueEntry keyValueEntry = entries[i];

            if (!keyValueEntry.key.Equals(key))
                continue;

            entries.RemoveAt(i);

            return;
        }
    }

    public void SetValue(TKey key, TValue value)
    {
        entries ??= new List <KeyValueEntry>();

        var entry = new KeyValueEntry {key = key, value = value};

        for (var i = 0; i < entries.Count; i++)
        {
            KeyValueEntry keyValueEntry = entries[i];

            if (!keyValueEntry.key.Equals(key))
                continue;

            entries[i] = entry;

            return;
        }

        entries.Add(entry);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        value = default;

        entries ??= new List <KeyValueEntry>();

        foreach (KeyValueEntry keyValueEntry in entries.Where(keyValueEntry => keyValueEntry.key.Equals(key)))
        {
            value = keyValueEntry.value;

            return true;
        }

        return false;
    }

    #endregion
}

}
#endif
