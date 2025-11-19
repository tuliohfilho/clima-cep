using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceIntegration.OpenMeteoGeocoding.Clients;
using ServiceIntegration.OpenMeteoGeocoding.Responses;

namespace ServiceIntegration.OpenMeteoGeocoding.Tests.Clients;

public class OpenMeteoGeocodingClientTests
{
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<OpenMeteoGeocodingClient>> _loggerMock;

    private readonly OpenMeteoGeocodingClient _client;

    public OpenMeteoGeocodingClientTests()
    {
        _loggerMock = new Mock<ILogger<OpenMeteoGeocodingClient>>();
        _httpClient = new HttpClient { BaseAddress = new Uri("https://geocoding-api.open-meteo.com") };

        _client = new OpenMeteoGeocodingClient(_httpClient, _loggerMock.Object);
    }

    [Fact]
    public async Task SearchAsync_WithValidCityAndState_ReturnsCoordinates()
    {
        var city = "São Paulo";
        var state = "SP";

        var (latitude, longitude) = await _client.SearchAsync(city, state);

        latitude.Should().Be(-23.5475m);
        longitude.Should().Be(-46.63611m);
    }

    [Fact]
    public async Task SearchAsync_WithMultipleResults_ReturnsFirstResult()
    {
        var city = "Porto";
        var state = "Alegre";

        var (latitude, longitude) = await _client.SearchAsync(city, state);

        latitude.Should().Be(41.14961m);
        longitude.Should().Be(-8.61099m);
    }

    [Fact]
    public async Task SearchAsync_WithNullCity_ReturnsNullCoordinates()
    {
        string? city = null;
        var state = "SP";

        var (latitude, longitude) = await _client.SearchAsync(city, state);

        latitude.Should().BeNull();
        longitude.Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_WithEmptyCity_ReturnsNullCoordinates()
    {
        var city = "";
        var state = "SP";

        var (latitude, longitude) = await _client.SearchAsync(city, state);

        latitude.Should().BeNull();
        longitude.Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_WithWhitespaceCity_ReturnsNullCoordinates()
    {
        var city = "   ";
        var state = "SP";

        var (latitude, longitude) = await _client.SearchAsync(city, state);

        latitude.Should().BeNull();
        longitude.Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_WithNullState_ReturnsNullCoordinates()
    {
        var city = "São Paulo";
        string? state = null;

        var (latitude, longitude) = await _client.SearchAsync(city, state);

        latitude.Should().BeNull();
        longitude.Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_WithEmptyState_ReturnsNullCoordinates()
    {
        var city = "São Paulo";
        var state = "";

        var (latitude, longitude) = await _client.SearchAsync(city, state);

        latitude.Should().BeNull();
        longitude.Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_WithNoResults_ReturnsNullCoordinates()
    {
        var city = "UnknownCity";
        var state = "XX";
        var expectedResponse = new OpenMeteoGeocodingResponse
        {
            Results = new List<OpenMeteoGeocodingResponse.GeocodingResult>()
        };

        var (latitude, longitude) = await _client.SearchAsync(city, state);

        latitude.Should().BeNull();
        longitude.Should().BeNull();
    }

    [Fact]
    public void OpenMeteoGeocodingClient_WithNullHttpClient_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new OpenMeteoGeocodingClient(null, _loggerMock.Object));
    }

    [Fact]
    public void OpenMeteoGeocodingClient_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new OpenMeteoGeocodingClient(_httpClient, null));
    }
}