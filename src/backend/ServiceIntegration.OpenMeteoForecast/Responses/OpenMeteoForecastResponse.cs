using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ServiceIntegration.OpenMeteoForecast.Responses;

public class OpenMeteoForecastResponse
{
    [JsonPropertyName("current_weather")]
    public CurrentWeather CurrentWeather { get; set; }

    public HourlyData Hourly { get; set; }

    [JsonPropertyName("daily")]
    public DailyData Daily { get; set; }
}

public class CurrentWeather
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("relative_humidity_2m")]
    public double RelativeHumidity { get; set; }

    [JsonPropertyName("apparent_temperature")]
    public double ApparentTemperature { get; set; }

    [JsonPropertyName("time")]
    public string Time { get; set; }

    [JsonPropertyName("windspeed")]
    public double WindSpeed { get; set; }

    [JsonPropertyName("winddirection")]
    public double WindDirection { get; set; }

    [JsonPropertyName("weathercode")]
    public int WeatherCode { get; set; }

    [JsonPropertyName("is_day")]
    public int IsDay { get; set; }
}

public class HourlyData
{
    [JsonPropertyName("time")]
    public List<string> Time { get; set; }

    [JsonPropertyName("relative_humidity_2m")]
    public List<double> RelativeHumidity2m { get; set; }

    [JsonPropertyName("temperature_2m")]
    public List<double> Temperature2m { get; set; }

    [JsonPropertyName("apparent_temperature")]
    public List<double> ApparentTemperature { get; set; }
}