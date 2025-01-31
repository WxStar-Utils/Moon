using System.Text.Json;

namespace Moon.API;

public class Mist
{
    private static readonly HttpClient Client = new HttpClient();
    
    /// <summary>
    /// Retrieves the current unit locations for the 
    /// </summary>
    /// <returns></returns>
    public static async Task<string[]> GetActiveLocations()
    {
        var response = await Client.GetAsync(
            "http://wxnet.bli.overcastsystems.org/internal/moon/unit/locations?unit_type=i2");

        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        
        MistLocationsResponse locationsResponse = JsonSerializer.
            Deserialize<MistLocationsResponse>(responseBody);

        return locationsResponse.locations.ToArray();
    }
}

public class MistLocationsResponse
{
    public List<string> locations { get; set; }
}