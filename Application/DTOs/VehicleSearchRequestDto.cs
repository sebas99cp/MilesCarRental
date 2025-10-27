namespace MilesCarRental.Application.DTOs;

public sealed class VehicleSearchRequestDto
{
    public string PickupLocation { get; set; } = default!;
    public string? ReturnLocation { get; set; }
    public string? ClassCode { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
