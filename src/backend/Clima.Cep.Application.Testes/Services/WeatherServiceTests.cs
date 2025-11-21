using Clima.Cep.Application.Dtos;
using Clima.Cep.Application.Services;
using Clima.Cep.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceIntegration.OpenMeteoForecast.Interfaces;
using ServiceIntegration.OpenMeteoForecast.Responses;
using ServiceIntegration.OpenMeteoGeocoding.Interfaces;

namespace Clima.Cep.Application.Testes.Services;

public class WeatherServiceTests
{
    private readonly Mock<ILogger<WeatherService>> _loggerMock;
    private readonly Mock<IZipCodeLookupRepository> _repositoryMock;
    private readonly Mock<IOpenMeteoWeatherClient> _weatherClientMock;
    private readonly Mock<IOpenMeteoGeocodingClient> _geocodingClientMock;

    private readonly IMemoryCache _cache;
    private readonly WeatherService _service;

    public WeatherServiceTests()
    {
        _loggerMock = new Mock<ILogger<WeatherService>>();
        _repositoryMock = new Mock<IZipCodeLookupRepository>();
        _weatherClientMock = new Mock<IOpenMeteoWeatherClient>();
        _geocodingClientMock = new Mock<IOpenMeteoGeocodingClient>();

        _cache = new MemoryCache(new MemoryCacheOptions());

        _service = new WeatherService(
            _weatherClientMock.Object,
            _geocodingClientMock.Object,
            _repositoryMock.Object,
            _cache,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetWeatherBySavedZipCodesAsync_WithValidZipCodesAndStoredLocation_ShouldReturnWeatherData()
    {
        var zipCodeId = Guid.NewGuid();
        var zipCodeResponse = new ZipCodeResponseDto
        {
            Id = zipCodeId,
            ZipCode = "01310-100",
            City = "São Paulo",
            State = "SP",
            Location = new ZipCodeResponseDto.LocationDto { Lat = -23.5615m, Lon = -46.6560m }
        };

        var forecast = new WeatherForecast
        {
            TemperatureC = 25,
            ApparentTemperatureC = 26,
            Humidity = 65,
            ObservedAt = DateTime.UtcNow,
            DailyForecasts =
            [
                new DailyForecast { Date = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"), TempMinC = 18, TempMaxC = 28 },
                new DailyForecast { Date = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd"), TempMinC = 17, TempMaxC = 27 },
                new DailyForecast { Date = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd"), TempMinC = 19, TempMaxC = 29 }
            ]
        };

        _weatherClientMock.Setup(x => x.GetForecastAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>()))
            .ReturnsAsync(forecast);

        var result = await _service.GetWeatherBySavedZipCodesAsync([zipCodeResponse], days: 3);

        Assert.NotEmpty(result);
        var weather = result.First();
        Assert.NotNull(weather);
        Assert.Equal(zipCodeId, weather.SourceZipCodeId);
        Assert.Equal(forecast.TemperatureC, weather.Current.TemperatureC);
        Assert.Equal(forecast.DailyForecasts.Count(), weather.Daily.Count());
        _weatherClientMock.Verify(x => x.GetForecastAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), 3), Times.Once);
    }

    [Fact]
    public async Task GetWeatherBySavedZipCodesAsync_WithValidZipCodesWithoutStoredLocation_ShouldGeocode()
    {
        var zipCodeId = Guid.NewGuid();
        var zipCodeResponse = new ZipCodeResponseDto
        {
            Id = zipCodeId,
            ZipCode = "01310-100",
            City = "São Paulo",
            State = "SP",
            Location = null
        };

        var forecast = new WeatherForecast
        {
            TemperatureC = 25,
            ApparentTemperatureC = 26,
            Humidity = 65,
            ObservedAt = DateTime.UtcNow,
            DailyForecasts =
            [
                new DailyForecast { Date = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"), TempMinC = 18, TempMaxC = 28 }
            ]
        };

        _geocodingClientMock.Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((-23.5615m, -46.6560m));

        _weatherClientMock.Setup(x => x.GetForecastAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>()))
            .ReturnsAsync(forecast);

        var result = await _service.GetWeatherBySavedZipCodesAsync([zipCodeResponse], days: 1);

        Assert.NotEmpty(result);
        _geocodingClientMock.Verify(x => x.SearchAsync("São Paulo", "SP"), Times.Once);
        _weatherClientMock.Verify(x => x.GetForecastAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), 1), Times.Once);
    }

