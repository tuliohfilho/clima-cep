namespace ServiceIntegration.ViaCEP.Exceptions;

public class CepBadRequestException : Exception
{
    public CepBadRequestException(string cep)
        : base($"CEP '{cep}' possui formato inválido.") {
    }

    public CepBadRequestException(string cep, Exception innerException)
        : base($"CEP '{cep}' possui formato inválido.", innerException) {
    }
}