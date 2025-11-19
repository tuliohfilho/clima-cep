namespace ServiceIntegration.BrasilAPICEP.Exceptions;

public class BrasilApiCepBadRequestException : Exception
{
    public BrasilApiCepBadRequestException(string cep)
        : base($"CEP '{cep}' possui formato inválido.") {
    }

    public BrasilApiCepBadRequestException(string cep, Exception innerException)
        : base($"CEP '{cep}' possui formato inválido.", innerException) {
    }
}