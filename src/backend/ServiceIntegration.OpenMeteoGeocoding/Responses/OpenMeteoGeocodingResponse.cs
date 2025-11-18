namespace ServiceIntegration.OpenMeteoGeocoding.Responses;

public class OpenMeteoGeocodingResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("results")]
    public List<GeocodingResult> Results { get; set; }

    public class GeocodingResult
    {
        [System.Text.Json.Serialization.JsonPropertyName("latitude")]
        public decimal Latitude { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("longitude")]
        public decimal Longitude { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("admin1")]
        public string State { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("country")]
        public string Country { get; set; }
    }
}