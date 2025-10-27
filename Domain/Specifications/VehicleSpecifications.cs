namespace MilesCarRental.Domain.Specifications;

using MilesCarRental.Domain.Entities;

public static class VehicleSpecifications
{
    public static bool IsAvailable(this Vehicle vehicle)
        => vehicle.IsAvailable;

    public static bool OperatesInMarket(this Vehicle vehicle, string market)
 => vehicle.MarketDepartments.Contains(market, StringComparer.OrdinalIgnoreCase);

    public static bool OperatesInMunicipality(this Vehicle vehicle, string municipality)
        => vehicle.Municipalities.Contains(municipality, StringComparer.OrdinalIgnoreCase);

    public static bool SupportsOneWayRental(this Vehicle vehicle, string pickup, string returnLocation)
        => string.Equals(pickup, returnLocation, StringComparison.OrdinalIgnoreCase)
 || vehicle.OperatesInMunicipality(returnLocation);

    public static bool MatchesClassCode(this Vehicle vehicle, string? classCode)
     => string.IsNullOrWhiteSpace(classCode)
             || string.Equals(vehicle.ClassCode, classCode, StringComparison.OrdinalIgnoreCase);
}
