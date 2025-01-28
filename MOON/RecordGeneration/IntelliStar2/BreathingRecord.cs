using Moon.API;
using Moon.Schema.ibm;

namespace Moon.RecordGeneration;

public class BreathingRecord : I2Record
{
    public async Task<string> MakeRecord(List<GenericResponse<BreathingResponse>> results)
    {
        string recordScript = "<Data type=\"Breathing\">";

        foreach (var result in results)
        {
            recordScript +=
                $"<Breathing id=\"000000000\" locationKey=\"{result.Location.coopId}\" isWxScan=\"0\">" +
                $"{result.RawResponse}<clientKey>{result.Location.coopId}</clientKey></Breathing>";
        }
        
        recordScript += "</Data>";

        return recordScript;
    }
}