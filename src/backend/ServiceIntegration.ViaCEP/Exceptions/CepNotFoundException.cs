namespace ServiceIntegration.ViaCEP.Exceptions;

public class CepNotFoundException(string cep) 
    : Exception($"CEP '{cep}' não encontrado ou inválido.")
{
}