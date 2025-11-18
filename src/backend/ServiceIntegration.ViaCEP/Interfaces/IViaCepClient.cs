using ServiceIntegration.ViaCEP.Responses;

namespace ServiceIntegration.ViaCEP.Interfaces;

public interface IViaCepClient
{
    Task<ViaCepResponse> GetAddressByZipCodeAsync(string cep, CancellationToken cancellationToken = default);
}