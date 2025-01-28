using System.Xml.Serialization;

namespace MistWX_i2Me;

/// <summary>
/// Class for building the config.xml file used to configure the software
/// </summary>
[XmlRoot("Config")]
public class Config
{
    // Config Elements \\
    
    [XmlElement] public string TwcApiKey { get; set; } = "REPLACE_ME";
    [XmlElement] public string WxNetToken { get; set; } = "REPLACE_ME";
    [XmlElement] public string LogLevel { get; set; } = "info";
    [XmlElement] public bool GetAlerts { get; set; } = true;
    
    [XmlElement] public bool UseNationalLocations { get; set; } = false;
    [XmlElement] public int CheckAlertTimeSeconds { get; set; } = 600;      // Defaults to 10 minutes

    [XmlElement("MQTTConfig")] public MqttConfig Mqtt { get; set; } = new MqttConfig();

    [XmlElement("RadarConfig")] public RadarConfig RadarConfiguration { get; set; } = new RadarConfig();
    [XmlElement("DataConfig")] public DataEndpointConfig DataConfig { get; set; } = new DataEndpointConfig();

    // Actual configuration setup \\
    
    public static Config config = new Config();

    
    /// <summary>
    /// Loads values from the configuration file into the software
    /// </summary>
    /// <returns>Config object</returns>
    public static Config Load()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "config.xml");
        XmlSerializer serializer = new XmlSerializer(typeof(Config));
        XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
        namespaces.Add("", "");
        
        // Create the temp directory if none exists
        if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "temp")))
        {
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "temp"));
        }
        
        // Create a base config if none exists
        if (!File.Exists(path))
        {
            config = new Config();
            serializer.Serialize(File.Create(path), config, namespaces);

            return config;
        }

        using (FileStream stream = new FileStream(path, FileMode.Open))
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