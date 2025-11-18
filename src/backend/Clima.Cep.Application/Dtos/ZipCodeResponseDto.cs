namespace Clima.Cep.Application.Dtos;

/// <summary>
/// DTO de resposta para consulta de CEP (US01 - GET /cep/{cep}).
/// </summary>
public class ZipCodeResponseDto
{
    /// <summary>
    /// Id da consulta de CEP persistida.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// CEP normalizado (8 dígitos).
    /// </summary>
    public string ZipCode { get; set; }

    /// <summary>
    /// Nome da rua/avenida.
    /// </summary>
    public string Street { get; set; }

    /// <summary>
    /// Bairro/distrito.
    /// </summary>
    public string District { get; set; }

    /// <summary>
    /// Cidade.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Unidade federativa (ex.: SP, RJ).
    /// </summary>
    public string State { get; set; }

    /// <summary>
    /// Código IBGE do município.
    /// </summary>
    public string Ibge { get; set; }

    /// <summary>
    /// Coordenadas geográficas (latitude e longitude).
    /// </summary>
    public LocationDto Location { get; set; }

    /// <summary>
    /// Provedor utilizado (brasilapi, viacep, etc.).
    /// </summary>
    public string Provider { get; set; }

    public class LocationDto
    {
        /// <summary>
        /// Latitude.
        /// </summary>
        public decimal Lat { get; set; }

        /// <summary>
        /// Longitude.
        /// </summary>
        public decimal Lon { get; set; }
    }
}