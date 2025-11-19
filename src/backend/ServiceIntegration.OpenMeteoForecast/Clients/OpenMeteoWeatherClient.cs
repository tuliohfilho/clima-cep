using Microsoft.Extensions.Logging;
using ServiceIntegration.OpenMeteoForecast.Interfaces;
using ServiceIntegration.OpenMeteoForecast.Responses;
using System.Globalization;
using System.Text.Json;

namespace ServiceIntegration.OpenMeteoForecast.Clients;

/// <summary>
/// Cliente HTTP para Open-Meteo Forecast API.
/// Consulta: https://api.open-meteo.com/v1/forecast
/// </summary>
public class OpenMeteoWeatherClient(HttpClient httpClient, ILogger<OpenMeteoWeatherClient> logger) : IOpenMeteoWeatherClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<OpenMeteoWeatherClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<WeatherForecast> GetForecastAsync(decimal latitude, decimal longitude, int days)
    {
        if (days < 1 || days > 7)
        {
            throw new ArgumentException("Days must be between 1 and 7", nameof(days));
        }

        try
        {
            _logger.LogInformation("Calling Open-Meteo Forecast for Lat: {Lat}, Lon: {Lon}, Days: {Days}", latitude, longitude, days);

            var url = $"/v1/forecast?" +
                $"latitude={latitude.ToString(CultureInfo.InvariantCulture)}&" +
                $"longitude={longitude.ToString(CultureInfo.InvariantCulture)}&" +
                $"current_weather=true&" +
                $"hourly=apparent_temperature,relative_humidity_2m&" +
                $"daily=temperature_2m_max,temperature_2m_min&" +
                $"timeformat=iso8601&" +
                $"timezone=UTC";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<OpenMeteoForecastResponse>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (data?.CurrentWeather == null)
            {
                _logger.LogWarning("Invalid response from Open-Meteo for Lat: {Lat}, Lon: {Lon}", latitude, longitude);
                return null;
            }

            _logger.LogInformation("Successfully retrieved forecast from Open-Meteo for Lat: {Lat}, Lon: {Lon}", latitude, longitude);

            var daily = new List<DailyForecast>();

            if (data.Daily?.Dates != null && data.Daily.TempMax != null && data.Daily.TempMin != null)
            {
                for (int i = 0; i < Math.Min(days, data.Daily.Dates.Count); i++)
                {
                    daily.Add(new DailyForecast
                    {
                        Date = data.Daily.Dates[i],
                        TempMaxC = data.Daily.TempMax[i],
                        TempMinC = data.Daily.TempMin[i]
                    });
                }
            }

            return new WeatherForecast
            {
                TemperatureC = data.CurrentWeather.Temperature,
                ApparentTemperatureC = data.Hourly.ApparentTemperature.Last(),
                Humidity = data.Hourly.RelativeHumidity2m.Last(),
                ObservedAt = DateTime.UtcNow,
                DailyForecasts = daily
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Open-Meteo request failed for Lat: {Lat}, Lon: {Lon}", latitude, longitude);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while calling Open-Meteo for Lat: {Lat}, Lon: {Lon}", latitude, longitude);
            throw;
        }
    }
}
