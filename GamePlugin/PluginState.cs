using System;
using System.Collections.Generic;
using System.Linq;
using BigBl4ckW0lf.Model;

namespace BigBl4ckW0lf.GamePlugin
{
    public class PluginState
    {
        public int LastRecordedLap = 0;
        public int PlayerLastPitLap = 0;
        public bool IsInPit = false;

        public DateTime PlayerLastPitTime = DateTime.Now;
        public readonly List<LapData> LastLaps = new List<LapData>();


        public readonly Dictionary<string, IProperty> Properties = new List<IProperty>
        {
            new Property<DateTime>(PropertyNames.LastDateTimeInPit, DateTime.Now),
            new Property<int>(PropertyNames.LastLapInPit, 0),
            new Property<string>(PropertyNames.DebugGameName, ""),
            new Property<string>(PropertyNames.DebugPitState, ""),
            new Property<string>(PropertyNames.DebugPitStateNum, ""),
            new Property<float>(PropertyNames.AverageFuelPerLap, 0),
            new Property<float>(PropertyNames.EstimatedFuelLapsLeft, 0),
            new Property<float>(PropertyNames.EstimatedFuelTimeLeft, 0),
            new Property<float>(PropertyNames.EstimatedRefuelNeeded, 0)
        }.ToDictionary(p => p.Key, p => p);

        private PluginState()
        {
        }

        private static PluginState _instance = null;

        public static PluginState GetInstance()
        {
            return _instance ?? (_instance = new PluginState());
        }
    }
}
