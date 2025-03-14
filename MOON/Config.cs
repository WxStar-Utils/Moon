using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Moon;

/// <summary>
/// Class for building the config.xml file used to configure the software
/// </summary>
[XmlRoot("Config")]
public class Config
{
    public static string ConfigPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "config.xml");

    // Config Elements \\

    [XmlElement] public TwcApiKeysList TwcApiKeys { get; set; } = new();
    [XmlElement] public string WxNetEndpoint { get; set; } = "https://wxnet.overcastsystems.org";
    [XmlElement] public string WxNetToken { get; set; } = "REPLACE_ME";
    [XmlElement] public string LogLevel { get; set; } = "info";
    [XmlElement] public bool GetAlerts { get; set; } = true;
    
    [XmlElement] public bool UseNationalLocations { get; set; } = false;
    [XmlElement] public int CheckAlertTimeSeconds { get; set; } = 600;      // Defaults to 10 minutes

    [XmlElement("MQTTConfig")] public MqttConfig Mqtt { get; set; } = new();

    [XmlElement("RadarConfig")] public RadarConfig RadarConfiguration { get; set; } = new();
    [XmlElement("DataConfig")] public DataEndpointConfig DataConfig { get; set; } = new();

    // Actual configuration setup \\
    
    public static Config config = new Config();

    
    /// <summary>
    /// Loads values from the configuration file into the software
    /// </summary>
    /// <returns>Config object</returns>
    public static Config Load()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Config));
        XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
        namespaces.Add("", "");
        
        // Create the temp directory if none exists
        if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "temp")))
        {
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "temp"));
        }
        
        // Create a base config if none exists
        if (!File.Exists(ConfigPath))
        {
            config = new Config();
            serializer.Serialize(File.Create(ConfigPath), config, namespaces);

            return config;
        }

        using (FileStream stream = new FileStream(ConfigPath, FileMode.Open))
        {
            var deserializedConfig = serializer.Deserialize(stream);

            if (deserializedConfig != null && deserializedConfig is Config cfg)
            {
                config = cfg;
                return config;
            }

            return new Config();
        }
        
    }

    [XmlRoot("TwcApiKeys")]
    public class TwcApiKeysList
    {
        [XmlElement("Key")]
        public List<string> Keys { get; set; } = new() {};
    }
    
    [XmlRoot("MQTTConfig")]
    public class MqttConfig
    {
        [XmlElement] public string Host { get; set; } = "127.0.0.1";
        [XmlElement] public int Port { get; set; } = 1883;
        [XmlElement] public string Username { get; set; } = "MOON";
        [XmlElement] public string Password { get; set; } = "intellistar";
        [XmlElement] public bool UseWebsockets { get; set; } = false;
        [XmlElement] public bool UseTLS { get; set; } = false;
        [XmlElement] public bool TLSInsecure { get; set; } = false;
        [XmlElement] public int? QOS { get; set; }
    }

    [XmlRoot("WebhookLogger")]
    public class WebhookLogger
    {
        [XmlElement] public bool Enabled { get; set; } = false;
        [XmlElement] public string WebhookUrl { get; set; } = "REPLACE_ME";
        [XmlElement] public string AvatarUrl { get; set; } = "REPLACE_ME";
    }
    
    [XmlRoot("RadarConfig")]
    public class RadarConfig
    {
        [XmlElement] public bool UseRadarServer { get; set; } = false;
        [XmlElement] public string RadarServerUrl { get; set; } = "REPLACE_ME";
    }

    [XmlRoot("DataConfig")]
    public class DataEndpointConfig
    {
        [XmlElement] public bool CurrentConditions { get; set; } = true;
        [XmlElement] public bool DailyForecast { get; set; } = true;
        [XmlElement] public bool HourlyForecast { get; set; } = true;
        [XmlElement] public bool AirQuality { get; set; } = true;
        [XmlElement] public bool AchesAndPains { get; set; } = true;
        [XmlElement] public bool Breathing { get; set; } = true;
        [XmlElement] public bool HeatingAndCooling { get; set; } = true;
        [XmlElement] public bool PollenForecast { get; set; } = true;
    }
}