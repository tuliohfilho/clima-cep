using Clima.Cep.Application.Dtos;

namespace Clima.Cep.Application.Interfaces.Queries;

public interface IZipCodeServiceQuery
{
    Task<IEnumerable<ZipCodeResponseDto>> GetAllOrderedByCreationDateAsync();
    Task<ZipCodeResponseDto> GetByZipCodeAsync(Domain.ValueObjects.ZipCode zipCode);
}
