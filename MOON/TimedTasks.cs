using Microsoft.Extensions.Caching.Memory;
using MistWX_i2Me.API;
using MistWX_i2Me.API.Products;
using MistWX_i2Me.Communication;
using MistWX_i2Me.RecordGeneration;
using MistWX_i2Me.Schema.ibm;

namespace MistWX_i2Me;

public class TimedTasks
{
    
    /// <summary>
    /// Checks every 10 minutes for new alerts in the unit's area.
    /// </summary>
    /// <param name="locations">Array of location IDs</param>
    /// <param name="sender">UdpSender, prefer priority port</param>
    public static async Task CheckForAlerts(string[] locations, UdpSender sender, int checkInterval)
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

            if (headlines == null || headlines.Count == 0)
            {
                Log.Info("No new alerts found.");
                await Task.Delay(checkInterval * 1000);
                continue;
            }

            List<GenericResponse<AlertDetailResponse>> alerts = await new AlertDetailsProduct().Populate(headlines);

            string? bulletinRecord = await new AlertBulletin().MakeRecord(alerts);
            
            sender.SendFile(bulletinRecord, "storeData(QGROUP=__BERecord__,Feed=BERecord)");
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
    
    public static async Task RecordGenTask(string[] locations, UdpSender sender, int generationInterval)
    {
        while (true)
        {
            Config.DataEndpointConfig dataConfig = Config.config.DataConfig;
            
            Log.Info("Running hourly record collection");
            
            // TODO: This could probably be a better system, but this is the simplest implementation for this for the
            //       time being.
            
            
            // Implements suggestion #3 in the issue tracker.
            
            if (dataConfig.CurrentConditions)
            {
                List<GenericResponse<CurrentObservationsResponse>> obs =
                    await new CurrentObservationsProduct().Populate(locations);
                string obsRecord = await new CurrentObsRecord().MakeRecord(obs);
                sender.SendFile(obsRecord, "storeData(QGROUP=__CurrentObservations__,Feed=CurrentObservations)");
            }

            if (dataConfig.DailyForecast)
            {
                List<GenericResponse<DailyForecastResponse>> dfs = await new DailyForecastProduct().Populate(locations);
                string dfsRecord = await new DailyForecastRecord().MakeRecord(dfs);
                sender.SendFile(dfsRecord, "storeData(QGROUP=__DailyForecast__,Feed=DailyForecast)");
            }

            if (dataConfig.HourlyForecast)
            {
                List<GenericResponse<HourlyForecastResponse>> hfs = await new HourlyForecastProduct().Populate(locations);
                string hfsRecord = await new HourlyForecastRecord().MakeRecord(hfs);
                sender.SendFile(hfsRecord, "storeData(QGROUP=__HourlyForecast__,Feed=HourlyForecast)");
            }

            if (dataConfig.AirQuality)
            {
                List<GenericResponse<AirQualityResponse>> aiqs = await new AirQualityProduct().Populate(locations);
                string aiqsRecord = await new AirQualityRecord().MakeRecord(aiqs);
                sender.SendFile(aiqsRecord, "storeData(QGROUP=__AirQuality__,Feed=AirQuality)");
            }

            if (dataConfig.PollenForecast)
            {
                List<GenericResponse<PollenResponse>> pfs = await new PollenForecastProduct().Populate(locations);
                string pfsRecord = await new PollenRecord().MakeRecord(pfs);
                sender.SendFile(pfsRecord, "storeData(QGROUP=__PollenForecast__,Feed=PollenForecast)");
            }

            if (dataConfig.HeatingAndCooling)
            {
                List<GenericResponse<HeatingCoolingResponse>> hcs = await new HeatingCoolingProduct().Populate(locations);
                string hcsRecord = await new HeatingCoolingRecord().MakeRecord(hcs);
                sender.SendFile(hcsRecord, "storeData(QGROUP=__HeatingAndCooling__,Feed=HeatingAndCooling)");
            }

            if (dataConfig.AchesAndPains)
            {
                List<GenericResponse<AchesPainResponse>> acps = await new AchesPainProduct().Populate(locations);
                string acpsRecord = await new AchesPainRecord().MakeRecord(acps);
                sender.SendFile(acpsRecord, "storeData(QGROUP=__AchesAndPain__,Feed=AchesAndPain)");
            }

            if (dataConfig.Breathing)
            {
                List<GenericResponse<BreathingResponse>> brs = await new BreathingProduct().Populate(locations);
                string brsRecord = await new BreathingRecord().MakeRecord(brs);
                sender.SendFile(brsRecord, "storeData(QGROUP=__Breathing__,Feed=Breathing)");
            }

            string nextTimestamp = DateTime.Now.AddSeconds(generationInterval).ToString("h:mm tt");
            
            Log.Info($"Next record generation will be at {nextTimestamp}");
            
            await Task.Delay(generationInterval * 1000);
        }
    }
}