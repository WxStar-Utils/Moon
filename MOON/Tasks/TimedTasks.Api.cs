using Moon.API;
using Moon.RecordGeneration;
using WxStarManager.Models;

namespace Moon.Tasks;

public partial class TimedTasks
{
    /// <summary>
    /// Updates the list of currently active location codes for all units connected to the API.
    /// </summary>
    public static async Task UpdateLocations()
    {
        if (Config.config.UseNationalLocations)
        {
            return;
        }
        
        while (true)
        {
            Log.Info("Refreshing locations cache.");

            await StarApi.GetActiveLocations(WxStarModel.IntelliStar2);
            await SendNewUnitData();
            await Task.Delay(300 * 1000);    // Checks every 5 minutes.
        }
    }

    /// <summary>
    /// Compares current active locations to the existing locations cache and sends out data for newly added location
    /// keys.
    /// </summary>
    private static async Task SendNewUnitData()
    {
        List<string> newLocations = new List<string>();
        
        foreach (string location in StarApi.Locations)
        {
            // Check the current locations cache to see if these are actually new locations
            if (Globals.LocationCache.TryGetValue(location, out var locationData))
            {
                continue;
            }
            
            newLocations.Add(location);
        }
        
        Log.Info($"The following locations have been added to the database:");
        Log.Info(string.Join(",", newLocations));

        await Publisher.PublishI2MRecords(newLocations.ToArray(), "i2m/global");
        
        Log.Info("Data has been sent out for all newly added locations.");
    }
}