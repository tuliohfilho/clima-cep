using Microsoft.Extensions.DependencyInjection;
using ServiceIntegration.OpenMeteoForecast.Clients;
using ServiceIntegration.OpenMeteoForecast.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ServiceIntegration.OpenMeteoForecast.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddOpenMeteoWeatherClient(this IServiceCollection services) {
        services.AddHttpClient<IOpenMeteoWeatherClient, OpenMeteoWeatherClient>(client => {
            client.BaseAddress = new Uri("https://api.open-meteo.com");
        });
    }
}