    [Fact]
    public async Task GetWeatherBySavedZipCodesAsync_WithInvalidDays_ShouldThrowArgumentException()
    {
        var zipCodeResponse = new ZipCodeResponseDto
        {
            ZipCode = "01310-100",
            City = "São Paulo",
            State = "SP",
            Location = new ZipCodeResponseDto.LocationDto { Lat = -23.5615m, Lon = -46.6560m }
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetWeatherBySavedZipCodesAsync([zipCodeResponse], days: 0));
        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetWeatherBySavedZipCodesAsync([zipCodeResponse], days: 8));
    }

    [Fact]
    public async Task GetWeatherBySavedZipCodesAsync_WithValidDays_ShouldReturnWeatherData()
    {
        var zipCodeResponse = new ZipCodeResponseDto
        {
            Id = Guid.NewGuid(),
            ZipCode = "01310-100",
            City = "São Paulo",
            State = "SP",
            Location = new ZipCodeResponseDto.LocationDto { Lat = -23.5615m, Lon = -46.6560m }
        };

        var forecast = new WeatherForecast
        {
            TemperatureC = 25,
            ApparentTemperatureC = 26,
            Humidity = 65,
            ObservedAt = DateTime.UtcNow,
            DailyForecasts =
            [
                new DailyForecast { Date = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"), TempMinC = 18, TempMaxC = 28 },
                new DailyForecast { Date = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd"), TempMinC = 17, TempMaxC = 27 },
                new DailyForecast { Date = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd"), TempMinC = 19, TempMaxC = 29 },
                new DailyForecast { Date = DateTime.UtcNow.AddDays(4).ToString("yyyy-MM-dd"), TempMinC = 20, TempMaxC = 30 },
                new DailyForecast { Date = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd"), TempMinC = 21, TempMaxC = 31 },
                new DailyForecast { Date = DateTime.UtcNow.AddDays(6).ToString("yyyy-MM-dd"), TempMinC = 22, TempMaxC = 32 },
                new DailyForecast { Date = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-dd"), TempMinC = 23, TempMaxC = 33 }
            ]
        };

        _weatherClientMock.Setup(x => x.GetForecastAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>()))
            .ReturnsAsync(forecast);

        for (int days = 1; days <= 7; days++)
        {
            var result = await _service.GetWeatherBySavedZipCodesAsync([zipCodeResponse], days: days);
            Assert.NotEmpty(result);
        }
    }

    [Fact]
    public async Task GetWeatherBySavedZipCodesAsync_WithCachedData_ShouldReturnCachedWeatherData()
    {
        var zipCodeId = Guid.NewGuid();
        var zipCodeResponse = new ZipCodeResponseDto
        {
            Id = zipCodeId,
            ZipCode = "01310-100",
            City = "São Paulo",
            State = "SP",
            Location = new ZipCodeResponseDto.LocationDto { Lat = -23.5615m, Lon = -46.6560m }
        };

        var forecast = new WeatherForecast
        {
            TemperatureC = 25,
            ApparentTemperatureC = 26,
            Humidity = 65,
            ObservedAt = DateTime.UtcNow,
            DailyForecasts =
            [
                new DailyForecast { Date = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"), TempMinC = 18, TempMaxC = 28 }
            ]
        };

        _weatherClientMock.Setup(x => x.GetForecastAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>()))
            .ReturnsAsync(forecast);

        var result1 = await _service.GetWeatherBySavedZipCodesAsync([zipCodeResponse], days: 1);

        var result2 = await _service.GetWeatherBySavedZipCodesAsync([zipCodeResponse], days: 1);

        Assert.NotEmpty(result1);
        Assert.NotEmpty(result2);
        _weatherClientMock.Verify(x => x.GetForecastAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), 1), Times.Once);
    }

    [Fact]
    public async Task GetWeatherBySavedZipCodesAsync_WithMultipleZipCodes_ShouldReturnWeatherForAll()
    {
        var zipCode1 = new ZipCodeResponseDto
        {
            Id = Guid.NewGuid(),
            ZipCode = "01310-100",
            City = "São Paulo",
            State = "SP",
            Location = new ZipCodeResponseDto.LocationDto { Lat = -23.5615m, Lon = -46.6560m }
        };

        var zipCode2 = new ZipCodeResponseDto
        {
            Id = Guid.NewGuid(),
            ZipCode = "20040-020",
            City = "Rio de Janeiro",
            State = "RJ",
            Location = new ZipCodeResponseDto.LocationDto { Lat = -22.9068m, Lon = -43.1729m }
        };

        var forecast = new WeatherForecast
        {
            TemperatureC = 25,
            ApparentTemperatureC = 26,
            Humidity = 65,
            ObservedAt = DateTime.UtcNow,
            DailyForecasts =
            [
                new DailyForecast { Date = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"), TempMinC = 18, TempMaxC = 28 }
            ]
        };

        _weatherClientMock.Setup(x => x.GetForecastAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>()))
            .ReturnsAsync(forecast);

        var result = await _service.GetWeatherBySavedZipCodesAsync([zipCode1, zipCode2], days: 1);

        Assert.Equal(2, result.Count());
        _weatherClientMock.Verify(x => x.GetForecastAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), 1), Times.Exactly(2));
    }

    [Fact]
    public async Task GetWeatherBySavedZipCodesAsync_WithFailingGeocoding_ShouldSkipAndReturnOthers()
    {
        var zipCode1 = new ZipCodeResponseDto
        {
            Id = Guid.NewGuid(),
            ZipCode = "01310-100",
            City = "São Paulo",
            State = "SP",
            Location = null
        };

        var zipCode2 = new ZipCodeResponseDto
        {
            Id = Guid.NewGuid(),
            ZipCode = "20040-020",
            City = "Rio de Janeiro",
            State = "RJ",
            Location = new ZipCodeResponseDto.LocationDto { Lat = -22.9068m, Lon = -43.1729m }
        };

        var forecast = new WeatherForecast
        {
            TemperatureC = 25,
            ApparentTemperatureC = 26,
            Humidity = 65,
            ObservedAt = DateTime.UtcNow,
            DailyForecasts =
            [
                new DailyForecast { Date = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"), TempMinC = 18, TempMaxC = 28 }
            ]
        };

        _geocodingClientMock.Setup(x => x.SearchAsync("São Paulo", "SP"))
            .ThrowsAsync(new Exception("Geocoding failed"));

        _weatherClientMock.Setup(x => x.GetForecastAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>()))
            .ReturnsAsync(forecast);

        var result = await _service.GetWeatherBySavedZipCodesAsync([zipCode1, zipCode2], days: 1);

        Assert.Single(result);
        Assert.Equal(zipCode2.Id, result.First().SourceZipCodeId);
    }

    [Fact]
    public async Task GetWeatherBySavedZipCodesAsync_WithNoSuccessfulResults_ShouldThrowInvalidOperationException()
    {
        var zipCodeResponse = new ZipCodeResponseDto
        {
            Id = Guid.NewGuid(),
            ZipCode = "01310-100",
            City = "São Paulo",
            State = "SP",
            Location = null
        };

        _geocodingClientMock.Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Geocoding failed"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.GetWeatherBySavedZipCodesAsync([zipCodeResponse], days: 1));
    }
}