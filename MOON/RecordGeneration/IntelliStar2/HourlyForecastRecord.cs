using Moon.API;
using Moon.Schema.ibm;

namespace Moon.RecordGeneration;

public class HourlyForecastRecord : I2Record
{
    public async Task<string> MakeRecord(List<GenericResponse<HourlyForecastResponse>> results)
    {
        Log.Info("Creating Hourly Forecast record..");
        string recordScript = "<Data type=\"HourlyForecast\">";

        foreach (var result in results)
        {
            recordScript +=
                $"<HourlyForecast id=\"000000000\" locationKey=\"{result.Location.coopId}\" isWxScan=\"0\">" +
                $"{result.RawResponse}<clientKey>{result.Location.coopId}</clientKey></HourlyForecast>";
        }

        recordScript += "</Data>";

        return recordScript;
    }
}