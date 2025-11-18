using Clima.Cep.Application.Dtos;

namespace Clima.Cep.Application.Interfaces.Services;

/// <summary>
/// Serviço para consulta de clima e previsão (US03).
/// Utiliza os CEPs persistidos (latitude/longitude) como referência.
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Retorna clima atual e previsão para os CEPs persistidos.
    /// </summary>
    /// <param name="zipCodeResponses">Lista de CEPs salvos.</param>
    /// <param name="days">Número de dias de previsão (1-7, padrão 3).</param>
    /// <returns>Lista com clima de cada CEP persistido.</returns>
    /// <exception cref="ArgumentException">Se days estiver fora do intervalo 1-7.</exception>
    /// <exception cref="InvalidOperationException">Se nenhum CEP for persistido.</exception>
    Task<IEnumerable<WeatherResponseDto>> GetWeatherBySavedZipCodesAsync(IEnumerable<ZipCodeResponseDto> zipCodeResponses, int days = 3);
}
