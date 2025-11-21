using Clima.Cep.Domain.Entities;
using Clima.Cep.Domain.ValueObjects;
using Clima.Cep.Infrastructure.Repositories;

namespace Clima.Cep.Infrastructure.Tests.Repositories;

public class InMemoryZipCodeLookupRepositoryTests
{
    private readonly InMemoryZipCodeLookupRepository _repository;

    public InMemoryZipCodeLookupRepositoryTests()
    {
        _repository = new InMemoryZipCodeLookupRepository();
    }

    [Fact]
    public async Task AddAsync_WithValidLookup_ShouldAddToRepository()
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

        await _repository.AddAsync(lookup);

        var result = await _repository.GetByZipCodeAsync(zipCode);
        Assert.NotNull(result);
        Assert.Equal(lookup.Id, result.Id);
    }

    [Fact]
    public async Task ExistsByZipCodeAsync_WithExistingZipCode_ShouldReturnTrue()
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

        await _repository.AddAsync(lookup);

        var result = await _repository.ExistsByZipCodeAsync(zipCode);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByZipCodeAsync_WithNonExistingZipCode_ShouldReturnFalse()
    {
        var zipCode = new ZipCode("99999-999");

        var result = await _repository.ExistsByZipCodeAsync(zipCode);

        Assert.False(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnLookup()
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

        await _repository.AddAsync(lookup);

        var result = await _repository.GetByIdAsync(lookup.Id);

        Assert.NotNull(result);
        Assert.Equal(lookup.Id, result.Id);
        Assert.Equal(zipCode.NormalizedValue, result.ZipCode.NormalizedValue);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var invalidId = Guid.NewGuid();

        var result = await _repository.GetByIdAsync(invalidId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByZipCodeAsync_WithValidZipCode_ShouldReturnLookup()
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

        await _repository.AddAsync(lookup);

        var result = await _repository.GetByZipCodeAsync(zipCode);

        Assert.NotNull(result);
        Assert.Equal(zipCode.NormalizedValue, result.ZipCode.NormalizedValue);
        Assert.Equal(lookup.Street, result.Street);
        Assert.Equal(lookup.City, result.City);
    }

    [Fact]
    public async Task GetByZipCodeAsync_WithNonExistingZipCode_ShouldReturnNull()
    {
        var zipCode = new ZipCode("99999-999");

        var result = await _repository.GetByZipCodeAsync(zipCode);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllOrderedByCreationDateAsync_WithNoLookups_ShouldReturnEmpty()
    {
        var result = await _repository.GetAllOrderedByCreationDateAsync();

        Assert.Empty(result);
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

        await _repository.AddAsync(lookup1);

        await Task.Delay(10);

        var zipCode2 = new ZipCode("20040020");
        var lookup2 = new ZipCodeLookup(
            zipCode2,
            "Avenida Rio Branco",
            "Centro",
            "Rio de Janeiro",
            "RJ",
            "3304557",
            new Location(-22.9068m, -43.1729m),
            "BrasilAPI");

        await _repository.AddAsync(lookup2);

        var result = await _repository.GetAllOrderedByCreationDateAsync();

        var lookupList = result.ToList();
        Assert.Equal(2, lookupList.Count);
        Assert.Equal(lookup2.Id, lookupList[0].Id);
        Assert.Equal(lookup1.Id, lookupList[1].Id);
    }

    [Fact]
    public async Task AddAsync_WithMultipleLookups_ShouldPersistAll()
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

        var zipCode2 = new ZipCode("20040020");
        var lookup2 = new ZipCodeLookup(
            zipCode2,
            "Avenida Rio Branco",
            "Centro",
            "Rio de Janeiro",
            "RJ",
            "3304557",
            new Location(-22.9068m, -43.1729m),
            "BrasilAPI");

        await _repository.AddAsync(lookup1);
        await _repository.AddAsync(lookup2);

        var all = await _repository.GetAllOrderedByCreationDateAsync();
        Assert.Equal(2, all.Count());
    }

    [Fact]
    public async Task GetAllOrderedByCreationDateAsync_ShouldOrderByMostRecentFirst()
    {
        var zipCodes = new List<ZipCode>
        {
            new("01310-100"),
            new("20040020"),
            new("30130100")
        };

        foreach (var zipCode in zipCodes)
        {
            var lookup = new ZipCodeLookup(
                zipCode,
                "Street",
                "District",
                "City",
                "State",
                "1234567",
                new Location(0, 0),
                "Provider");

            await _repository.AddAsync(lookup);
            await Task.Delay(5);
        }

        var result = await _repository.GetAllOrderedByCreationDateAsync();

        var lookupList = result.ToList();
        Assert.Equal(3, lookupList.Count);
        Assert.Equal(zipCodes[2].NormalizedValue, lookupList[0].ZipCode.NormalizedValue);
        Assert.Equal(zipCodes[1].NormalizedValue, lookupList[1].ZipCode.NormalizedValue);
        Assert.Equal(zipCodes[0].NormalizedValue, lookupList[2].ZipCode.NormalizedValue);
    }
}