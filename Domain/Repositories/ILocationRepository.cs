namespace MilesCarRental.Domain.Repositories;

using MilesCarRental.Domain.Entities;

public interface ILocationRepository
{
    Task<Location?> GetByMunicipioIdAsync(string municipioId, CancellationToken ct = default);
}
