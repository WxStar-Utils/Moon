using Moon.Schema.ibm;

namespace Moon.API.Products;

public class AirQualityProduct : Base
{
    public AirQualityProduct()
    {
        RecordName = "AirQuality";
        DataUrl = "https://api.weather.com/v1/location/{zip}:4:US/airquality.xml?language=en-US&apiKey={apiKey}";
    }

    public async Task<List<GenericResponse<AirQualityResponse>>> Populate(string[] locations)
    {
        return await GetData<AirQualityResponse>(locations);
    }
}