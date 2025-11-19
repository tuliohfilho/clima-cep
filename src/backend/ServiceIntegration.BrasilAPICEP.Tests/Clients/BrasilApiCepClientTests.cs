using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceIntegration.BrasilAPICEP.Clints;
using ServiceIntegration.BrasilAPICEP.Exceptions;
using ServiceIntegration.BrasilAPICEP.Responses;

namespace ServiceIntegration.BrasilAPICEP.Tests.Clients;

public class BrasilApiCepClientTests
{
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<BrasilApiCepClient>> _loggerMock;

    private readonly BrasilApiCepClient _client;

    public BrasilApiCepClientTests()
    {
        _loggerMock = new Mock<ILogger<BrasilApiCepClient>>();

        _httpClient = new HttpClient { BaseAddress = new Uri("https://brasilapi.com.br") };

        _client = new BrasilApiCepClient(_httpClient, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAddressByZipCodeAsync_WithValidCep_ReturnsAddress()
    {
        var cep = "01310-100";
        var expectedResponse = new BrasilApiCepResponse
        {
            Cep = "01310100",
            State = "SP",
            City = "São Paulo",
            Neighborhood = "Centro",
            Street = "Avenida Paulista",
            Service = "brasilia",
            Provider = "brasilapi"
        };

        var result = await _client.GetAddressByZipCodeAsync(cep);

        result.Should().NotBeNull();
        result.Cep.Should().Be(expectedResponse.Cep);
        result.City.Should().Be(expectedResponse.City);
        result.State.Should().Be(expectedResponse.State);
        result.Street.Should().Be(expectedResponse.Street);
    }

    [Fact]
    public async Task GetAddressByZipCodeAsync_WithCepWithSpecialCharacters_NormalizesAndReturnsAddress()
    {
        var cep = "01.310-100";
        var expectedResponse = new BrasilApiCepResponse
        {
            Cep = "01310100",
            State = "SP",
            City = "São Paulo",
            Neighborhood = "Centro",
            Street = "Avenida Paulista",
            Service = "brasilia",
            Provider = "brasilapi"
        };

        var result = await _client.GetAddressByZipCodeAsync(cep);

        result.Should().NotBeNull();
        result.Cep.Should().Be(expectedResponse.Cep);
    }

    [Fact]
    public async Task GetAddressByZipCodeAsync_WithOnlyDigits_ReturnsAddress() {
        var cep = "01310100";
        var expectedResponse = new BrasilApiCepResponse {
            Cep = "01310100",
            State = "SP",
            City = "São Paulo",
            Neighborhood = "Centro",
            Street = "Avenida Paulista",
            Service = "brasilia",
            Provider = "brasilapi"
        };

        var result = await _client.GetAddressByZipCodeAsync(cep);

        result.Should().NotBeNull();
        result.Cep.Should().Be(expectedResponse.Cep);
    }

    [Fact]
    public async Task GetAddressByZipCodeAsync_WithBadRequest_ThrowsException()
    {
        var cep = "123";

        await Assert.ThrowsAsync<BrasilApiCepBadRequestException>(() => _client.GetAddressByZipCodeAsync(cep));
    }

    [Fact]
    public void GetAddressByZipCodeAsync_WithNullHttpClient_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() => new BrasilApiCepClient(null, _loggerMock.Object));
    }

    [Fact]
    public void GetAddressByZipCodeAsync_WithNullLogger_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() => new BrasilApiCepClient(_httpClient, null));
    }
}