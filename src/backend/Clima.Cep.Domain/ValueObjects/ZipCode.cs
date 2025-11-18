using Clima.Cep.Domain.SeedWork;
using System.Text.RegularExpressions;

namespace Clima.Cep.Domain.ValueObjects;

public class ZipCode : ValueObject
{
    public string Value { get; private set; }
    public string NormalizedValue { get; private set; }

    private ZipCode() { }

    public ZipCode(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            throw new ArgumentException("ZipCode cannot be null or empty.", nameof(value));
        }

        Value = value.Trim();
        NormalizedValue = Normalize(Value);

        if (!IsValid(NormalizedValue)) {
            throw new ArgumentException($"Invalid ZipCode format: {value}. Must be 8 digits.", nameof(value));
        }
    }

    private static string Normalize(string zipCode) {
        return Regex.Replace(zipCode, @"[^\d]", "");
    }

    private static bool IsValid(string normalizedZipCode) {
        return normalizedZipCode.Length == 8 && Regex.IsMatch(normalizedZipCode, @"^\d{8}$");
    }

    public static implicit operator string(ZipCode zipCode) => zipCode.Value;
    public static explicit operator ZipCode(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents() {
        yield return NormalizedValue;
    }

    public override string ToString() => NormalizedValue;
}