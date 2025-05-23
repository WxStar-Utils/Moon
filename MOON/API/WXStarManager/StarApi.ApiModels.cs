using System.Text.Json.Serialization;

namespace Moon.API.WXStarManager;

public class WxStarLocationsResponse
{
    [JsonPropertyName("locations")] public List<string> Locations { get; set; }
}

public class SystemServiceIn
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("host")] public string Host { get; set; }
    [JsonPropertyName("pid")] public int? Pid { get; set; } = null;
}

public class ServiceUptimeReport
{
    [JsonPropertyName("online")] public bool Online { get; set; } = true;
    [JsonPropertyName("uptime_timestamp")] public DateTime UptimeTimestamp { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("json_stats")] public string? JsonStats { get; set; } = null;
}