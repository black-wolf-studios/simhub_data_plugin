namespace BigBl4ckW0lf
{
    /// <summary>
    /// Settings class, make sure it can be correctly serialized using JSON.net
    /// </summary>
    public class PluginSettings
    {
        public float RefuelExtraLaps { get; set; }
        public int LapCountForAverage { get; set; }
    }
}
