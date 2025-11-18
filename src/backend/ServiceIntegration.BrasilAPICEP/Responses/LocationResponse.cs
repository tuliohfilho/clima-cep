namespace ServiceIntegration.BrasilAPICEP.Responses;

public class LocationResponse
{
    public string Type { get; set; }

    public CoordinatesResponse Coordinates { get; set; }
}