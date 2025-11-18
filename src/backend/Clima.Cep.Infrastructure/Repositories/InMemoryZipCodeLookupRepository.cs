using Clima.Cep.Domain.Entities;
using Clima.Cep.Domain.Repositories;
using Clima.Cep.Domain.ValueObjects;

namespace Clima.Cep.Infrastructure.Repositories;

/// <summary>
/// Implementação em memória do repositório de CEPs.
/// Persiste dados em memória (adequado para testes e prototipagem).
/// Para produção, considerar SQLite, EF Core com SQL Server, etc.
/// </summary>
public class InMemoryZipCodeLookupRepository : IZipCodeLookupRepository
{
    private readonly List<ZipCodeLookup> _lookups = [];
    private readonly object _lockObject = new();

    public Task AddAsync(ZipCodeLookup lookup)
    {
        lock (_lockObject)
        {
            _lookups.Add(lookup);
        }
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByZipCodeAsync(ZipCode zipCode)
    {
        lock (_lockObject)
        {
            var exists = _lookups.Any(l => l.ZipCode == zipCode);
            return Task.FromResult(exists);
        }
    }

    public Task<IEnumerable<ZipCodeLookup>> GetAllOrderedByCreationDateAsync()
    {
        lock (_lockObject)
        {
            var result = _lookups
                .OrderByDescending(l => l.CreatedAtUtc)
                .ToList()
                .AsEnumerable();
            return Task.FromResult(result);
        }
    }

    public Task<ZipCodeLookup> GetByIdAsync(Guid id)
    {
        lock (_lockObject)
        {
            var lookup = _lookups.FirstOrDefault(l => l.Id == id);
            return Task.FromResult(lookup);
        }
    }

    public Task<ZipCodeLookup> GetByZipCodeAsync(ZipCode zipCode) {
        lock (_lockObject) {
            var entity = _lookups.FirstOrDefault(l => l.ZipCode == zipCode);
            return Task.FromResult(entity);
        }
    }
}