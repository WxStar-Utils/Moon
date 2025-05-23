using System.Text.Json.Serialization;

namespace Moon.API.WXStarManager;

public class MoonStats
{
    [JsonPropertyName("national")] public bool IsNational { get; set; } = Config.config.UseNationalLocations;
    [JsonPropertyName("last_record_gen")] public DateTime LastRecordGen { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("unit_support")] public UnitSupport UnitSupport { get; set; } = new();
}

public class UnitSupport
{
    // TODO: These need to be updated per the configuration file
    [JsonPropertyName("i1")] public bool IntelliStar { get; set; } = false;
    [JsonPropertyName("i2")] public bool IntelliStar2 { get; set; } = true;
    [JsonPropertyName("xl")] public bool WeatherStarXl { get; set; } = false;
}