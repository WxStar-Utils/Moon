using MistWX_i2Me.API;
using MistWX_i2Me.Schema.ibm;

namespace MistWX_i2Me.RecordGeneration;

public class AirQualityRecord : I2Record
{
    public async Task<string> MakeRecord(List<GenericResponse<AirQualityResponse>> results)
    {
        Log.Info("Creating Air Quality Record");
        string recordScript = "<Data type=\"AirQuality\">";

        foreach (var result in results)
        {
            if (String.IsNullOrEmpty(result.Location.epaId))
            {
                continue;
            }
            
            recordScript +=
                $"<AirQuality id=\"000000000\" locationKey=\"{result.Location.epaId}\" isWxScan=\"0\">" +
                $"{result.RawResponse}<clientKey>{result.Location.epaId}</clientKey></AirQuality>";
        }
        
        recordScript += "</Data>";

        return recordScript;
    }
}