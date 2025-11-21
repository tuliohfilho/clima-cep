namespace ServiceIntegration.ViaCEP.Responses;

/// <summary>
/// Modelo de resposta da API ViaCEP.
/// </summary>
public class ViaCepResponse
{
    public string Cep { get; set; }
    public string Logradouro { get; set; }
    public string Complemento { get; set; }
    public string Unidade { get; set; }
    public string Bairro { get; set; }
    public string Localidade { get; set; }
    public string Uf { get; set; }
    public string Estado { get; set; }
    public string Regiao { get; set; }
    public string Ibge { get; set; }
    public string Gia { get; set; }
    public string Ddd { get; set; }
    public string Siafi { get; set; }
    public string Erro { get; set; }

    public bool HasErro() {
        if (!string.IsNullOrWhiteSpace(Erro)) {
            var str = Erro.ToLower();

            if (str == "true" || str == "1") return true;
            if (str == "false" || str == "0") return false;
        }

        return false;
    }
}