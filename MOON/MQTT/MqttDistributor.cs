using System.Collections.Immutable;
using MQTTnet;
using MQTTnet.Client;

namespace Moon.MQTT;

public partial class MqttDistributor
{
    public static IMqttClient Client;
    public static MqttFactory Factory;
    private static string HOST = Config.config.Mqtt.Host;
    private static int PORT = Config.config.Mqtt.Port;
    private static string USERNAME = Config.config.Mqtt.Username;
    private static string PASSWORD = Config.config.Mqtt.Password;
    private static string CLIENT_ID = $"MOON_{Guid.NewGuid()}";

    public static async Task Connect()
    {
        var factory = new MqttFactory();
        Factory = factory;
        Client = factory.CreateMqttClient();
        
        var clientOptions = new MqttClientOptionsBuilder()
            .WithClientId(CLIENT_ID)
            .WithTcpServer(HOST, PORT)
            .WithCredentials(USERNAME, PASSWORD)
            .WithCleanSession()
            .Build();


        await Client.ConnectAsync(clientOptions, CancellationToken.None);
        Log.Info($"Connected to MQTT Broker as {CLIENT_ID}");
    }

    public static async Task Disconnect()
    {
        await Client.DisconnectAsync();
    }

    public static async Task PublishFile(string fileData, string command, string topic)
    {
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload("{\"data\": \"" + fileData.Replace("\"", "\'") + "\", \"cmd\": \"" + command + "\"}")
            .Build();
        await Client.PublishAsync(applicationMessage, CancellationToken.None);
            
        Log.Info($"File published to topic {topic}.");
        Log.Debug($"Command Sent to {topic}: {command}");
        
    }

    public static async Task PublishCommand(string command, string topic)
    {
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload("{\"cmd\": \"" + command + "\"}")
            .Build();

        await Client.PublishAsync(applicationMessage, CancellationToken.None);
            
        Log.Info($"Command sent to topic {topic}");
    }
}