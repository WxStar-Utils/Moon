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
        Log.Info("Published uptime notification.");
    }

    public static async Task PublishRecordGenSuccess(RecordGenMessage recordGenMessage)
    {
        var dataGenStatusMessage = new MqttApplicationMessageBuilder()
            .WithTopic("monitor/record-gen")
            .WithPayload(JsonSerializer.Serialize(recordGenMessage))
            .Build();
        
        await Client.PublishAsync(dataGenStatusMessage, CancellationToken.None);
        
        Log.Info("Published record gen success code.");
    }
}

public class UptimeStatusMessage
{
    [JsonPropertyName("server_status")] public string ServerStatus { get; set; }
    [JsonPropertyName("process_name")] public string ProcessName { get; set; }
}

public class RecordGenMessage
{
    [JsonPropertyName("time_generated")] public string TimeGenerated { get; set; }
    [JsonPropertyName("process_time")] public long TimeTakenSeconds { get; set; }
    [JsonPropertyName("location_count")] public int LocationCount { get; set; }
    [JsonPropertyName("national_data")] public bool NationalData { get; set; }
}