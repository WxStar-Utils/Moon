using Moon.Schema.ibm;

namespace Moon.API.Products;

public class AlertHeadlinesProduct : Base
{
    public AlertHeadlinesProduct()
    {
        RecordName = "AlertHeadlines";
        DataUrl =
            "https://api.weather.com/v3/alerts/headlines?areaId={zone}:US&format=json&language=en-US&apiKey={apiKey}";
    }

    public async Task<List<GenericResponse<HeadlineResponse>>> Populate(string[] locations)
    {
        return await GetJsonData<HeadlineResponse>(locations);
    }
}