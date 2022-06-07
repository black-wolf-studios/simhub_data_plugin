using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace BigBl4ckW0lf
{
    public partial class PluginSettingsControl : UserControl
    {
        public BigBl4ckW0lfDataPlugin Plugin { get; private set; }

        public PluginSettingsControl(BigBl4ckW0lfDataPlugin plugin)
        {
            InitializeComponent();

            this.Plugin = plugin;

            LoadSettings();
            LbWarningBox.Content = "";
        }

        private void LoadSettings()
        {
            InputRefuelExtraLaps.Text = Plugin.Settings.RefuelExtraLaps.ToString();
            InputLapsForAverage.Text = Plugin.Settings.LapCountForAverage.ToString();
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // parse before saving to ensure either no change at all or a change matching the whole settings screen
                var refuelExtraLaps =
                    SafeParseFloat(InputRefuelExtraLaps, "Extra laps for refuel could not be parsed.");
                var lapsForAverage = SafeParseInt(InputLapsForAverage, "Lap count for averages could not be parsed.");

                Plugin.Settings.RefuelExtraLaps = refuelExtraLaps;
                Plugin.Settings.LapCountForAverage = lapsForAverage;

                LbWarningBox.Content = "Applied";
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private float SafeParseFloat(TextBox inputField, string errorMessage)
        {
            try
            {
                return float.Parse(inputField.Text.Trim());
            }
            catch (Exception)
            {
                try
                {
                    return ParseFloatWithPoint(inputField.Text);
                }
                catch (Exception)
                {
                    try
                    {
                        return ParseFloatWithComma(inputField.Text);
                    }
                    catch (Exception ex)
                    {
                        LbWarningBox.Content = errorMessage;
                        throw ex;
                    }
                }
            }
        }

        private float ParseFloatWithPoint(string s)
        {
            var cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            return float.Parse(s.Trim(), cultureInfo);
        }

        private float ParseFloatWithComma(string s)
        {
            var cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInfo.NumberFormat.NumberDecimalSeparator = ",";
            return float.Parse(s.Trim(), cultureInfo);
        }

        private int SafeParseInt(TextBox inputField, string errorMessage)
        {
            try
            {
                return int.Parse(inputField.Text);
            }
            catch (Exception ex)
            {
                LbWarningBox.Content = errorMessage;
                throw ex;
            }
        }
    }
}
