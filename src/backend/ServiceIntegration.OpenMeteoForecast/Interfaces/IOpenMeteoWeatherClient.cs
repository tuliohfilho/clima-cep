using ServiceIntegration.OpenMeteoForecast.Responses;
using System.Threading.Tasks;

namespace ServiceIntegration.OpenMeteoForecast.Interfaces;

public interface IOpenMeteoWeatherClient
{
    Task<WeatherForecast> GetForecastAsync(decimal latitude, decimal longitude, int days);
}