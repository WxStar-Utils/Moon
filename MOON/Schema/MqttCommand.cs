using System.Text.Json.Serialization;

namespace Moon.Schema;

public class MqttCommand
{
    [JsonPropertyName("data")]
    public string? Data { get; set; }
    
    [JsonPropertyName("cmd")]
    public string Command { get; set; }
}