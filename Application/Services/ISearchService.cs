namespace MilesCarRental.Application.Services;

using MilesCarRental.Application.DTOs;

public interface ISearchService
{
    Task<VehicleSearchResponseDto> SearchAsync(VehicleSearchRequestDto request, CancellationToken ct = default);
}
