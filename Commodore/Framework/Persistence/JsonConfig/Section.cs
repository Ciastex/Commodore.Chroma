using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Commodore.Framework.Persistence.JsonConfig
{
    public class Section : Dictionary<string, object>
    {
        public bool Dirty { get; protected set; }

        public new object this[string key]
        {
            get
            {
                if (!ContainsKey(key))
                    return null;

                return base[key];
            }

            set
            {
                if (!ContainsKey(key))
                    Add(key, value);
                else
                    base[key] = value;

                Dirty = true;
            }
        }

        public T GetOrCreate<T>(string key) where T : new()
        {
            if (!ContainsKey(key))
                this[key] = new T();

            return GetItem<T>(key);
        }

        public T GetOrCreate<T>(string key, T defaultValue)
        {
            if (!ContainsKey(key))
                this[key] = defaultValue;

            return GetItem<T>(key);
        }

        public T GetItem<T>(string key)
        {
            if (!ContainsKey(key))
                throw new KeyNotFoundException($"The key requested doesn't exist in store: '{key}'.");

            try
            {
                object o = null;

                if (this[key] is JsonElement je)
                {
                    o = je.ValueKind switch
                    {
                        JsonValueKind.True => je.GetBoolean(),
                        JsonValueKind.False => je.GetBoolean(),
                        JsonValueKind.Number => je.GetInt32(),
                        JsonValueKind.String => je.GetString(),
                        _ => default(T)
                    };
                }
                
                return (T)o;
            }
            catch (JsonException je)
            {
                throw new JsonSettingsException("Failed to convert a JSON token to the requested type.", key, true, je);
            }
            catch (Exception e)
            {
                throw new JsonSettingsException($".NET type conversion exception has been thrown.", key, false, e);
            }
        }

        public bool ContainsKey<T>(string key)
        {
            try
            {
                GetItem<T>(key);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
