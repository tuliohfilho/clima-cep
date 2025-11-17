using Clima.Cep.Domain.SeedWork;
using Clima.Cep.Domain.ValueObjects;

namespace Clima.Cep.Domain.Entities;

public class ZipCodeLookup : Entity
{
    public ZipCode ZipCode { get; private set; }
    public string Street { get; private set; }
    public string District { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Ibge { get; private set; }
    public Location Location { get; private set; }
    public string Provider { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private ZipCodeLookup() { }

    public ZipCodeLookup(
        ZipCode zipCode,
        string street,
        string district,
        string city,
        string state,
        string ibge,
        Location location,
        string provider) {
        ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));
        Street = street;
        District = district;
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state ?? throw new ArgumentNullException(nameof(state));
        Ibge = ibge;
        Location = location;
        Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static ZipCodeLookup Create(
        string zipCode,
        string street,
        string district,
        string city,
        string state,
        string ibge,
        double? lat,
        double? lon,
        string provider) {
        var location = (lat.HasValue && lon.HasValue) ? new Location(lat.Value, lon.Value) : null;

        return new ZipCodeLookup(
            (ZipCode)zipCode,
            street,
            district,
            city,
            state,
            ibge,
            location,
            provider);
    }
}