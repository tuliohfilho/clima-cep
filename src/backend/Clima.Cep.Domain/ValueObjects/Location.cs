using Clima.Cep.Domain.SeedWork;

namespace Clima.Cep.Domain.ValueObjects;

public class Location : ValueObject
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    private Location() { }

    public Location(double latitude, double longitude) {
        Latitude = latitude;
        Longitude = longitude;
    }

    protected override IEnumerable<object> GetEqualityComponents() {
        yield return Latitude;
        yield return Longitude;
    }

    public override string ToString() => $"(Lat: {Latitude}, Lon: {Longitude})";
}