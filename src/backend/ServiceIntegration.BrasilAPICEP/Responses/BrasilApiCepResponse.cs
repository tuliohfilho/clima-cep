namespace ServiceIntegration.BrasilAPICEP.Responses;

public sealed class BrasilApiCepResponse
{
    public string Cep { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string Neighborhood { get; set; }
    public string Street { get; set; }
    public string Service { get; set; }

    public string Provider { get; set; }

    public LocationResponse Location { get; init; }
}