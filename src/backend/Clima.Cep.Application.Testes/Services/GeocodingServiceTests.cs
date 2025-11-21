using Clima.Cep.Application.Services;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceIntegration.OpenMeteoGeocoding.Interfaces;

namespace Clima.Cep.Application.Testes.Services;

public class GeocodingServiceTests
{
    private readonly Mock<ILogger<GeocodingService>> _loggerMock;
    private readonly Mock<IOpenMeteoGeocodingClient> _apiClientMock;

    private readonly GeocodingService _service;

    public GeocodingServiceTests() {
        _loggerMock = new Mock<ILogger<GeocodingService>>();
        _apiClientMock = new Mock<IOpenMeteoGeocodingClient>();

        _service = new GeocodingService(
            _apiClientMock.Object,
            _loggerMock.Object
        );
    }
}