using Clima.Cep.Application.Dtos;
using Clima.Cep.Application.Interfaces.Services;
using Clima.Cep.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using ServiceIntegration.BrasilAPICEP.Interfaces;
using ServiceIntegration.BrasilAPICEP.Responses;
using ServiceIntegration.ViaCEP.Interfaces;
using ServiceIntegration.ViaCEP.Responses;

namespace Clima.Cep.Application.Services;

public class ZipCodeService(
    IViaCepClient viaCepClient,
    IBrasilApiCepClient brasilApiClient,
    ILogger<ZipCodeService> logger) : IZipCodeService
{
    private readonly ILogger<ZipCodeService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IViaCepClient _viaCepClient = viaCepClient ?? throw new ArgumentNullException(nameof(viaCepClient));
    private readonly IBrasilApiCepClient _brasilApiClient = brasilApiClient ?? throw new ArgumentNullException(nameof(brasilApiClient));

    public async Task<ZipCodeResponseDto> GetZipCodeAsync(ZipCode zipCode) {
        var normalizedZipCode = zipCode.NormalizedValue;

        try {
            _logger.LogInformation("Attempting to get address for CEP {ZipCode} using BrasilAPI", normalizedZipCode);
            var brasilResponse = await _brasilApiClient.GetAddressByZipCodeAsync(normalizedZipCode);

            if (brasilResponse != null) {
                _logger.LogInformation("Successfully retrieved address for CEP {ZipCode} using BrasilAPI", normalizedZipCode);
                return MapToDto(brasilResponse);
            }
        } catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to get address from BrasilAPI for CEP {ZipCode}. Falling back to ViaCEP...", normalizedZipCode);
        }

        try {
            _logger.LogInformation("Attempting to get address for CEP {ZipCode} using ViaCEP", normalizedZipCode);
            var viacepResponse = await _viaCepClient.GetAddressByZipCodeAsync(normalizedZipCode);

            if (viacepResponse != null) {
                _logger.LogInformation("Successfully retrieved address for CEP {ZipCode} using ViaCEP", normalizedZipCode);
                return MapToDto(viacepResponse);
            }
        } catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to get address from ViaCEP for CEP {ZipCode}", normalizedZipCode);
        }

        _logger.LogWarning("CEP not found in any provider: {ZipCode}", normalizedZipCode);
        return null;
    }

    private static ZipCodeResponseDto MapToDto(BrasilApiCepResponse response) {
        return new ZipCodeResponseDto {
            ZipCode = response.Cep,
            Street = response.Street,
            District = response.Neighborhood,
            City = response.City,
            State = response.State,
            Provider = "brasilapi",
            Location = (response.Location.Coordinates.Latitude.HasValue && response.Location.Coordinates.Longitude.HasValue)
                ? new ZipCodeResponseDto.LocationDto {
                    Lat = response.Location.Coordinates.Latitude.Value,
                    Lon = response.Location.Coordinates.Longitude.Value
                }
                : null
        };
    }

    private static ZipCodeResponseDto MapToDto(ViaCepResponse response) {
        return new ZipCodeResponseDto {
            ZipCode = response.Cep,
            Street = response.Logradouro,
            District = response.Bairro,
            City = response.Localidade,
            State = response.Estado,
            Ibge = response.Ibge,
            Provider = "viaCep",
            Location = null,
        };
    }
}