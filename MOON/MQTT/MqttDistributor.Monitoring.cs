using System.Text.Json;
using System.Text.Json.Serialization;
using MQTTnet;

namespace Moon.MQTT;

public partial class MqttDistributor
{
    /// <summary>
    /// Publishes the uptime information for the process in question.
    /// </summary>
    public static async Task PublishUptime()
    {
        UptimeStatusMessage statusMessage = new UptimeStatusMessage()
        {
            ServerStatus = "UP",
            ProcessName = CLIENT_ID
        };
        
        var statusApplicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("monitor/uptime")
            .WithPayload(JsonSerializer.Serialize(statusMessage))
            .Build();
        
        await Client.PublishAsync(statusApplicationMessage, CancellationToken.None);
    }    
}

public class UptimeStatusMessage
{
    [JsonPropertyName("server_status")] public string ServerStatus { get; set; }
    [JsonPropertyName("process_name")] public string ProcessName { get; set; }
}