using System.Text.Json;
using Moon.Schema;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;

namespace Moon.MQTT;

public class MqttDistributor
{
    public static IMqttClient Client;
    public static MqttFactory Factory;
    private static string HOST = Config.config.Mqtt.Host;
    private static int PORT = Config.config.Mqtt.Port;
    private static string USERNAME = Config.config.Mqtt.Username;
    private static string PASSWORD = Config.config.Mqtt.Password;
    private static string CLIENT_ID = $"MOON_{Guid.NewGuid()}";
    private static bool _disconnected = false;

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

        Client.DisconnectedAsync += async e =>
        {
            _disconnected = true;
            Log.Warning("Lost connection to the MQTT Broker. Attempting to re-connect...");

            while (_disconnected)
            {
                Log.Debug("DISCONNECTED");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await Client.ConnectAsync(clientOptions, CancellationToken.None);
                    Log.Info("Reconnected to broker successfully.");
                    _disconnected = false;
                }
                catch (MqttCommunicationException)
                {
                    Log.Error("Failed to connect to broker. Will re-attempt in 5 seconds.");
                }
            }
        };


        await Client.ConnectAsync(clientOptions, CancellationToken.None);
        
        Log.Info($"Connected to MQTT Broker as {CLIENT_ID}");
    }

    public static async Task Disconnect()
    {
        await Client.DisconnectAsync();
    }

    public static async Task PublishFile(string fileData, string command, string topic)
    {
        MqttCommand mqttCommand = new MqttCommand()
        {
            Data = fileData,
            Command = command
        };
        
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(JsonSerializer.Serialize(mqttCommand))
            .Build();
        await Client.PublishAsync(applicationMessage, CancellationToken.None);
            
        Log.Info($"File published to topic {topic}.");
        Log.Debug($"Command Sent to {topic}: {command}");
        
    }
}