using Microsoft.Extensions.DependencyInjection;
using ServiceIntegration.BrasilAPICEP.Clints;
using ServiceIntegration.BrasilAPICEP.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ServiceIntegration.BrasilAPICEP.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddBrasilAPICEPClient(this IServiceCollection services) {
        services.AddHttpClient<IBrasilApiCepClient, BrasilApiCepClient>(client => {
            client.BaseAddress = new Uri("https://brasilapi.com.br");
        });
    }
}