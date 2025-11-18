using Clima.Cep.Application.Dtos;
using Clima.Cep.Application.Interfaces.Services;
using Clima.Cep.Domain.Entities;
using Clima.Cep.Domain.Repositories;
using Clima.Cep.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clima.Cep.Application.Commands.PersistZipCode;

/// <summary>
/// Command para persistir um CEP no banco de dados.
/// </summary>
public record PersistZipCodeCommand(CreateZipCodeLookupRequestDto Request) : IRequest<ZipCodeLookupResponseDto>; 

public class PersistZipCodeCommandHandler(
    IZipCodeService zipCodeService,
    IZipCodeLookupRepository repository,
    ILogger<PersistZipCodeCommandHandler> logger) : IRequestHandler<PersistZipCodeCommand, ZipCodeLookupResponseDto>
{
    private readonly IZipCodeService _zipCodeService = zipCodeService;
    private readonly IZipCodeLookupRepository _repository = repository;
    private readonly ILogger<PersistZipCodeCommandHandler> _logger = logger;

    public async Task<ZipCodeLookupResponseDto> Handle(PersistZipCodeCommand request, CancellationToken cancellationToken) {
        ZipCode zipCodeVo;
        try {
            zipCodeVo = new ZipCode(request.Request.ZipCode);
        } catch (ArgumentException ex) {
            _logger.LogWarning(ex, "Invalid CEP received: {ZipCode}", request.Request.ZipCode);
            throw new ApplicationException("CEP inválido. CEP deve conter 8 dígitos.", ex);
        }

        if (await _repository.ExistsByZipCodeAsync(zipCodeVo)) {
            _logger.LogInformation("CEP {ZipCode} already persisted.", zipCodeVo.NormalizedValue);

            throw new ApplicationException($"CEP {zipCodeVo.NormalizedValue} já foi persistido.");
        }

        var zipCodeResult = await _zipCodeService.GetZipCodeAsync(zipCodeVo);

        if (zipCodeResult == null) {
            _logger.LogWarning("CEP {ZipCode} not found in any provider.", zipCodeVo.NormalizedValue);
            throw new ApplicationException($"CEP {zipCodeVo.NormalizedValue} não encontrado.");
        }

        var lookup = ZipCodeLookup.Create(
            zipCodeResult.ZipCode,
            zipCodeResult.Street,
            zipCodeResult.District,
            zipCodeResult.City,
            zipCodeResult.State,
            zipCodeResult.Ibge,
            zipCodeResult.Location?.Lat,
            zipCodeResult.Location?.Lon,
            zipCodeResult.Provider);

        await _repository.AddAsync(lookup);

        return MapToDto(lookup);
    }

    private static ZipCodeLookupResponseDto MapToDto(ZipCodeLookup lookup) {
        return new ZipCodeLookupResponseDto {
            Id = lookup.Id,
            ZipCode = lookup.ZipCode.NormalizedValue,
            Street = lookup.Street,
            District = lookup.District,
            City = lookup.City,
            State = lookup.State,
            Ibge = lookup.Ibge,
            Provider = lookup.Provider,
            CreatedAtUtc = lookup.CreatedAtUtc,
            Location = lookup.Location != null
                ? new ZipCodeResponseDto.LocationDto {
                    Lat = lookup.Location.Latitude,
                    Lon = lookup.Location.Longitude
                }
                : null
        };
    }
}