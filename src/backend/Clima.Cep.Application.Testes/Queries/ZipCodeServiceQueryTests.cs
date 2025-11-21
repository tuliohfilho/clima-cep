using Clima.Cep.Application.Queries;
using Clima.Cep.Domain.Entities;
using Clima.Cep.Domain.Repositories;
using Clima.Cep.Domain.ValueObjects;
using Moq;

namespace Clima.Cep.Application.Testes.Queries;

public class ZipCodeServiceQueryTests
{
    private readonly Mock<IZipCodeLookupRepository> _repositoryMock;

    private readonly ZipCodeServiceQuery _query;

    public ZipCodeServiceQueryTests()
    {
        _repositoryMock = new Mock<IZipCodeLookupRepository>();

        _query = new ZipCodeServiceQuery(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllOrderedByCreationDateAsync_WithMultipleLookups_ShouldReturnOrderedByCreationDate()
    {
        var zipCode1 = new ZipCode("01310-100");
        var lookup1 = new ZipCodeLookup(
            zipCode1,
            "Avenida Paulista",
            "Bela Vista",
            "São Paulo",
            "SP",
            "3550308",
            new Location(-23.5615m, -46.6560m),
            "ViaCEP");

        var zipCode2 = new ZipCode("20040-020");
        var lookup2 = new ZipCodeLookup(
            zipCode2,
            "Avenida Rio Branco",
            "Centro",
            "Rio de Janeiro",
            "RJ",
            "3304557",
            new Location(-22.9068m, -43.1729m),
            "BrasilAPI");

        var lookups = new[] { lookup1, lookup2 };

        _repositoryMock.Setup(x => x.GetAllOrderedByCreationDateAsync())
            .ReturnsAsync(lookups);

        var result = await _query.GetAllOrderedByCreationDateAsync();

        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal(lookup1.ZipCode.NormalizedValue, resultList[0].ZipCode);
        Assert.Equal(lookup2.ZipCode.NormalizedValue, resultList[1].ZipCode);
        _repositoryMock.Verify(x => x.GetAllOrderedByCreationDateAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllOrderedByCreationDateAsync_WithNoLookups_ShouldReturnEmpty()
    {
        var lookups = Enumerable.Empty<ZipCodeLookup>();

        _repositoryMock.Setup(x => x.GetAllOrderedByCreationDateAsync())
            .ReturnsAsync(lookups);

        var result = await _query.GetAllOrderedByCreationDateAsync();

        Assert.Empty(result);
        _repositoryMock.Verify(x => x.GetAllOrderedByCreationDateAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByZipCodeAsync_WithValidZipCode_ShouldReturnZipCodeResponseDto()
    {
        var zipCode = new ZipCode("01310-100");
        var lookup = new ZipCodeLookup(
            zipCode,
            "Avenida Paulista",
            "Bela Vista",
            "São Paulo",
            "SP",
            "3550308",
            new Location(-23.5615m, -46.6560m),
            "ViaCEP");

        _repositoryMock.Setup(x => x.GetByZipCodeAsync(zipCode))
            .ReturnsAsync(lookup);

        var result = await _query.GetByZipCodeAsync(zipCode);

        Assert.NotNull(result);
        Assert.Equal(lookup.ZipCode.NormalizedValue, result.ZipCode);
        Assert.Equal(lookup.Street, result.Street);
        Assert.Equal(lookup.City, result.City);
        Assert.Equal(lookup.State, result.State);
        Assert.NotNull(result.Location);
        Assert.Equal(lookup.Location.Latitude, result.Location.Lat);
        Assert.Equal(lookup.Location.Longitude, result.Location.Lon);
        _repositoryMock.Verify(x => x.GetByZipCodeAsync(zipCode), Times.Once);
    }

    [Fact]
    public async Task GetByZipCodeAsync_WithNonExistingZipCode_ShouldReturnNull()
    {
        var zipCode = new ZipCode("99999-999");

        _repositoryMock.Setup(x => x.GetByZipCodeAsync(zipCode))
            .ReturnsAsync((ZipCodeLookup)null);

        var result = await _query.GetByZipCodeAsync(zipCode);

        Assert.Null(result);
        _repositoryMock.Verify(x => x.GetByZipCodeAsync(zipCode), Times.Once);
    }

    [Fact]
    public async Task GetByZipCodeAsync_WithValidZipCode_ShouldMapAllProperties()
    {
        var zipCode = new ZipCode("01310-100");
        var lookup = new ZipCodeLookup(
            zipCode,
            "Avenida Paulista",
            "Bela Vista",
            "São Paulo",
            "SP",
            "3550308",
            new Location(-23.5615m, -46.6560m),
            "ViaCEP");

        _repositoryMock.Setup(x => x.GetByZipCodeAsync(zipCode))
            .ReturnsAsync(lookup);

        var result = await _query.GetByZipCodeAsync(zipCode);

        Assert.NotNull(result);
        Assert.Equal(lookup.Id, result.Id);
        Assert.Equal(lookup.ZipCode.ToString(), result.ZipCode);
        Assert.Equal(lookup.Street, result.Street);
        Assert.Equal(lookup.District, result.District);
        Assert.Equal(lookup.City, result.City);
        Assert.Equal(lookup.State, result.State);
        Assert.Equal(lookup.Ibge, result.Ibge);
        Assert.Equal(lookup.Provider, result.Provider);
    }

    [Fact]
    public async Task GetByZipCodeAsync_WithValidZipCode_WithoutLocation_ShouldReturnDtoWithoutLocation()
    {
        var zipCode = new ZipCode("20040-020");
        var lookup = new ZipCodeLookup(
            zipCode,
            "Avenida Rio Branco",
            "Centro",
            "Rio de Janeiro",
            "RJ",
            "3304557",
            null,
            "ViaCEP");

        _repositoryMock.Setup(x => x.GetByZipCodeAsync(zipCode))
            .ReturnsAsync(lookup);

        var result = await _query.GetByZipCodeAsync(zipCode);

        Assert.NotNull(result);
        Assert.Null(result.Location);
    }

    [Fact]
    public async Task GetAllOrderedByCreationDateAsync_ShouldMapAllProperties()
    {
        var zipCode = new ZipCode("01310-100");
        var lookup = new ZipCodeLookup(
            zipCode,
            "Avenida Paulista",
            "Bela Vista",
            "São Paulo",
            "SP",
            "3550308",
            new Location(-23.5615m, -46.6560m),
            "ViaCEP");

        var lookups = new[] { lookup };

        _repositoryMock.Setup(x => x.GetAllOrderedByCreationDateAsync())
            .ReturnsAsync(lookups);

        var result = await _query.GetAllOrderedByCreationDateAsync();

        var resultList = result.ToList();
        Assert.Single(resultList);
        var dto = resultList[0];
        Assert.Equal(lookup.Id, dto.Id);
        Assert.Equal(lookup.ZipCode.ToString(), dto.ZipCode);
        Assert.Equal(lookup.Street, dto.Street);
        Assert.Equal(lookup.District, dto.District);
        Assert.Equal(lookup.City, dto.City);
        Assert.Equal(lookup.State, dto.State);
        Assert.Equal(lookup.Ibge, dto.Ibge);
        Assert.Equal(lookup.Provider, dto.Provider);
    }

    [Fact]
    public void MapEntityToDto_WithNullEntity_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _query.MapEntityToDto(null));
    }

    [Fact]
    public async Task GetAllOrderedByCreationDateAsync_WithMultipleLookups_WithoutLocation_ShouldReturnDtosWithoutLocation()
    {
        var zipCode1 = new ZipCode("01310-100");
        var lookup1 = new ZipCodeLookup(
            zipCode1,
            "Avenida Paulista",
            "Bela Vista",
            "São Paulo",
            "SP",
            "3550308",
            null,
            "ViaCEP");

        var zipCode2 = new ZipCode("20040-020");
        var lookup2 = new ZipCodeLookup(
            zipCode2,
            "Avenida Rio Branco",
            "Centro",
            "Rio de Janeiro",
            "RJ",
            "3304557",
            null,
            "BrasilAPI");

        var lookups = new[] { lookup1, lookup2 };

        _repositoryMock.Setup(x => x.GetAllOrderedByCreationDateAsync())
            .ReturnsAsync(lookups);

        var result = await _query.GetAllOrderedByCreationDateAsync();

        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.All(resultList, dto => Assert.Null(dto.Location));
    }
}