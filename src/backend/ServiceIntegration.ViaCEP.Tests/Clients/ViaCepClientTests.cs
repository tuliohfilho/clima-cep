using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceIntegration.ViaCEP.Clients;
using ServiceIntegration.ViaCEP.Exceptions;
using ServiceIntegration.ViaCEP.Responses;

namespace ServiceIntegration.ViaCEP.Tests.Clients;

public class ViaCepClientTests
{
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<ViaCepClient>> _loggerMock;

    private readonly ViaCepClient _client;

    public ViaCepClientTests()
    {
        _loggerMock = new Mock<ILogger<ViaCepClient>>();

        _httpClient = new HttpClient {
            BaseAddress = new Uri("https://viacep.com.br"),
            DefaultRequestHeaders = {
                { "Accept", "application/json" }
            }
        };

        _client = new ViaCepClient(_httpClient, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAddressByZipCodeAsync_WithValidCep_ReturnsAddress()
    {
        var cep = "01310-100";
        var expectedResponse = new ViaCepResponse
        {
            Cep = "01310-100",
            Logradouro = "Avenida Paulista",
            Complemento = "",
            Bairro = "Bela Vista",
            Localidade = "São Paulo",
            Uf = "SP",
            Estado = "São Paulo",
            Regiao = "Sudeste",
            Ibge = "3550308",
            Gia = "1234567",
            Ddd = "11",
            Siafi = "7107"
        };

        var result = await _client.GetAddressByZipCodeAsync(cep);

        result.Should().NotBeNull();
        result.Cep.Should().Be(expectedResponse.Cep);
        result.Localidade.Should().Be(expectedResponse.Localidade);
        result.Uf.Should().Be(expectedResponse.Uf);
        result.Logradouro.Should().Be(expectedResponse.Logradouro);
    }

    [Fact]
    public async Task GetAddressByZipCodeAsync_WithCepWithSpecialCharacters_NormalizesAndReturnsAddress()
    {
        var cep = "01.310-100";
        var expectedResponse = new ViaCepResponse
        {
            Cep = "01310-100",
            Logradouro = "Avenida Paulista",
            Complemento = "",
            Bairro = "Bela Vista",
            Localidade = "São Paulo",
            Uf = "SP",
            Estado = "São Paulo",
            Regiao = "Sudeste",
            Ibge = "3550308",
            Gia = "1234567",
            Ddd = "11",
            Siafi = "7107"
        };

        var result = await _client.GetAddressByZipCodeAsync(cep);

        result.Should().NotBeNull();
        result.Localidade.Should().Be(expectedResponse.Localidade);
    }

    [Fact]
    public async Task GetAddressByZipCodeAsync_WithOnlyDigits_ReturnsAddress()
    {
        var cep = "01310100";
        var expectedResponse = new ViaCepResponse
        {
            Cep = "01310-100",
            Logradouro = "Avenida Paulista",
            Complemento = "",
            Bairro = "Bela Vista",
            Localidade = "São Paulo",
            Uf = "SP",
            Estado = "São Paulo",
            Regiao = "Sudeste"
        };

        var result = await _client.GetAddressByZipCodeAsync(cep);

        result.Should().NotBeNull();
        result.Localidade.Should().Be(expectedResponse.Localidade);
    }

    [Fact]
    public async Task GetAddressByZipCodeAsync_WithServerError_ThrowsException()
    {
        var cep = "99999999";

        await Assert.ThrowsAsync<CepNotFoundException>(() => _client.GetAddressByZipCodeAsync(cep));
    }

    [Fact]
    public async Task GetAddressByZipCodeAsync_WithResponseNull_ReturnsNull() {
        var cep = "1";

        await Assert.ThrowsAsync<CepBadRequestException>(() => _client.GetAddressByZipCodeAsync(cep));
    }

    [Fact]
    public void ViaCepClient_WithNullHttpClient_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ViaCepClient(null, _loggerMock.Object));
    }

    [Fact]
    public void ViaCepClient_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ViaCepClient(_httpClient, null));
    }
}