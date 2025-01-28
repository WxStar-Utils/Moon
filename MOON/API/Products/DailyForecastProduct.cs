using Moon.Schema.ibm;

namespace Moon.API.Products;

public class DailyForecastProduct : Base
{
    public DailyForecastProduct()
    {
        RecordName = "DailyForecast";
        DataUrl =
            "https://api.weather.com/v1/location/{zip}:4:US/forecast/daily/7day.xml?language=en-US&units=e&apiKey={apiKey}";
    }

    public async Task<List<GenericResponse<DailyForecastResponse>>> Populate(string[] locations)
    {
        return await GetData<DailyForecastResponse>(locations);
    }
}