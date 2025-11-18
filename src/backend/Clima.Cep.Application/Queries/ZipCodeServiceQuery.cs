using Clima.Cep.Application.Dtos;
using Clima.Cep.Application.Interfaces.Queries;
using Clima.Cep.Domain.Repositories;
using Clima.Cep.Domain.ValueObjects;

namespace Clima.Cep.Application.Queries;

public class ZipCodeServiceQuery(
    IZipCodeLookupRepository repository) : IZipCodeServiceQuery
{
    private readonly IZipCodeLookupRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<IEnumerable<ZipCodeResponseDto>> GetAllOrderedByCreationDateAsync() {
        var entites = await _repository.GetAllOrderedByCreationDateAsync();

        return entites.Select(entity => MapEntityToDto(entity));
    }

    public async Task<ZipCodeResponseDto> GetByZipCodeAsync(ZipCode zipCode) {
        var entity = await _repository.GetByZipCodeAsync(zipCode);

        if (entity == null) {
            return default;
        }

        return MapEntityToDto(entity);
    }

    public ZipCodeResponseDto MapEntityToDto(Domain.Entities.ZipCodeLookup entity) {
        ArgumentNullException.ThrowIfNull(entity);

        return new ZipCodeResponseDto {
            Id = entity.Id,
            ZipCode = entity.ZipCode.ToString(),
            Street = entity.Street,
            District = entity.District,
            City = entity.City,
            State = entity.State,
            Ibge = entity.Ibge,
            Location = entity.Location is not null
                ? new ZipCodeResponseDto.LocationDto {
                    Lat = entity.Location.Latitude,
                    Lon = entity.Location.Longitude
                }
                : null,
            Provider = entity.Provider
        };
    }
}