using Moon.API;
using Moon.Schema.ibm;

namespace Moon.RecordGeneration;

public class AchesPainRecord : I2Record
{
    public async Task<string> MakeRecord(List<GenericResponse<AchesPainResponse>> results)
    {
        Log.Info("Creating Aches & Pain record.");
        string recordScript = "<Data type=\"AchesAndPains\">";

        foreach (var result in results)
        {
            recordScript += 
                $"<AchesAndPains id=\"000000000\" locationKey=\"{result.Location.coopId}\" isWxScan=\"0\">" +
                $"{result.RawResponse}<clientKey>{result.Location.coopId}</clientKey></AchesAndPains>";
        }
        
        recordScript += "</Data>";
        
        return recordScript;
    }
}