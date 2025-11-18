using ServiceIntegration.BrasilAPICEP.Interfaces;
using ServiceIntegration.BrasilAPICEP.Responses;
using System.Net;
using System.Net.Http.Json;

namespace ServiceIntegration.BrasilAPICEP.Clints;

public sealed class BrasilApiCepClient(HttpClient httpClient) : IBrasilApiCepClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<BrasilApiCepResponse> GetAddressByZipCodeAsync(string cep, CancellationToken cancellationToken = default) {
        cep = NormalizeCep(cep);

        var url = $"/api/cep/v2/{cep}";

        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<BrasilApiCepResponse>(cancellationToken: cancellationToken);

        return result;
    }

    private static string NormalizeCep(string cep)
        => new([.. cep.Where(char.IsDigit)]);
}