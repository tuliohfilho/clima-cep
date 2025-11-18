using ServiceIntegration.BrasilAPICEP.Responses;

namespace ServiceIntegration.BrasilAPICEP.Interfaces;

public interface IBrasilApiCepClient
{
    Task<BrasilApiCepResponse> GetAddressByZipCodeAsync(string cep, CancellationToken cancellationToken = default);
}