using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceIntegration.OpenMeteoForecast.Clients;

namespace ServiceIntegration.OpenMeteoForecast.Tests.Clients;

public class OpenMeteoWeatherClientTests
{
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<OpenMeteoWeatherClient>> _loggerMock;

    private readonly OpenMeteoWeatherClient _client;

    public OpenMeteoWeatherClientTests()
    {
        _loggerMock = new Mock<ILogger<OpenMeteoWeatherClient>>();
        _httpClient = new HttpClient { BaseAddress = new Uri("https://api.open-meteo.com") };

        _client = new OpenMeteoWeatherClient(_httpClient, _loggerMock.Object);
    }

    [Fact]
    public async Task GetForecastAsync_WithValidCoordinates_ReturnsForecast()
    {
        var latitude = -23.5505m;
        var longitude = -46.6333m;
        var days = 3;
        
        var result = await _client.GetForecastAsync(latitude, longitude, days);

        result.Should().NotBeNull();
        result.DailyForecasts.Should().HaveCount(days);
    }

    [Fact]
    public async Task GetForecastAsync_WithMaxDays_ReturnsForecast()
    {
        var latitude = -23.5505m;
        var longitude = -46.6333m;
        var days = 7;
        
        var result = await _client.GetForecastAsync(latitude, longitude, days);

        result.Should().NotBeNull();
        result.DailyForecasts.Should().HaveCount(days);
    }

    [Fact]
    public async Task GetForecastAsync_WithMinDays_ReturnsForecast()
    {
        var latitude = -23.5505m;
        var longitude = -46.6333m;
        var days = 1;
        
        var result = await _client.GetForecastAsync(latitude, longitude, days);

        result.Should().NotBeNull();
        result.DailyForecasts.Should().HaveCount(days);
    }

    [Fact]
    public async Task GetForecastAsync_WithDaysLessThan1_ThrowsArgumentException()
    {
        var latitude = -23.5505m;
        var longitude = -46.6333m;
        var days = 0;

        await Assert.ThrowsAsync<ArgumentException>(() => _client.GetForecastAsync(latitude, longitude, days));
    }

    [Fact]
    public async Task GetForecastAsync_WithDaysGreaterThan7_ThrowsArgumentException()
    {
        var latitude = -23.5505m;
        var longitude = -46.6333m;
        var days = 8;

        await Assert.ThrowsAsync<ArgumentException>(() => _client.GetForecastAsync(latitude, longitude, days));
    }

    [Fact]
    public async Task GetForecastAsync_WithNegativeDays_ThrowsArgumentException()
    {
        var latitude = -23.5505m;
        var longitude = -46.6333m;
        var days = -1;

        await Assert.ThrowsAsync<ArgumentException>(() => _client.GetForecastAsync(latitude, longitude, days));
    }

    [Fact]
    public void OpenMeteoWeatherClient_WithNullHttpClient_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new OpenMeteoWeatherClient(null, _loggerMock.Object));
    }

    [Fact]
    public void OpenMeteoWeatherClient_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new OpenMeteoWeatherClient(_httpClient, null));
    }
}