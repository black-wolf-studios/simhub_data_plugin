using System.Windows.Controls;
using BigBl4ckW0lf.GamePlugin;
using GameReaderCommon;
using SimHub.Plugins;

namespace BigBl4ckW0lf
{
    [PluginDescription("Add special calculation data to simhub")]
    [PluginAuthor("Daniel Weiss")]
    [PluginName("BigBl4ckW0lf Data Plugin")]
    public class BigBl4ckW0lfDataPlugin : IDataPlugin, IOutputPlugin, IWPFSettings
    {
        public PluginSettings Settings { get; set; }
        public PluginManager PluginManager { get; set; }

        /// <summary>
        /// Called at plugin manager stop, close/dispose anything needed here !
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            // Save settings
            this.SaveCommonSettings("BigBl4ckW0lfDataPlugin", Settings);
        }

        /// <summary>
        /// Returns the settings control, return null if no settings control is required
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new PluginSettingsControl(this);
        }

        /// <summary>
        /// Called once after plugins startup
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            var defaultValues = new PluginSettings();

            foreach (var p in PluginState.GetInstance().Properties.Values)
            {
                p.InitProperty(pluginManager, GetType());
            }

            // Load settings
            Settings = this.ReadCommonSettings("BigBl4ckW0lfDataPlugin", () => defaultValues);
        }

        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            BaseGamePlugin.UpdateWithCorrectPlugin(pluginManager, ref data, Settings);
            foreach (var p in PluginState.GetInstance().Properties.Values)
            {
                p.WriteProperty(pluginManager, GetType());
            }
        }
    }
}
