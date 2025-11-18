namespace ServiceIntegration.OpenMeteoForecast.Responses;

/// <summary>
/// Modelo de previsão diária.
/// </summary>
public class DailyForecast
{
    public string Date { get; set; }
    public double TempMinC { get; set; }
    public double TempMaxC { get; set; }
}