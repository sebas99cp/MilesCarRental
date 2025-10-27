namespace Tests;

using FluentAssertions;
using MilesCarRental.Domain.Entities;
using MilesCarRental.Infrastructure.InMemory;
using Xunit;

public class MarketResolverTests
{
    [Fact]
    public void ResolveMarket_ShouldReturnDepartamentoId_WhenPickupLocationIsProvided()
    {
        // Arrange
        var resolver = new MarketResolver();
        var pickup = new Location("YOPAL", "Yopal", "CASANARE");

        // Act
        var market = resolver.ResolveMarket(pickup);

        // Assert
        market.Should().Be("CASANARE");
    }

    [Fact]
    public void ResolveMarket_ShouldReturnCorrectMarket_ForDifferentDepartments()
    {
        // Arrange
        var resolver = new MarketResolver();
        var pickup1 = new Location("BOGOTA", "Bogotá", "CUNDINAMARCA");
        var pickup2 = new Location("MEDELLIN", "Medellín", "ANTIOQUIA");

        // Act
        var market1 = resolver.ResolveMarket(pickup1);
        var market2 = resolver.ResolveMarket(pickup2);

        // Assert
        market1.Should().Be("CUNDINAMARCA");
        market2.Should().Be("ANTIOQUIA");
    }
}
