using Clima.Cep.Application.Services;
using Clima.Cep.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceIntegration.BrasilAPICEP.Interfaces;
using ServiceIntegration.BrasilAPICEP.Responses;
using ServiceIntegration.ViaCEP.Interfaces;
using ServiceIntegration.ViaCEP.Responses;

namespace Clima.Cep.Application.Testes.Services;

public class ZipCodeServiceTests
{
    private readonly Mock<IViaCepClient> _viaCepClientMock;
    private readonly Mock<ILogger<ZipCodeService>> _loggerMock;
    private readonly Mock<IBrasilApiCepClient> _brasilApiClientMock;

    private readonly ZipCodeService _service;

    public ZipCodeServiceTests()
    {
        _viaCepClientMock = new Mock<IViaCepClient>();
        _loggerMock = new Mock<ILogger<ZipCodeService>>();
        _brasilApiClientMock = new Mock<IBrasilApiCepClient>();

        _service = new ZipCodeService(
            _viaCepClientMock.Object,
            _brasilApiClientMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetZipCodeAsync_WithValidCep_BrasilAPISuccess_ShouldReturnZipCodeResponseDto()
    {
        var zipCode = new ZipCode("01310-100");
        var brasilResponse = new BrasilApiCepResponse
        {
            Cep = "01310-100",
            State = "SP",
            City = "São Paulo",
            Neighborhood = "Bela Vista",
            Street = "Avenida Paulista",
            Location = new LocationResponse
            {
                Coordinates = new CoordinatesResponse
                {
                    Latitude = -23.5615m,
                    Longitude = -46.6560m
                }
            }
        };

        _brasilApiClientMock.Setup(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(brasilResponse);

        // Act
        var result = await _service.GetZipCodeAsync(zipCode);

        Assert.NotNull(result);
        Assert.Equal(brasilResponse.Cep, result.ZipCode);
        Assert.Equal(brasilResponse.Street, result.Street);
        Assert.Equal(brasilResponse.City, result.City);
        Assert.Equal("brasilapi", result.Provider);
        Assert.NotNull(result.Location);
        Assert.Equal(brasilResponse.Location.Coordinates.Latitude, result.Location.Lat);
        _brasilApiClientMock.Verify(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _viaCepClientMock.Verify(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetZipCodeAsync_WithValidCep_BrasilAPIFails_FallsBackToViaCep_ShouldReturnZipCodeResponseDto()
    {
        var zipCode = new ZipCode("01310-100");
        var viaCepResponse = new ViaCepResponse
        {
            Cep = "01310-100",
            Logradouro = "Avenida Paulista",
            Bairro = "Bela Vista",
            Localidade = "São Paulo",
            Estado = "São Paulo",
            Uf = "SP",
            Ibge = "3550308"
        };

        _brasilApiClientMock.Setup(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("BrasilAPI error"));

        _viaCepClientMock.Setup(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(viaCepResponse);

        var result = await _service.GetZipCodeAsync(zipCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(viaCepResponse.Cep, result.ZipCode);
        Assert.Equal(viaCepResponse.Logradouro, result.Street);
        Assert.Equal("viaCep", result.Provider);
        _brasilApiClientMock.Verify(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _viaCepClientMock.Verify(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetZipCodeAsync_WithValidCep_BrasilAPIReturnsNull_FallsBackToViaCep_ShouldReturnZipCodeResponseDto()
    {
        var zipCode = new ZipCode("01310-100");
        var viaCepResponse = new ViaCepResponse
        {
            Cep = "01310-100",
            Logradouro = "Avenida Paulista",
            Bairro = "Bela Vista",
            Localidade = "São Paulo",
            Estado = "São Paulo",
            Uf = "SP"
        };

        _brasilApiClientMock.Setup(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BrasilApiCepResponse)null);

        _viaCepClientMock.Setup(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(viaCepResponse);

        var result = await _service.GetZipCodeAsync(zipCode);

        Assert.NotNull(result);
        Assert.Equal(viaCepResponse.Cep, result.ZipCode);
        Assert.Equal("viaCep", result.Provider);
    }

    [Fact]
    public async Task GetZipCodeAsync_WithInvalidCep_BothProvidersFail_ShouldReturnNull()
    {
        var zipCode = new ZipCode("99999-999");

        _brasilApiClientMock.Setup(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("CEP not found"));

        _viaCepClientMock.Setup(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("CEP not found"));

        var result = await _service.GetZipCodeAsync(zipCode);

        Assert.Null(result);
        _brasilApiClientMock.Verify(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _viaCepClientMock.Verify(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetZipCodeAsync_WithValidCep_BothProvidersReturnNull_ShouldReturnNull()
    {
        var zipCode = new ZipCode("01310-100");

        _brasilApiClientMock.Setup(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BrasilApiCepResponse)null);

        _viaCepClientMock.Setup(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ViaCepResponse)null);

        var result = await _service.GetZipCodeAsync(zipCode);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetZipCodeAsync_WithValidCep_BrasilAPISuccessWithoutLocation_ShouldReturnZipCodeResponseDtoWithoutLocation()
    {
        var zipCode = new ZipCode("01310-100");
        var brasilResponse = new BrasilApiCepResponse
        {
            Cep = "01310-100",
            State = "SP",
            City = "São Paulo",
            Neighborhood = "Bela Vista",
            Street = "Avenida Paulista",
            Location = new LocationResponse
            {
                Coordinates = new CoordinatesResponse
                {
                    Latitude = null,
                    Longitude = null
                }
            }
        };

        _brasilApiClientMock.Setup(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(brasilResponse);

        var result = await _service.GetZipCodeAsync(zipCode);

        Assert.NotNull(result);
        Assert.Null(result.Location);
    }

    [Fact]
    public async Task GetZipCodeAsync_WithFormattedCep_ShouldNormalizeAndQuery()
    {
        var zipCode = new ZipCode("01.310-100");
        var viaCepResponse = new ViaCepResponse
        {
            Cep = "01310-100",
            Logradouro = "Avenida Paulista",
            Bairro = "Bela Vista",
            Localidade = "São Paulo",
            Estado = "São Paulo",
            Uf = "SP"
        };

        _brasilApiClientMock.Setup(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BrasilApiCepResponse)null);

        _viaCepClientMock.Setup(x => x.GetAddressByZipCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(viaCepResponse);

        var result = await _service.GetZipCodeAsync(zipCode);

        Assert.NotNull(result);
        Assert.Equal("viaCep", result.Provider);
    }
}