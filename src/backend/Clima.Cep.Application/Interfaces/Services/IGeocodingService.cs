namespace Clima.Cep.Application.Interfaces.Services;

/// <summary>
/// Serviço de geocodificação para converter cidade + estado em coordenadas.
/// Utilizado como fallback quando CEP não possui lat/lon.
/// </summary>
public interface IGeocodingService
{
    /// <summary>
    /// Obtém latitude e longitude para uma cidade e estado.
    /// </summary>
    /// <param name="city">Nome da cidade.</param>
    /// <param name="state">Sigla do estado (ex.: SP, RJ).</param>
    /// <returns>Tupla com (latitude, longitude) ou (null, null) se não encontrado.</returns>
    Task<(decimal? Latitude, decimal? Longitude)> GetCoordinatesByCityAndStateAsync(string city, string state);
}