using Moon.API;

namespace Moon.Tasks;

public partial class TimedTasks
{
    /// <summary>
    /// Grabs data refresh requests from the API every minute.
    /// Also processes the process of distributing data refreshes and cleaning them up from the API.
    /// </summary>
    public static async Task CheckRequests()
    {
        // TODO:
        return;
    }

    /// <summary>
    /// Updates the current lists of applications and cleans up the ones that are not currently online/connected.
    /// </summary>
    public static async Task UpdateLocations()
    {
        if (Config.config.UseNationalLocations)
        {
            return;
        }
        
        while (true)
        {
            Log.Info("Refreshing Mist locations cache.");

            await Mist.GetActiveLocations();

            await Task.Delay(1800 * 1000);    // Checks every 30 minutes by default.
        }
    }
}