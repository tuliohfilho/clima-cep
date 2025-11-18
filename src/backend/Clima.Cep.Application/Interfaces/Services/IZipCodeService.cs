using Clima.Cep.Application.Dtos;

namespace Clima.Cep.Application.Interfaces.Services;

/// <summary>
/// Serviço de domínio/aplicação para consulta de CEP com lógica de fallback.
/// </summary>
public interface IZipCodeService
{
    /// <summary>
    /// Consulta um CEP usando o provedor primário e fallback em caso de falha.
    /// </summary>
    /// <param name="zipCode">O CEP a ser consultado.</param>
    /// <returns>O DTO de resposta do CEP.</returns>
    Task<ZipCodeResponseDto> GetZipCodeAsync(Domain.ValueObjects.ZipCode zipCode);
}