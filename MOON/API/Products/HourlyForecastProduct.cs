using MistWX_i2Me.Schema.ibm;

namespace MistWX_i2Me.API.Products;

public class HourlyForecastProduct : Base
{
    public HourlyForecastProduct()
    {
        RecordName = "HourlyForecast";
        DataUrl =
            "https://api.weather.com/v1/location/{zip}:4:US/forecast/hourly/48hour.xml?language=en-US&units=e&apiKey={apiKey}";
    }

    public async Task<List<GenericResponse<HourlyForecastResponse>>> Populate(string[] locations)
    {
        return await GetData<HourlyForecastResponse>(locations);
    }
}