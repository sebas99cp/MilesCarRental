namespace MilesCarRental.Application.Models;

using MilesCarRental.Domain.Entities;

public sealed class ValidatedSearchRequest
{
    public required Location Pickup { get; init; }
    public required Location Return { get; init; }
    public required string Market { get; init; }
    public string? ClassCode { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}
