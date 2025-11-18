using Microsoft.Extensions.DependencyInjection;
using ServiceIntegration.OpenMeteoGeocoding.Clients;
using ServiceIntegration.OpenMeteoGeocoding.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ServiceIntegration.OpenMeteoGeocoding.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddOpenMeteoGeocodingClient(this IServiceCollection services) {
        services.AddHttpClient<IOpenMeteoGeocodingClient, OpenMeteoGeocodingClient>(client => {
            client.BaseAddress = new Uri("https://geocoding-api.open-meteo.com");
        });
    }
}