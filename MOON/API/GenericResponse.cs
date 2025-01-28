using Moon.Schema.System;

namespace Moon.API;

public class GenericResponse<T>
{
    public LFRecordLocation Location { get; set; }
    public T ParsedData { get; set; }
    public string RawResponse { get; set; }


    public GenericResponse(LFRecordLocation location, string rawResponse, T parsedData)
    {
        this.Location = location;
        this.RawResponse = rawResponse;
        this.ParsedData = parsedData;
    }
}