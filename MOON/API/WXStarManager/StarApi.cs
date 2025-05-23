using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Moon.MQTT;

namespace Moon.API.WXStarManager;

public class StarApi
{
    private static readonly HttpClient Client = new HttpClient();
    public static string[] Locations = { };

    /// <summary>
    /// Prepares the HTTP client for interactions with the WeatherStar Management API
    /// </summary>
    public static async Task Init()
    {
        bool isApiUp = await StarApiUp();

        if (isApiUp)
        {
            if (Config.config.UseNationalLocations)
            {
                Log.Error("An error occurred while connecting to the WXStarManager API");
                Log.Warning("National data enabled -- ignoring WXStarManager connect failure");
                Log.Warning("Stats for national data will not be available.");
                return;
            }
            
            Log.Error("An error occurred while connecting to the WXStarManager API");
            throw new Exception("Unable to continue initialization.");
        }
        
        await RegisterSystemService();
        await SendUptimeReport();
    }


    /// <summary>
    /// Checks for connectivity with the WeatherStar Management API
    /// </summary>
    /// <returns>Boolean depending on connection result</returns>
    public static async Task<bool> StarApiUp()
    {
        try
        {
            var response = await Client.GetAsync(Config.config.StarApiEndpoint);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (HttpRequestException e)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Retrieves unit-specific locations from the management API
    /// </summary>
    /// <returns></returns>
    public static async Task<string[]> GetActiveLocations(StarModels model)
    {
        string requestUri = "";

        switch (model)
        {
            case StarModels.IntelliStar:
                requestUri = $"{Config.config.StarApiEndpoint}/services/moon/wxstar_locids?star_model=i1";
                break;
            case StarModels.IntelliStar2:
                requestUri = $"{Config.config.StarApiEndpoint}/services/moon/wxstar_locids?star_model=i2";
                break;
            case StarModels.WeatherStarXl:
                requestUri = $"{Config.config.StarApiEndpoint}/services/moon/wxstar_locids?star_model=wsxl";
                break;
        }
        
        var response = await Client.GetAsync(requestUri);

        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        
        WxStarLocationsResponse locationsResponse = JsonSerializer.
            Deserialize<WxStarLocationsResponse>(responseBody);

        Locations = locationsResponse.Locations.ToArray();
        return Locations;
    }

    /// <summary>
    /// Sends the current uptime report to the API
    /// </summary>
    public static async Task SendUptimeReport()
    {
        try
        {
            var uptimeReport = new ServiceUptimeReport();
            var serviceUuid = await File.ReadAllTextAsync("service-uuid");

            var response = await Client.PutAsJsonAsync(
                $"{Config.config.StarApiEndpoint}/services/report_up?service_uuid={serviceUuid}",
                uptimeReport);

            response.EnsureSuccessStatusCode();
            
            Log.Info("Successfully reported uptime to the management API.");
        }
        catch (Exception e)
        {
            Log.Warning("Failed to report uptime to the management API.");
        }
    }
    
    /// <summary>
    /// Registers the current instance of Moon into the management API's system service database.
    /// </summary>
    private static async Task RegisterSystemService()
    {
        // Check to see if this particular instance has already been registered in the past
        if (File.Exists("service-uuid"))
        {
            string serviceUuid = await File.ReadAllTextAsync("service-uuid");

            try
            {
                var checkResponse = await Client.GetAsync(
                    $"{Config.config.StarApiEndpoint}/services/{serviceUuid}");

                checkResponse.EnsureSuccessStatusCode();

                return;
            }
            catch (HttpRequestException e)
            {
                Log.Debug("Potentially old registration, re-registering..");
                File.Delete("service-uuid");
            }
        }

        SystemServiceIn serviceInfo = new SystemServiceIn();

        if (Config.config.UseNationalLocations)
        {
            serviceInfo.Name = "MOON-NATIONAL";
        }
        else
        {
            serviceInfo.Name = "MOON";
        }

        serviceInfo.Host = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
        serviceInfo.Pid = System.Diagnostics.Process.GetCurrentProcess().Id;
        
        var response = await Client.PostAsJsonAsync(
            $"{Config.config.StarApiEndpoint}/services/register",
            serviceInfo);
        
        Log.Info($"Service {serviceInfo.Name} registered with UUID {await response.Content.ReadAsStringAsync()}");

        await File.WriteAllTextAsync("service-uuid", await response.Content.ReadAsStringAsync());
    }
}