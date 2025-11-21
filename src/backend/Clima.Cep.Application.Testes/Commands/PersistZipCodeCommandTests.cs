using Clima.Cep.Application.Commands.PersistZipCode;
using Clima.Cep.Application.Dtos;
using Clima.Cep.Application.Interfaces.Services;
using Clima.Cep.Domain.Repositories;
using Clima.Cep.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace Clima.Cep.Application.Testes.Commands;

public class PersistZipCodeCommandTests
{
    private readonly Mock<IZipCodeService> _zipCodeServiceMock;
    private readonly Mock<IZipCodeLookupRepository> _repositoryMock;
    private readonly Mock<ILogger<PersistZipCodeCommandHandler>> _loggerMock;

    private readonly PersistZipCodeCommandHandler _handler;

    public PersistZipCodeCommandTests()
    {
        _zipCodeServiceMock = new Mock<IZipCodeService>();
        _repositoryMock = new Mock<IZipCodeLookupRepository>();
        _loggerMock = new Mock<ILogger<PersistZipCodeCommandHandler>>();

        _handler = new PersistZipCodeCommandHandler(
            _zipCodeServiceMock.Object,
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCep_ShouldPersistZipCodeAndReturnDto()
    {
        var request = new CreateZipCodeLookupRequestDto { ZipCode = "01310-100" };
        var command = new PersistZipCodeCommand(request);

        var zipCodeResponseDto = new ZipCodeResponseDto
        {
            ZipCode = "01310100",
            Street = "Avenida Paulista",
            District = "Bela Vista",
            City = "São Paulo",
            State = "SP",
            Ibge = "3550308",
            Provider = "viaCep",
            Location = new ZipCodeResponseDto.LocationDto { Lat = -23.5615m, Lon = -46.6560m }
        };

        _repositoryMock.Setup(x => x.ExistsByZipCodeAsync(It.IsAny<ZipCode>()))
            .ReturnsAsync(false);

        _zipCodeServiceMock.Setup(x => x.GetZipCodeAsync(It.IsAny<ZipCode>()))
            .ReturnsAsync(zipCodeResponseDto);

        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Domain.Entities.ZipCodeLookup>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(zipCodeResponseDto.ZipCode, result.ZipCode);
        Assert.Equal(zipCodeResponseDto.Street, result.Street);
        Assert.Equal(zipCodeResponseDto.City, result.City);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.ZipCodeLookup>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidCep_ShouldThrowApplicationException()
    {
        var request = new CreateZipCodeLookupRequestDto { ZipCode = "invalid" };
        var command = new PersistZipCodeCommand(request);

        await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithAlreadyPersistedCep_ShouldThrowApplicationException()
    {
        var request = new CreateZipCodeLookupRequestDto { ZipCode = "01310-100" };
        var command = new PersistZipCodeCommand(request);

        _repositoryMock.Setup(x => x.ExistsByZipCodeAsync(It.IsAny<ZipCode>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("já foi persistido", exception.Message);
    }

    [Fact]
    public async Task Handle_WithCepNotFoundInProviders_ShouldThrowApplicationException()
    {
        var request = new CreateZipCodeLookupRequestDto { ZipCode = "99999-999" };
        var command = new PersistZipCodeCommand(request);

        _repositoryMock.Setup(x => x.ExistsByZipCodeAsync(It.IsAny<ZipCode>()))
            .ReturnsAsync(false);

        _zipCodeServiceMock.Setup(x => x.GetZipCodeAsync(It.IsAny<ZipCode>()))
            .ReturnsAsync((ZipCodeResponseDto)null);

        var exception = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("não encontrado", exception.Message);
    }

    [Fact]
    public async Task Handle_WithValidCepAndLocation_ShouldPersistLocationData()
    {
        var request = new CreateZipCodeLookupRequestDto { ZipCode = "20040-020" };
        var command = new PersistZipCodeCommand(request);

        var zipCodeResponseDto = new ZipCodeResponseDto
        {
            ZipCode = "20040020",
            Street = "Avenida Rio Branco",
            District = "Centro",
            City = "Rio de Janeiro",
            State = "RJ",
            Provider = "brasilapi",
            Location = new ZipCodeResponseDto.LocationDto { Lat = -22.9068m, Lon = -43.1729m }
        };

        _repositoryMock.Setup(x => x.ExistsByZipCodeAsync(It.IsAny<ZipCode>()))
            .ReturnsAsync(false);

        _zipCodeServiceMock.Setup(x => x.GetZipCodeAsync(It.IsAny<ZipCode>()))
            .ReturnsAsync(zipCodeResponseDto);

        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Domain.Entities.ZipCodeLookup>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Location);
        Assert.Equal(zipCodeResponseDto.Location.Lat, result.Location.Lat);
        Assert.Equal(zipCodeResponseDto.Location.Lon, result.Location.Lon);
    }

    [Fact]
    public async Task Handle_WithValidCepWithoutLocation_ShouldPersistWithoutLocationData()
    {
        var request = new CreateZipCodeLookupRequestDto { ZipCode = "01310-100" };
        var command = new PersistZipCodeCommand(request);

        var zipCodeResponseDto = new ZipCodeResponseDto
        {
            ZipCode = "01310100",
            Street = "Avenida Paulista",
            District = "Bela Vista",
            City = "São Paulo",
            State = "SP",
            Provider = "viaCep",
            Location = null
        };

        _repositoryMock.Setup(x => x.ExistsByZipCodeAsync(It.IsAny<ZipCode>()))
            .ReturnsAsync(false);

        _zipCodeServiceMock.Setup(x => x.GetZipCodeAsync(It.IsAny<ZipCode>()))
            .ReturnsAsync(zipCodeResponseDto);

        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Domain.Entities.ZipCodeLookup>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Null(result.Location);
    }
}