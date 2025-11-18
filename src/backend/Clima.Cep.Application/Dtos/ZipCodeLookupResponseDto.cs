namespace Clima.Cep.Application.Dtos;

/// <summary>
/// DTO de resposta para CEP persistido (US02 - POST /cep).
/// Estende ZipCodeResponseDto com informações de persistência.
/// </summary>
public class ZipCodeLookupResponseDto : ZipCodeResponseDto
{
    /// <summary>
    /// Identificador único do registro no banco de dados.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Data e hora de criação (UTC).
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }
}
