namespace MilesCarRental.Controllers;

using Microsoft.AspNetCore.Mvc;
using MilesCarRental.Api.Filters;
using MilesCarRental.Api.Models;
using MilesCarRental.Application.DTOs;
using MilesCarRental.Application.Services;

[ApiController]
[Route("api/v1/vehicles")]
[ServiceFilter(typeof(ValidationFilter))]
public class VehiclesController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(ISearchService searchService, ILogger<VehiclesController> logger)
    {
        _searchService = searchService;
   _logger = logger;
    }

    /// <summary>
  /// Search for available vehicles based on pickup/return locations and optional filters.
    /// </summary>
  /// <param name="request">Search criteria including locations, dates, and filters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of available vehicles matching the search criteria.</returns>
    /// <response code="200">Returns the list of available vehicles.</response>
    /// <response code="400">If the request parameters are invalid.</response>
    /// <response code="404">If the specified location is not found.</response>
    /// <response code="422">If validation fails.</response>
    /// <response code="429">If rate limit is exceeded.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(VehicleSearchResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<VehicleSearchResponseDto>> Search(
        [FromQuery] VehicleSearchRequestDto request,
 CancellationToken ct = default)
    {
  _logger.LogInformation(
    "Vehicle search started: Pickup={Pickup}, Department={Department}, ReturnLocation={Return}, ClassCode={ClassCode}",
    request.PickupLocation,
    request.PickupDepartment,
       request.ReturnLocation ?? "Same as pickup",
  request.ClassCode ?? "All classes");

        var result = await _searchService.SearchAsync(request, ct);

     _logger.LogInformation(
       "Vehicle search completed: Found {Count} vehicles in {Market} market, Page {Page}/{TotalPages}",
    result.Vehicles.Count,
     result.Market,
     result.Paging.Page,
result.Paging.TotalPages);

        return Ok(result);
    }
}
