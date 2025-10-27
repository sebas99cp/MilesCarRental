namespace MilesCarRental.Application.Services;

using MilesCarRental.Application.DTOs;
using MilesCarRental.Application.Extensions;
using MilesCarRental.Application.Models;
using MilesCarRental.Domain.Entities;
using MilesCarRental.Domain.Exceptions;
using MilesCarRental.Domain.Repositories;
using MilesCarRental.Domain.Services;
using MilesCarRental.Domain.Specifications;

public sealed class SearchService : ISearchService
{
    private readonly ILocationRepository _locations;
    private readonly IVehicleRepository _vehicles;
    private readonly IMarketResolver _marketResolver;

    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;

    public SearchService(ILocationRepository locations, IVehicleRepository vehicles, IMarketResolver marketResolver)
    {
        _locations = locations;
        _vehicles = vehicles;
        _marketResolver = marketResolver;
    }

    public async Task<VehicleSearchResponseDto> SearchAsync(VehicleSearchRequestDto request, CancellationToken ct = default)
    {
        // 1. Validar y normalizar la solicitud
        var validatedRequest = await ValidateAndNormalizeRequestAsync(request, ct);

        // 2. Obtener vehículos filtrados
        var filteredVehicles = await GetFilteredVehiclesAsync(validatedRequest, ct);

        // 3. Aplicar paginación
        var paginatedResult = ApplyPagination(filteredVehicles, validatedRequest.Page, validatedRequest.PageSize);

        // 4. Mapear a DTO de respuesta
        return MapToResponse(paginatedResult, validatedRequest, request);
    }

    private async Task<ValidatedSearchRequest> ValidateAndNormalizeRequestAsync(
        VehicleSearchRequestDto request,
 CancellationToken ct)
    {
        // Validar y cargar ubicación de recogida
        var pickup = await _locations.GetByMunicipioIdAsync(request.PickupLocation, ct)
      ?? throw new LocationNotFoundException(request.PickupLocation);

        // Validar y cargar ubicación de devolución
        var returnLoc = await _locations.GetByMunicipioIdAsync(request.ReturnLocation!, ct)
  ?? throw new LocationNotFoundException(request.ReturnLocation!);

        // Resolver el mercado basado en las ubicaciones
        var market = _marketResolver.ResolveMarket(pickup);

        return new ValidatedSearchRequest
        {
            Pickup = pickup,
            Return = returnLoc,
            Market = market,
            ClassCode = request.ClassCode,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private async Task<List<Vehicle>> GetFilteredVehiclesAsync(
  ValidatedSearchRequest request,
      CancellationToken ct)
    {
        // Obtener todos los vehículos
        var allVehicles = await _vehicles.ListAllAsync(ct);

        // Aplicar filtros usando specifications
        var filteredVehicles = allVehicles
            .Where(v => v.IsAvailable())
 .Where(v => v.OperatesInMarket(request.Market))
            .Where(v => v.OperatesInMunicipality(request.Pickup.MunicipioId))
  .Where(v => v.SupportsOneWayRental(request.Pickup.MunicipioId, request.Return.MunicipioId))
            .Where(v => v.MatchesClassCode(request.ClassCode))
            .ToList();

        return filteredVehicles;
    }

    private static PaginatedList<Vehicle> ApplyPagination(
        List<Vehicle> vehicles,
        int requestedPage,
     int requestedPageSize)
    {
        var total = vehicles.Count;
        var page = Math.Max(1, requestedPage);
        var pageSize = Math.Clamp(requestedPageSize, MinPageSize, MaxPageSize);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        var pageItems = vehicles
       .Skip((page - 1) * pageSize)
     .Take(pageSize)
            .ToList();

        return new PaginatedList<Vehicle>
        {
            Items = pageItems,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    private static VehicleSearchResponseDto MapToResponse(
        PaginatedList<Vehicle> paginatedResult,
      ValidatedSearchRequest validatedRequest,
        VehicleSearchRequestDto originalRequest)
    {
        // Mapear vehículos a DTOs
        var vehicleDtos = paginatedResult.Items.Select(v =>
              {
                  var allowsDifferentDropoff = !string.Equals(
         validatedRequest.Pickup.MunicipioId,
            validatedRequest.Return.MunicipioId,
                  StringComparison.OrdinalIgnoreCase)
               && v.OperatesInMunicipality(validatedRequest.Return.MunicipioId);

                  return v.ToItemDto(allowsDifferentDropoff);
              }).ToList();

        return new VehicleSearchResponseDto
        {
            Market = validatedRequest.Market,
            Pickup = validatedRequest.Pickup.ToDto(),
            Return = validatedRequest.Return.ToDto(),
            Criteria = new
            {
                originalRequest.ClassCode,
                originalRequest.Page,
                originalRequest.PageSize
            },
            Paging = new PagingDto
            {
                Total = paginatedResult.Total,
                Page = paginatedResult.Page,
                PageSize = paginatedResult.PageSize,
                TotalPages = paginatedResult.TotalPages
            },
            Vehicles = vehicleDtos
        };
    }
}
