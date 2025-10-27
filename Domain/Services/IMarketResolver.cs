namespace MilesCarRental.Domain.Services;

using MilesCarRental.Domain.Entities;

public interface IMarketResolver
{
    // Para esta prueba: market = pickup.DepartamentoId
    string ResolveMarket(Location pickup);
}
