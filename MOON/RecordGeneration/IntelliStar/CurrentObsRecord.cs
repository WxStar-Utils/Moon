using Moon.API;
using Moon.Schema.ibm;

namespace Moon.RecordGeneration.IntelliStar;

public class CurrentObsRecord
{
    public static async Task<string> MakeRecord(List<GenericResponse<CurrentObservationsResponse>> results)
    {
        string recordScript = "";

        recordScript += "import twccommon\ntwccommon.Log.info(\"ana is cute <3\")\n";

        foreach (var response in results)
        {
            recordScript += $"twccommon.Log.info('MOON CC OBS FOR {response.Location.primTecci}.')\n";
            recordScript += "b = twc.data()\n";
            recordScript += $"b.skyCondition = {response.ParsedData.Observation.IconExtd}\n";
            recordScript += $"b.temp = {response.ParsedData.Observation.Imperial.Temp}\n";
            recordScript += $"b.humidity = {response.ParsedData.Observation.Imperial.Rh}\n";
            recordScript += $"b.dewpoint = {response.ParsedData.Observation.Imperial.Dewpt}\n";
            recordScript += $"b.altimeter = {response.ParsedData.Observation.Imperial.Altimeter}\n";
            recordScript += $"b.windDirection = {response.ParsedData.Observation.Wdir}\n";
            recordScript += $"b.windSpeed = {response.ParsedData.Observation.Imperial.Wspd}\n";
            recordScript += $"b.windChill = {response.ParsedData.Observation.Imperial.Wc}\n";
            recordScript += $"b.pressureTendency = {response.ParsedData.Observation.PtendCode}\n";

            recordScript +=
                $"wxdata.setData('{response.Location.primTecci}', 'obs', b, {response.ParsedData.Metadata.expire_time_gmt})\n";
        }
        
        Log.Debug(recordScript);
        
        return recordScript;
    }
}