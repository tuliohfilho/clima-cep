namespace ServiceIntegration.OpenMeteoForecast.Responses;

/// <summary>
/// Modelo normalizado de resposta de clima.
/// </summary>
public class WeatherForecast
{
    public double TemperatureC { get; set; }
    public double Humidity { get; set; }
    public double ApparentTemperatureC { get; set; }
    public DateTime ObservedAt { get; set; }
    public IEnumerable<DailyForecast> DailyForecasts { get; set; }
}