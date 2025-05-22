using System.Text.Json;
using System.Text.Json.Serialization;
using Moon.MQTT;

namespace Moon.API;

public class StarApi
{
    private static readonly HttpClient Client = new HttpClient();
    public static string[] Locations = { };

    /// <summary>
    /// Checks connectivity with the configured WXNet API endpoint, and sets the configured API token for use in
    /// requests to the backend.
    /// </summary>
    public static async Task Init()
    {
        try
        {
            var response = await Client.GetAsync(
                $"{Config.config.StarApiEndpoint}/up");

            response.EnsureSuccessStatusCode();
            
            Client.DefaultRequestHeaders.Add("X-WXNet-Key", Config.config.StarApiToken);
            
            Log.Info("WXNet API is up!");
        }
        catch (HttpRequestException e)
        {
            Log.Error("The WXNet API is not currently up.");
            Log.Error("Contact system administrator when possible.");
            throw;
        }
    }
    
    /// <summary>
    /// Retrieves the current unit locations for the 
    /// </summary>
    /// <returns></returns>
    public static async Task<string[]> GetActiveLocations()
    {
        var response = await Client.GetAsync(
            $"{Config.config.StarApiEndpoint}/moon/get_loc_ids?unit_type=i2");

        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        
        MistLocationsResponse locationsResponse = JsonSerializer.
            Deserialize<MistLocationsResponse>(responseBody);

        Locations = locationsResponse.Locations.ToArray();
        return Locations;
    }
    
    /// <summary>
    /// Sends a POST request to the /status/update endpoint in the API
    /// </summary>
    public static async Task SendUptimeNotification()
    {
        var uptimeNotification = new UptimeNotification()
        {
            ProcessName = MqttDistributor.Client.Options.ClientId,
            CurrentStatus = "UP"
        };
        var uptimeNotificationJson = JsonSerializer.Serialize(uptimeNotification);
        var buffer = System.Text.Encoding.UTF8.GetBytes(uptimeNotificationJson);
        var byteContent = new ByteArrayContent(buffer);
        
        byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        
        var postRequest = await Client.PostAsync(
            $"{Config.config.StarApiEndpoint}/status/update", byteContent);
        
    }
}

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