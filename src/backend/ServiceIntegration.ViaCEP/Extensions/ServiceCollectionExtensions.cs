using Microsoft.Extensions.DependencyInjection;
using ServiceIntegration.ViaCEP.Clients;
using ServiceIntegration.ViaCEP.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ServiceIntegration.ViaCEP.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddViaCepClient(this IServiceCollection services) {
        services.AddHttpClient<IViaCepClient, ViaCepClient>(client => {
            client.BaseAddress = new Uri("https://viacep.com.br");
        });
    }
}