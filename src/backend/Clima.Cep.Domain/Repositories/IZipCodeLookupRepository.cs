using Clima.Cep.Domain.Entities;
using Clima.Cep.Domain.ValueObjects;

namespace Clima.Cep.Domain.Repositories;

public interface IZipCodeLookupRepository
{
    /// <summary>
    /// Adiciona um novo registro de consulta de CEP.
    /// </summary>
    Task AddAsync(ZipCodeLookup lookup);

    /// <summary>
    /// Verifica se um CEP já foi persistido.
    /// </summary>
    Task<bool> ExistsByZipCodeAsync(ZipCode zipCode);

    /// <summary>
    /// Retorna todos os registros de CEPs salvos, ordenados pela data de criação (mais recente primeiro).
    /// </summary>
    Task<IEnumerable<ZipCodeLookup>> GetAllOrderedByCreationDateAsync();

    /// <summary>
    /// Retorna um registro de CEP pelo seu ID.
    /// </summary>
    Task<ZipCodeLookup> GetByIdAsync(Guid id);

    /// <summary>
    ///  Retorna um registro de CEP pelo seu zipCode
    /// </summary>
    Task<ZipCodeLookup> GetByZipCodeAsync(ZipCode zipCode);
}