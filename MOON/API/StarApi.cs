using System.Net;
using WxStarManager;
using WxStarManager.Models;

namespace Moon.API;

public static class StarApi
{
    private static Api _api;
    public static string[] Locations = { };


    public static async Task Init()
    {
        _api = new Api(Config.config.StarApiEndpoint);
        bool apiState = await _api.IsApiUp();

        if (!apiState)
        {
            if (Config.config.UseNationalLocations)
            {
                Log.Error("An error occurred while connecting to StarAPI");
                Log.Warning("National data enabled -- Ignoring connection failure.");
                Log.Warning("Stats for national data will not be available.");
                return;
            }
            
            Log.Error("An error occurred while connecting to StarAPI.");
            throw new Exception("Unable to continue initialization.");
        }

    }

    public static async Task<string[]> GetActiveLocations(WxStarModel starModel)
    {
        var locations = await _api.GetMoonLocations(starModel);
        Locations = locations.ToArray();

        return Locations;
    }
}