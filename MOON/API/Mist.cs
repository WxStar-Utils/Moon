using System.Text.Json;

namespace Moon.API;

public class Mist
{
    private static readonly HttpClient Client = new HttpClient();
    public static string[] Locations = { };
    
    /// <summary>
    /// Retrieves the current unit locations for the 
    /// </summary>
    /// <returns></returns>
    public static async Task<string[]> GetActiveLocations()
    {
        var response = await Client.GetAsync(
            "http://wxnet.overcastsystems.org/internal/moon/unit/locations?unit_type=i2");
            // TODO: ^^ This url should be changeable in config.xml!

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