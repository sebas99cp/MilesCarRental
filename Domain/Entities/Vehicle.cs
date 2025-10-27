namespace MilesCarRental.Domain.Entities;

public sealed record Vehicle(
    string Id,
    string ClassCode,
    string ClassName,
    int Seats,
    string Transmission,
    string Category,
    List<string> MarketDepartments,
    List<string> Municipalities,
    bool IsAvailable
);
