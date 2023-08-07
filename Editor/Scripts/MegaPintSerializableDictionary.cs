using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor.Scripts
{
    [Serializable]
    public class MegaPintSerializableDictionary<TKey, TValue>
    {
        [Serializable]
        public struct KeyValueEntry
        {
            public TKey key;
            public TValue value;
        }

        public List<KeyValueEntry> entries;
        public int Count => entries.Count;

        public List<TKey> Keys => entries.Select(keyValueEntry => keyValueEntry.key).ToList();

        public List<TValue> Values => entries.Select(keyValueEntry => keyValueEntry.value).ToList();

        public void SetValue(TKey key, TValue value)
        {
            entries ??= new List<KeyValueEntry>();

            var entry = new KeyValueEntry
            {
                key = key,
                value = value
            };

            for (var i = 0; i < entries.Count; i++)
            {
                var keyValueEntry = entries[i];

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

            entries ??= new List<KeyValueEntry>();

            foreach (var keyValueEntry in entries.Where(keyValueEntry => keyValueEntry.key.Equals(key)))
            {
                value = keyValueEntry.value;
                return true;
            }

            return false;
        }
    }
}