using Microsoft.Extensions.Logging;
using ServiceIntegration.OpenMeteoGeocoding.Interfaces;
using ServiceIntegration.OpenMeteoGeocoding.Responses;
using System.Text.Json;

namespace ServiceIntegration.OpenMeteoGeocoding.Clients;

/// <summary>
/// Cliente HTTP para Open-Meteo Geocoding API.
/// Consulta: https://geocoding-api.open-meteo.com/v1/search
/// </summary>
public class OpenMeteoGeocodingClient(HttpClient httpClient, ILogger<OpenMeteoGeocodingClient> logger) : IOpenMeteoGeocodingClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<OpenMeteoGeocodingClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<(decimal? Latitude, decimal? Longitude)> SearchAsync(string city, string state)
    {
        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(state))
        {
            _logger.LogWarning("Invalid city or state: City={City}, State={State}", city, state);
            return (null, null);
        }

        try
        {
            _logger.LogInformation("Calling Open-Meteo Geocoding for City: {City}, State: {State}", city, state);

            var searchQuery = $"{city}";
            var url = $"https://geocoding-api.open-meteo.com/v1/search?" +
                $"name={Uri.EscapeDataString(searchQuery)}&" +
                $"count=100&" +
                $"language=pt&" +
                $"format=json";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<OpenMeteoGeocodingResponse>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (data?.Results == null || data.Results.Count == 0)
            {
                _logger.LogInformation("No results found for City: {City}, State: {State}", city, state);
                return (null, null);
            }

            var first = data.Results[0];
            _logger.LogInformation("Successfully found coordinates for City: {City}, State: {State} -> Lat: {Lat}, Lon: {Lon}", 
                city, state, first.Latitude, first.Longitude);

            return (first.Latitude, first.Longitude);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Open-Meteo Geocoding request failed for City: {City}, State: {State}", city, state);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while calling Open-Meteo Geocoding for City: {City}, State: {State}", city, state);
            throw;
        }
    }
}