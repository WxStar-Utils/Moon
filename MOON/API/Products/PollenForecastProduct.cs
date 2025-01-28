using Moon.Schema.ibm;

namespace Moon.API.Products;

public class PollenForecastProduct : Base
{
    public PollenForecastProduct()
    {
        RecordName = "PollenForecast";
        DataUrl =
            "https://api.weather.com/v2/indices/pollen/daypart/7day?postalCode={zip}&countryCode=US&language=en-US&format=xml&apiKey={apiKey}";
    }

    public async Task<List<GenericResponse<PollenResponse>>> Populate(string[] locations)
    {
        return await GetData<PollenResponse>(locations);
    }
}