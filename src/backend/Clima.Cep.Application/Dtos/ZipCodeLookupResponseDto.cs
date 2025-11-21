namespace Clima.Cep.Application.Dtos;

/// <summary>
/// DTO de resposta para CEP persistido.
/// Estende ZipCodeResponseDto com informações de persistência.
/// </summary>
public class ZipCodeLookupResponseDto : ZipCodeResponseDto
{
    /// <summary>
    /// Data e hora de criação (UTC).
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }
}