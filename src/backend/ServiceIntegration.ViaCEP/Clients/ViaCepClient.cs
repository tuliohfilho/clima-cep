using Microsoft.Extensions.Logging;
using ServiceIntegration.ViaCEP.Exceptions;
using ServiceIntegration.ViaCEP.Interfaces;
using ServiceIntegration.ViaCEP.Responses;
using System.Net;
using System.Net.Http.Json;

namespace ServiceIntegration.ViaCEP.Clients;

/// <summary>
/// Cliente HTTP para ViaCEP (provedor fallback).
/// Consulta: https://viacep.com.br/ws/{cep}/json/
/// </summary>
public class ViaCepClient(HttpClient httpClient, ILogger<ViaCepClient> logger) : IViaCepClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<ViaCepClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<ViaCepResponse> GetAddressByZipCodeAsync(string cep, CancellationToken cancellationToken = default) {
        cep = NormalizeCep(cep);

        var url = $"/ws/{cep}/json/";

        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.BadRequest)
            throw new CepBadRequestException(cep);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<ViaCepResponse>(cancellationToken: cancellationToken);

        if (result.HasErro())
            throw new CepNotFoundException(cep);

        return result;
    }

    private static string NormalizeCep(string cep)
        => new([.. cep.Where(char.IsDigit)]);
}