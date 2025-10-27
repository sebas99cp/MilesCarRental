namespace MilesCarRental.Infrastructure.InMemory;

using MilesCarRental.Domain.Entities;
using MilesCarRental.Domain.Services;

public sealed class MarketResolver : IMarketResolver
{
    public string ResolveMarket(Location pickup) => pickup.DepartamentoId;
}
