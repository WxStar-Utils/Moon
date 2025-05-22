using System.Text.Json.Serialization;

namespace Moon.API.WXStarManager;

public class MistLocationsResponse
{
    [JsonPropertyName("locations")] public List<string> Locations { get; set; }
}

public class UptimeNotification
{
    [JsonPropertyName("process_name")] public string ProcessName { get; set; }
    [JsonPropertyName("current_status")] public string CurrentStatus { get; set; }
    [JsonPropertyName("stats")] public Object? Stats { get; set; } = null;
}