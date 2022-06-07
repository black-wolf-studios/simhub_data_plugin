using System;
using System.Linq;
using BigBl4ckW0lf.Model;
using GameReaderCommon;
using SimHub.Plugins;

namespace BigBl4ckW0lf.GamePlugin
{
    public abstract class BaseGamePlugin
    {
        private readonly PluginManager _pluginManager;

        protected BaseGamePlugin(PluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        private static void UpdateProperty<T>(string key, T value)
        {
            PluginState.GetInstance().Properties.Remove(key);
            PluginState.GetInstance().Properties.Add(key, new Property<T>(key, value));
        }

        private void UpdateData(ref GameData data, PluginSettings settings)
        {
            var newCurrLap = CurrentLap();

            if (IsPlayerInPit())
            {
                PluginState.GetInstance().PlayerLastPitLap = newCurrLap;
                PluginState.GetInstance().PlayerLastPitTime = DateTime.Now;

                UpdateProperty(PropertyNames.LastLapInPit, PluginState.GetInstance().PlayerLastPitLap);
                UpdateProperty(PropertyNames.LastDateTimeInPit, PluginState.GetInstance().PlayerLastPitTime);
            }

            if (newCurrLap > PluginState.GetInstance().LastRecordedLap)
            {
                PluginState.GetInstance().LastLaps.Add(GenerateLastLapData());
                if (PluginState.GetInstance().LastLaps.Count > settings.LapCountForAverage)
                {
                    PluginState.GetInstance().LastLaps.RemoveAt(0);
                }

                PluginState.GetInstance().LastRecordedLap = newCurrLap;
            }

            CalculateFuelStrategyData(settings);
        }

        private bool IsPlayerInPit()
        {
            return (int)_pluginManager.GetPropertyValue("DataCorePlugin.GameData.IsInPit") > 0;
        }

        private int CurrentLap()
        {
            return (int)_pluginManager.GetPropertyValue("DataCorePlugin.GameData.CurrentLap");
        }

        private LapData GenerateLastLapData()
        {
            return new LapData
            {
                LapTime = (TimeSpan)_pluginManager.GetPropertyValue("DataCorePlugin.GameData.LastLapTime"),
                LapNumber = (int)_pluginManager.GetPropertyValue("DataCorePlugin.GameData.CurrentLap"),
                FuelUsed =
                    (double)_pluginManager.GetPropertyValue("DataCorePlugin.Computed.Fuel_LastLapConsumption"),
            };
        }

        public static void UpdateWithCorrectPlugin(PluginManager pluginManager, ref GameData data,
            PluginSettings settings)
        {
            // select the corresponding plugin if you want to generalize different RawData fields
            switch (data.GameName.ToLowerInvariant())
            {
                case "rfactor2":
                    new DefaultGamePlugin(pluginManager).UpdateData(ref data, settings);
                    break;
                case "automobilista":
                    new DefaultGamePlugin(pluginManager).UpdateData(ref data, settings);
                    break;
                default:
                    UpdateProperty(PropertyNames.DebugGameName,
                        (string)pluginManager.GetPropertyValue("DataCorePlugin.CurrentGame"));
                    break;
            }
        }

        private void CalculateFuelStrategyData(PluginSettings settings)
        {
            var fuelLeft = (double)_pluginManager.GetPropertyValue("DataCorePlugin.GameData.Fuel");
            var avgConsumption = PluginState.GetInstance().LastLaps.Select((data) => data.FuelUsed).Sum() /
                                 PluginState.GetInstance().LastLaps.Count;
            var fuelLapsLeft = fuelLeft / avgConsumption;
            var fuelTimeLeft = fuelLapsLeft *
                               (PluginState.GetInstance().LastLaps.Select((data) => data.LapTime.TotalSeconds).Sum() /
                                PluginState.GetInstance().LastLaps.Count);
            var refuel = CalculateRefuel(_pluginManager, avgConsumption, settings);

            UpdateProperty(PropertyNames.AverageFuelPerLap, avgConsumption);
            UpdateProperty(PropertyNames.EstimatedFuelLapsLeft, fuelLapsLeft);
            UpdateProperty(PropertyNames.EstimatedFuelLapsLeft, fuelTimeLeft);
            UpdateProperty(PropertyNames.EstimatedRefuelNeeded, refuel);
        }

        private static double CalculateRefuel(PluginManager pluginManager, double avgConsumption,
            PluginSettings settings)
        {
            var timeLeft = (TimeSpan)pluginManager.GetPropertyValue("DataCorePlugin.GameData.NewData.SessionTimeLeft");
            var lapsLeft = (int)pluginManager.GetPropertyValue("DataCorePlugin.GameData.NewData.RemainingLaps");
            var avgLapTime = PluginState.GetInstance().LastLaps.Select(data => data.LapTime.TotalSeconds).Sum() /
                             PluginState.GetInstance().LastLaps.Count;
            var remainingLapsAccordingToTime = Convert.ToInt32(Math.Ceiling(timeLeft.TotalSeconds / avgLapTime));

            var calculatedRemainingLaps = -1;

            if (lapsLeft > 1 && remainingLapsAccordingToTime > 1)
            {
                calculatedRemainingLaps = Math.Min(lapsLeft, remainingLapsAccordingToTime);
            }
            else if (remainingLapsAccordingToTime > 1)
            {
                calculatedRemainingLaps = remainingLapsAccordingToTime;
            }
            else
            {
                calculatedRemainingLaps = lapsLeft;
            }

            if (calculatedRemainingLaps < 1)
            {
                return -1;
            }

            return avgConsumption * (calculatedRemainingLaps + settings.RefuelExtraLaps);
        }
    }
}
