using Moon.API;
using Moon.API.Products;
using Moon.MQTT;
using Moon.Schema.ibm;

namespace Moon.RecordGeneration;

/// <summary>
/// Handles the publishing of data to the MQTT broker, as well as individual units.
/// </summary>
public static class Publisher
{
    /// <summary>
    /// Publishes data to the MQTT broker for IntelliStar 2 units
    /// </summary>
    public static async Task PublishI2MRecords(string[] locations, string mqttTopic)
    {
        Config.DataEndpointConfig dataConfig = Config.config.DataConfig;
        
        if (dataConfig.CurrentConditions)
        {
            List<GenericResponse<CurrentObservationsResponse>> obs =
                await new CurrentObservationsProduct().Populate(locations);
            string obsRecord = await new CurrentObsRecord().MakeRecord(obs);
            MqttDistributor.PublishFile(obsRecord,
                "storeData(QGROUP=__CurrentObservations__,Feed=CurrentObservations)",
                mqttTopic);
        }

        if (dataConfig.DailyForecast)
        {
            List<GenericResponse<DailyForecastResponse>> dfs = await new DailyForecastProduct().Populate(locations);
            string dfsRecord = await new DailyForecastRecord().MakeRecord(dfs);
            MqttDistributor.PublishFile(dfsRecord,
                "storeData(QGROUP=DailyForecast,Feed=DailyForecast)",
                mqttTopic);
        }

        if (dataConfig.HourlyForecast)
        {
            List<GenericResponse<HourlyForecastResponse>> hfs = await new HourlyForecastProduct().Populate(locations);
            string hfsRecord = await new HourlyForecastRecord().MakeRecord(hfs);
            MqttDistributor.PublishFile(hfsRecord,
                "storeData(QGROUP=__HourlyForecast__,Feed=HourlyForecast)",
                mqttTopic);
        }

        if (dataConfig.AirQuality)
        {
            List<GenericResponse<AirQualityResponse>> aiqs = await new AirQualityProduct().Populate(locations);
            string aiqsRecord = await new AirQualityRecord().MakeRecord(aiqs);
            MqttDistributor.PublishFile(aiqsRecord,
                "storeData(QGROUP=__AirQuality__,Feed=AirQuality)",
                mqttTopic);
        }

        if (dataConfig.PollenForecast)
        {
            List<GenericResponse<PollenResponse>> pfs = await new PollenForecastProduct().Populate(locations);
            string pfsRecord = await new PollenRecord().MakeRecord(pfs);
            MqttDistributor.PublishFile(pfsRecord,
                "storeData(QGROUP=__PollenForecast__,Feed=PollenForecast)",
                mqttTopic);
        }

        if (dataConfig.HeatingAndCooling)
        {
            List<GenericResponse<HeatingCoolingResponse>> hcs = await new HeatingCoolingProduct().Populate(locations);
            string hcsRecord = await new HeatingCoolingRecord().MakeRecord(hcs);
            MqttDistributor.PublishFile(hcsRecord,
                "storeData(QGROUP=__HeatingAndCooling__,Feed=HeatingAndCooling)",
                mqttTopic);
        }

        if (dataConfig.AchesAndPains)
        {
            List<GenericResponse<AchesPainResponse>> acps = await new AchesPainProduct().Populate(locations);
            string acpsRecord = await new AchesPainRecord().MakeRecord(acps);
            MqttDistributor.PublishFile(acpsRecord,
                "storeData(QGROUP=__AchesAndPains__,Feed=AchesAndPains)",
                mqttTopic);
        }

        if (dataConfig.Breathing)
        {
            List<GenericResponse<BreathingResponse>> brs = await new BreathingProduct().Populate(locations);
            string brsRecord = await new BreathingRecord().MakeRecord(brs);
            MqttDistributor.PublishFile(brsRecord,
                "storeData(QGROUP=__Breathing__,Feed=Breathing)",
                mqttTopic);
        }
    }
}