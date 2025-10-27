namespace MilesCarRental.Application.Models;

public sealed class PaginatedList<T>
{
    public required List<T> Items { get; init; }
    public required int Total { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalPages { get; init; }
}
