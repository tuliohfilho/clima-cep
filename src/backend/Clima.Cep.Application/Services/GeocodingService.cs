using Clima.Cep.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using ServiceIntegration.OpenMeteoGeocoding.Interfaces;

namespace Clima.Cep.Application.Services;

/// <summary>
/// Implementação do serviço de geocodificação.
/// Converte cidade + estado em coordenadas usando API externa.
/// </summary>
public class GeocodingService(IOpenMeteoGeocodingClient apiClient, ILogger<GeocodingService> logger) : IGeocodingService
{
    private readonly ILogger<GeocodingService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IOpenMeteoGeocodingClient _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

    public async Task<(decimal? Latitude, decimal? Longitude)> GetCoordinatesByCityAndStateAsync(string city, string state)
    {
        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(state))
        {
            _logger.LogWarning("Invalid city or state: City={City}, State={State}", city, state);
            return (null, null);
        }

        try
        {
            _logger.LogInformation("Getting coordinates for City: {City}, State: {State}", city, state);

            var (latitude, longitude) = await _apiClient.SearchAsync(city, state);

            if (latitude.HasValue && longitude.HasValue)
            {
                _logger.LogInformation("Found coordinates for City: {City}, State: {State} -> Lat: {Lat}, Lon: {Lon}",
                    city, state, latitude, longitude);
            }
            else
            {
                _logger.LogWarning("No coordinates found for City: {City}, State: {State}", city, state);
            }

            return (latitude, longitude);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting coordinates for City: {City}, State: {State}", city, state);
            throw;
        }
    }
}