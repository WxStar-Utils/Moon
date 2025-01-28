using Moon.API;
using Moon.Schema.ibm;

namespace Moon.RecordGeneration;

public class HeatingCoolingRecord : I2Record
{
    public async Task<string> MakeRecord(List<GenericResponse<HeatingCoolingResponse>> results)
    {
        Log.Info("Creating Heating & Cooling record.");
        string recordScript = "<Data type=\"HeatingAndCooling\">";

        foreach (var result in results)
        {
            recordScript +=
                $"<HeatingAndCooling id=\"000000000\" locationKey=\"{result.Location.coopId}\" isWxScan=\"0\">" +
                $"{result.RawResponse}<clientKey>{result.Location.coopId}</clientKey></HeatingAndCooling>";
        }

        recordScript += "</Data>";
        
        return recordScript;
    }
}