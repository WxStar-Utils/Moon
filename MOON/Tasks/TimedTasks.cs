using Microsoft.Extensions.Caching.Memory;
using Moon.API;
using Moon.API.Products;
using Moon.MQTT;
using Moon.RecordGeneration;
using Moon.Schema.ibm;

namespace Moon.Tasks;

public partial class TimedTasks
{
    
    /// <summary>
    /// Checks every 10 minutes for new alerts in the unit's area.
    /// </summary>
    /// <param name="locations">Array of location IDs</param>
    /// <param name="checkInterval">Interval to check for new alerts in seconds</param>
    public static async Task CheckForAlerts(string[] locations, int checkInterval)
    {
        if (Config.config.UseNationalLocations || !Config.config.GetAlerts)
        {
            Log.Debug("Disabling alert generation.");
            return;
        }
        
        while (true)
        {
            Log.Info("Checking for new alerts..");
            List<GenericResponse<HeadlineResponse>> headlines = await new AlertHeadlinesProduct().Populate(locations);

            if (headlines.Count == 0)
            {
                Log.Info("No new alerts found.");
                await Task.Delay(checkInterval * 1000);
                continue;
            }

            List<GenericResponse<AlertDetailResponse>> alerts = await new AlertDetailsProduct().Populate(headlines);

            string? bulletinRecord = await new AlertBulletin().MakeRecord(alerts);

            Log.Debug(bulletinRecord);
            
            await MqttDistributor.PublishFile(
                bulletinRecord, 
                "storeData(QGROUP=__BERecord__,Feed=BERecord)",
                "i2m/urgent");
            
            Log.Debug("By this point, the record should be sent out.");
            
            await Task.Delay(checkInterval * 1000);
        }
    }

    /// <summary>
    /// Removes expired alerts from the alerts cache
    /// </summary>
    public static async Task ClearExpiredAlerts()
    {
        if (Config.config.UseNationalLocations || !Config.config.GetAlerts)
        {
            return;
        }
        
        while (true)
        {
            var currentTimeLong = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            foreach (var key in Globals.AlertDetailKeys)
            {
                if (!Globals.AlertsCache.TryGetValue(key, out int expireTime))
                {
                    continue;
                }

                if (currentTimeLong < expireTime)
                {
                    continue;
                }
            
                Globals.AlertsCache.Remove(key);
                Globals.AlertDetailKeys.Remove(key);
                Log.Debug($"Removed expired alert {key} from the alerts cache.");
                await Task.Delay(90 * 1000);
            }
        }

    }
    
    /// <summary>
    /// Generates all data for units regardless of type. 
    /// </summary>
    /// <param name="locations"></param>
    public static async Task HourlyRecordGenTask(string[] locations)
    {
        string mqttTopic;
        var watch = System.Diagnostics.Stopwatch.StartNew();

        switch (Config.config.UseNationalLocations)
        {
            case true:
                mqttTopic = "i2m/national";
                break;
            case false:
                mqttTopic = "i2m/global";
                break;
        }

        while (true)
        {
            var currentTime = DateTime.Now;
            
            watch.Restart();
            
            // Don't generate if we're not at the start of the hour 
            if (currentTime.Minute != 0 && !Globals.FreshStart)
            {
                await Task.Delay(30 * 1000);
                continue;
            }
            
            Log.Info("Running hourly record collection");

            await Publisher.PublishI2MRecords(locations, mqttTopic);
            
            if (Globals.FreshStart)
            {
                Globals.FreshStart = false;
            }
            
            watch.Stop();
            
            Log.Info($"Generated hourly records in {watch.ElapsedMilliseconds} ms.");
            
            await Task.Delay(120 * 1000);
        }
    }
}