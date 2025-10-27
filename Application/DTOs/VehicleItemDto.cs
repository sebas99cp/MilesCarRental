namespace MilesCarRental.Application.DTOs;

public sealed class VehicleItemDto
{
    public string Id { get; set; } = default!;
    public string ClassCode { get; set; } = default!;
    public string ClassName { get; set; } = default!;
    public int Seats { get; set; }
    public string Transmission { get; set; } = default!;
    public string Category { get; set; } = default!;
    public bool AllowsDifferentDropoff { get; set; }
}
