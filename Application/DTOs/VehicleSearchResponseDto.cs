namespace MilesCarRental.Application.DTOs;

public sealed class VehicleSearchResponseDto
{
    public string Market { get; set; } = default!;
    public LocationDto Pickup { get; set; } = default!;
    public LocationDto Return { get; set; } = default!;
    public object Criteria { get; set; } = default!;
    public PagingDto Paging { get; set; } = default!;
    public List<VehicleItemDto> Vehicles { get; set; } = new();
}

public sealed class PagingDto
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
