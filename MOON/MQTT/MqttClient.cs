using System.Collections.Immutable;
using MQTTnet;
using MQTTnet.Client;

namespace Moon.MQTT;

public class MqttClient
{
    public IMqttClient Client;
    public MqttFactory Factory;
    private string _host = Config.config.Mqtt.Host;
    private int _port = Config.config.Mqtt.Port;
    private string _username = Config.config.Mqtt.Username;
    private string _password = Config.config.Mqtt.Password;

    public async Task Connect()
    {
        var factory = new MqttFactory();
        this.Factory = factory;
        using (var mqttClient = factory.CreateMqttClient())
        {
            var clientOptions = new MqttClientOptionsBuilder()
                .WithClientId("MOON")
                .WithTcpServer(_host, _port)
                .WithCredentials(_username, _password)
                .WithCleanSession()
                .Build();


            await mqttClient.ConnectAsync(clientOptions, CancellationToken.None);
            this.Client = mqttClient;
            
            Log.Info($"Connected to MQTT Broker @ {_host}:{_port}");
        }
    }

    public async Task PublishFile(string fileData, string command, string topic)
    {
        using (var mqttClient = this.Factory.CreateMqttClient())
        {
            var clientOptions = new MqttClientOptionsBuilder()
                .WithClientId("MOON_DATA_WORKER")
                .WithTcpServer(_host, _port)
                .WithCredentials(_username, _password)
                .WithCleanSession()
                .Build();

            await mqttClient.ConnectAsync(clientOptions, CancellationToken.None);

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload("{\"data\": \"" + fileData.Replace("\"", "\'") + "\", \"cmd\": \"" + command + "\"}")
                .Build();
            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
            
            Log.Info($"File published to topic {topic}.");
            Log.Debug($"Command Sent to {topic}: {command}");
            
            await mqttClient.DisconnectAsync();
        }
    }

    public async Task PublishCommand(string command, string topic)
    {
        using (var mqttClient = this.Factory.CreateMqttClient())
        {
            var clientOptions = new MqttClientOptionsBuilder()
                .WithClientId("MOON_CMD_WORKER")
                .WithTcpServer(_host, _port)
                .WithCredentials(_username, _password)
                .WithCleanSession()
                .Build();

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload("{\"cmd\": \"" + command + "\"}")
                .Build();

            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
            
            Log.Info($"Command sent to topic {topic}");
            
            await mqttClient.DisconnectAsync();
        }
    }
}