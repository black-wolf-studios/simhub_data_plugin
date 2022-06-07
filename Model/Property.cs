using SimHub.Plugins;
using System;

namespace BigBl4ckW0lf.Model
{
    class Property<T> : IProperty
    {
        public Property(string key, T val) {
            Key = key;
            Value = val;
        }
        public string Key { get; set; }
        public T Value { get; set; }

        public V GetValue<V>()
        {
            if (Value is V v)
            {
                return v;
            }
            throw new ArgumentException();
        }

        public void WriteProperty(PluginManager pluginManager, Type pluginType)
        {
            pluginManager.SetPropertyValue(Key, pluginType, Value);
        }

        public void InitProperty(PluginManager pluginManager, Type pluginType)
        {
            pluginManager.AddProperty(Key, pluginType, Value);
        }
    }
}
