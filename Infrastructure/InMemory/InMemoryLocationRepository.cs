namespace MilesCarRental.Infrastructure.InMemory;

using MilesCarRental.Domain.Entities;
using MilesCarRental.Domain.Repositories;

public sealed class InMemoryLocationRepository : ILocationRepository
{
    private readonly List<Location> _items;
    public InMemoryLocationRepository(List<Location> items) => _items = items;

    public Task<Location?> GetByMunicipioIdAsync(string municipioId, CancellationToken ct = default)
        => Task.FromResult(_items.FirstOrDefault(x => string.Equals(x.MunicipioId, municipioId, StringComparison.OrdinalIgnoreCase)));
}
