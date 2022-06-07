using SimHub.Plugins;
using System;

namespace BigBl4ckW0lf.Model
{
    public interface IProperty
    {
        string Key { get; set; }

        T GetValue<T>();

        void WriteProperty(PluginManager pluginManager, Type pluginType);
        void InitProperty(PluginManager pluginManager, Type pluginType);
    }
}
