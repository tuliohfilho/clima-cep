namespace Clima.Cep.Application.Dtos;

/// <summary>
/// DTO de requisição para persistir um novo CEP (US02 - POST /cep).
/// </summary>
public class CreateZipCodeLookupRequestDto
{
    /// <summary>
    /// CEP a ser consultado e persistido.
    /// Aceita com ou sem hífen (ex.: 01001-000 ou 01001000).
    /// </summary>
    public string ZipCode { get; set; }
}
