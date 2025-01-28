using Moon.API;
using Moon.Schema.ibm;

namespace Moon.RecordGeneration;

public class PollenRecord : I2Record
{
    public async Task<string> MakeRecord(List<GenericResponse<PollenResponse>> results)
    {
        Log.Info("Creating Pollen Forecast record.");
        string recordScript = "<Data type=\"PollenForecast\">";

        foreach (var result in results)
        {
            if (string.IsNullOrEmpty(result.Location.pllnId))
            {
                continue;
            }
            
            recordScript +=
                $"<PollenForecast id=\"000000000\" locationKey=\"{result.Location.pllnId}\" isWxscan=\"0\">" +
                $"{result.RawResponse}<clientKey>{result.Location.pllnId}</clientKey></PollenForecast>";
        }
        
        recordScript += "</Data>";
        
        return recordScript;
    }
}