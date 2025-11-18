using System.Collections.Generic;

namespace ServiceIntegration.OpenMeteoForecast.Responses;

public class DailyData
{
    [System.Text.Json.Serialization.JsonPropertyName("time")]
    public List<string> Dates { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("temperature_2m_max")]
    public List<double> TempMax { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("temperature_2m_min")]
    public List<double> TempMin { get; set; }
}