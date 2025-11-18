namespace ServiceIntegration.OpenMeteoGeocoding.Interfaces;

public interface IOpenMeteoGeocodingClient
{
    Task<(decimal? Latitude, decimal? Longitude)> SearchAsync(string city, string state);
}