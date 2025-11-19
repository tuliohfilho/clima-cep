using Microsoft.Extensions.Logging;
using ServiceIntegration.BrasilAPICEP.Exceptions;
using ServiceIntegration.BrasilAPICEP.Interfaces;
using ServiceIntegration.BrasilAPICEP.Responses;
using System.Net;
using System.Net.Http.Json;

namespace ServiceIntegration.BrasilAPICEP.Clints;

public sealed class BrasilApiCepClient(HttpClient httpClient, ILogger<BrasilApiCepClient> logger) : IBrasilApiCepClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<BrasilApiCepClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<BrasilApiCepResponse> GetAddressByZipCodeAsync(string cep, CancellationToken cancellationToken = default) {
        cep = NormalizeCep(cep);

        var url = $"/api/cep/v2/{cep}";

        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.BadRequest)
            throw new BrasilApiCepBadRequestException(cep);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<BrasilApiCepResponse>(cancellationToken: cancellationToken);

        return result;
    }

    private static string NormalizeCep(string cep)
        => new([.. cep.Where(char.IsDigit)]);
}