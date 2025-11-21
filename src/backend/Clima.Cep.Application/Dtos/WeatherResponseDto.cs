namespace Clima.Cep.Application.Dtos;

/// <summary>
/// DTO de resposta para consulta de clima.
/// </summary>
public class WeatherResponseDto
{
    /// <summary>
    /// ID do registro de CEP origem (ZipCodeLookup).
    /// </summary>
    public Guid SourceZipCodeId { get; set; }

    /// <summary>
    /// Informações de localização (latitude, longitude, cidade, estado).
    /// </summary>
    public LocationInfoDto Location { get; set; }

    /// <summary>
    /// Dados de clima atual.
    /// </summary>
    public CurrentWeatherDto Current { get; set; }

    /// <summary>
    /// Previsão diária (até 7 dias).
    /// </summary>
    public IEnumerable<DailyWeatherDto> Daily { get; set; }

    /// <summary>
    /// Provedor utilizado (open-meteo).
    /// </summary>
    public string Provider { get; set; }

    public class LocationInfoDto
    {
        /// <summary>
        /// Latitude.
        /// </summary>
        public decimal Lat { get; set; }

        /// <summary>
        /// Longitude.
        /// </summary>
        public decimal Lon { get; set; }

        /// <summary>
        /// Cidade.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Estado/Unidade Federativa.
        /// </summary>
        public string State { get; set; }
    }

    public class CurrentWeatherDto
    {
        /// <summary>
        /// Temperatura atual em graus Celsius.
        /// </summary>
        public double TemperatureC { get; set; }

        /// <summary>
        /// Umidade relativa (0-1 ou 0-100 conforme provedor).
        /// </summary>
        public double Humidity { get; set; }

        /// <summary>
        /// Temperatura aparente em graus Celsius.
        /// </summary>
        public double ApparentTemperatureC { get; set; }

        /// <summary>
        /// Data e hora da observação (UTC).
        /// </summary>
        public DateTime ObservedAt { get; set; }
    }

    public class DailyWeatherDto
    {
        /// <summary>
        /// Data da previsão (ISO 8601).
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Temperatura mínima em graus Celsius.
        /// </summary>
        public double TempMinC { get; set; }

        /// <summary>
        /// Temperatura máxima em graus Celsius.
        /// </summary>
        public double TempMaxC { get; set; }
    }
}
