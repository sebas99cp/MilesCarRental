namespace MilesCarRental.Domain.Repositories;

using MilesCarRental.Domain.Entities;

public interface IVehicleRepository
{
    Task<IReadOnlyList<Vehicle>> ListAllAsync(CancellationToken ct = default);
}
