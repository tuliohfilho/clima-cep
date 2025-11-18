using Clima.Cep.Application.Interfaces.Queries;
using Clima.Cep.Application.Interfaces.Services;
using Clima.Cep.Application.Queries;
using Clima.Cep.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Clima.Cep.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddApplicationLayer(this IServiceCollection services) {
        // Register application services
        services.AddScoped<IZipCodeService, ZipCodeService>();
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<IGeocodingService, GeocodingService>();

        // Register application queries
        services.AddScoped<IZipCodeServiceQuery, ZipCodeServiceQuery>();

        // Register MidiateR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
    }
}