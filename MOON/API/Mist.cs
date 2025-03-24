using System.Text.Json;

namespace Moon.API;

public class Mist
{
    private static readonly HttpClient Client = new HttpClient();
    public static string[] Locations = { };

    /// <summary>
    /// Checks connectivity with the configured WXNet API endpoint, and sets the configured API token for use in
    /// requests to the backend.
    /// </summary>
    public static async Task InitWxnetApi()
    {
        try
        {
            var response = await Client.GetAsync(
                $"{Config.config.WxNetEndpoint}/up");

            response.EnsureSuccessStatusCode();
            
            Client.DefaultRequestHeaders.Add("X-WXNet-Key", Config.config.WxNetToken);
            
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
            $"{Config.config.WxNetEndpoint}/moon/get_loc_ids?unit_type=i2");

        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        
        MistLocationsResponse locationsResponse = JsonSerializer.
            Deserialize<MistLocationsResponse>(responseBody);

        Locations = locationsResponse.locations.ToArray();
        return Locations;
    }
}

public class MistLocationsResponse
{
    public List<string> locations { get; set; }
}