namespace MilesCarRental.Infrastructure.InMemory;

using MilesCarRental.Domain.Entities;
using MilesCarRental.Domain.Repositories;

public sealed class InMemoryVehicleRepository : IVehicleRepository
{
    private readonly List<Vehicle> _items;
    public InMemoryVehicleRepository(List<Vehicle> items) => _items = items;
    public Task<IReadOnlyList<Vehicle>> ListAllAsync(CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<Vehicle>)_items);
}
