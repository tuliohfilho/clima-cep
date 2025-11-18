using Clima.Cep.Application.Dtos;
using Clima.Cep.Application.Interfaces.Services;
using Clima.Cep.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ServiceIntegration.OpenMeteoForecast.Interfaces;
using ServiceIntegration.OpenMeteoForecast.Responses;
using ServiceIntegration.OpenMeteoGeocoding.Interfaces;

namespace Clima.Cep.Application.Services;

/// <summary>
/// Implementação do serviço de clima (US03).
/// Consulta clima para CEPs persistidos com cache por 10 minutos.
/// </summary>
public class WeatherService(
    IOpenMeteoWeatherClient weatherClient,
    IOpenMeteoGeocodingClient geocodingClient,
    IZipCodeLookupRepository repository,
    IMemoryCache cache,
    ILogger<WeatherService> logger) : IWeatherService
{
    private readonly IOpenMeteoWeatherClient _weatherClient = weatherClient ?? throw new ArgumentNullException(nameof(weatherClient));
    private readonly IOpenMeteoGeocodingClient _geocodingClient = geocodingClient ?? throw new ArgumentNullException(nameof(geocodingClient));
    private readonly IZipCodeLookupRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ILogger<WeatherService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private const int CacheDurationMinutes = 10;
    private const string CacheKeyPattern = "weather:{key}:{days}";

    public async Task<IEnumerable<WeatherResponseDto>> GetWeatherBySavedZipCodesAsync(IEnumerable<ZipCodeResponseDto> zipCodeResponses, int days = 3)
    {
        if (days < 1 || days > 7)
        {
            _logger.LogWarning("Invalid days parameter: {Days}. Must be between 1 and 7.", days);
            throw new ArgumentException("Days must be between 1 and 7.", nameof(days));
        }

        _logger.LogInformation("Getting weather for saved CEPs. Days: {Days}", days);

        var weatherList = new List<WeatherResponseDto>();

        foreach (var zipCode in zipCodeResponses)
        {
            try
            {
                var (latitude, longitude) = await GetCoordinatesAsync(zipCode);

                if (!latitude.HasValue || !longitude.HasValue)
                {
                    _logger.LogWarning("Could not determine coordinates for CEP: {ZipCode}", zipCode.ZipCode);
                    continue;
                }

                var cacheKey = GenerateCacheKey(latitude.Value, longitude.Value, days);
                
                if (_cache.TryGetValue(cacheKey, out WeatherResponseDto cachedWeather))
                {
                    _logger.LogInformation("Weather found in cache for CEP: {ZipCode}", zipCode.ZipCode);
                    cachedWeather.SourceZipCodeId = zipCode.Id;
                    weatherList.Add(cachedWeather);
                    continue;
                }

                _logger.LogInformation("Fetching weather from API for CEP: {ZipCode} (Lat: {Lat}, Lon: {Lon})",
                    zipCode.ZipCode, latitude, longitude);

                var forecast = await _weatherClient.GetForecastAsync(latitude.Value, longitude.Value, days);

                if (forecast == null)
                {
                    _logger.LogWarning("Failed to get weather for CEP: {ZipCode}", zipCode.ZipCode);
                    continue;
                }

                var weatherDto = MapToWeatherResponseDto(zipCode, latitude.Value, longitude.Value, forecast);

                // Armazenar em cache
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheDurationMinutes)
                };
                _cache.Set(cacheKey, weatherDto, cacheOptions);

                weatherList.Add(weatherDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching weather for CEP: {ZipCode}", zipCode.ZipCode);
            }
        }

        if (weatherList.Count == 0)
        {
            _logger.LogWarning("No weather data could be retrieved for any saved CEP.");
            throw new InvalidOperationException("No weather data could be retrieved for any saved CEP.");
        }

        return weatherList;
    }

    private async Task<(decimal? Latitude, decimal? Longitude)> GetCoordinatesAsync(ZipCodeResponseDto zipCode)
    {
        if (zipCode.Location != null)
        {
            _logger.LogInformation("Using stored coordinates for CEP: {ZipCode}", zipCode.ZipCode);
            return (zipCode.Location.Lat, zipCode.Location.Lon);
        }

        _logger.LogInformation("Geocoding CEP: {ZipCode} (City: {City}, State: {State})",
            zipCode.ZipCode, zipCode.City, zipCode.State);

        return await _geocodingClient.SearchAsync(zipCode.City, zipCode.State);
    }

    private static string GenerateCacheKey(decimal latitude, decimal longitude, int days)
    {
        return CacheKeyPattern
            .Replace("{key}", $"{latitude:F4}:{longitude:F4}")
            .Replace("{days}", days.ToString());
    }

    private static WeatherResponseDto MapToWeatherResponseDto(ZipCodeResponseDto zipCode, decimal latitude, decimal longitude,  WeatherForecast forecast)
    {
        return new WeatherResponseDto
        {
            SourceZipCodeId = zipCode.Id,
            Location = new WeatherResponseDto.LocationInfoDto
            {
                Lat = latitude,
                Lon = longitude,
                City = zipCode.City,
                State = zipCode.State
            },
            Current = new WeatherResponseDto.CurrentWeatherDto
            {
                TemperatureC = forecast.TemperatureC,
                ApparentTemperatureC = forecast.ApparentTemperatureC,
                Humidity = forecast.Humidity,
                ObservedAt = forecast.ObservedAt
            },
            Daily = forecast.DailyForecasts?.Select(d => new WeatherResponseDto.DailyWeatherDto
            {
                Date = d.Date,
                TempMinC = d.TempMinC,
                TempMaxC = d.TempMaxC
            }) ?? [],
            Provider = "open-meteo"
        };
    }
}