namespace Tests;

using FluentAssertions;
using MilesCarRental.Application.DTOs;
using MilesCarRental.Application.Services;
using MilesCarRental.Domain.Entities;
using MilesCarRental.Domain.Exceptions;
using MilesCarRental.Infrastructure.InMemory;
using Xunit;

public class SearchServiceTests
{
    private readonly SearchService _searchService;
    private readonly List<Location> _locations;
    private readonly List<Vehicle> _vehicles;

    public SearchServiceTests()
    {
        // Setup test data
        _locations = new List<Location>
        {
     new("YOPAL", "Yopal", "CASANARE"),
            new("AGUAZUL", "Aguazul", "CASANARE"),
          new("TAURAMENA", "Tauramena", "CASANARE"),
            new("BOGOTA", "Bogotá", "CUNDINAMARCA")
   };

        _vehicles = new List<Vehicle>
        {
        // Available vehicles
         new("VH-001", "ECON", "Económico", 5, "AT", "Car", new() { "CASANARE" }, new() { "YOPAL", "AGUAZUL" }, true),
      new("VH-002", "SUV", "SUV", 5, "AT", "SUV", new() { "CASANARE" }, new() { "YOPAL", "TAURAMENA" }, true),
            new("VH-003", "ECON", "Económico", 5, "AT", "Car", new() { "CASANARE", "CUNDINAMARCA" }, new() { "YOPAL", "BOGOTA" }, true),
            new("VH-004", "LUX", "Lujo", 5, "AT", "Car", new() { "CUNDINAMARCA" }, new() { "BOGOTA" }, true),
    // Unavailable vehicle (should be filtered out)
    new("VH-005", "ECON", "Económico", 5, "AT", "Car", new() { "CASANARE" }, new() { "YOPAL" }, false),
        };

        // Setup repositories
        var locationRepo = new InMemoryLocationRepository(_locations);
        var vehicleRepo = new InMemoryVehicleRepository(_vehicles);
        var marketResolver = new MarketResolver();

        _searchService = new SearchService(locationRepo, vehicleRepo, marketResolver);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnOnlyAvailableVehicles()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE"
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Assert
        result.Vehicles.Should().NotBeEmpty();
        // Check all returned vehicles are available
        foreach (var v in result.Vehicles)
        {
            var vehicle = _vehicles.First(x => x.Id == v.Id);
            vehicle.IsAvailable.Should().BeTrue();
        }
        result.Vehicles.Should().NotContain(v => v.Id == "VH-005"); // Unavailable vehicle
    }

    [Fact]
    public async Task SearchAsync_ShouldFilterByMarketDepartment()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE"
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Assert
        result.Market.Should().Be("CASANARE");
        result.Vehicles.Should().NotBeEmpty();
        // Check all returned vehicles operate in CASANARE market
        foreach (var v in result.Vehicles)
        {
            var vehicle = _vehicles.First(x => x.Id == v.Id);
            vehicle.MarketDepartments.Should().Contain(x => x.Equals("CASANARE", StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public async Task SearchAsync_ShouldFilterByPickupMunicipio()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE"
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Assert
        result.Vehicles.Should().NotBeEmpty();
        // Check all returned vehicles operate in YOPAL
        foreach (var v in result.Vehicles)
        {
            var vehicle = _vehicles.First(x => x.Id == v.Id);
            vehicle.Municipalities.Should().Contain(m => m.Equals("YOPAL", StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public async Task SearchAsync_ShouldFilterByReturnMunicipio_WhenDifferentFromPickup()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE",
            ReturnLocation = "AGUAZUL" // Different from pickup
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Assert
        result.Vehicles.Should().NotBeEmpty();
        // Check all returned vehicles operate in both locations
        foreach (var v in result.Vehicles)
        {
            var vehicle = _vehicles.First(x => x.Id == v.Id);
            vehicle.Municipalities.Should().Contain(m => m.Equals("YOPAL", StringComparison.OrdinalIgnoreCase));
            vehicle.Municipalities.Should().Contain(m => m.Equals("AGUAZUL", StringComparison.OrdinalIgnoreCase));
        }
        result.Vehicles.Should().Contain(v => v.Id == "VH-001"); // Operates in both YOPAL and AGUAZUL
        result.Vehicles.Should().NotContain(v => v.Id == "VH-002"); // Only operates in YOPAL and TAURAMENA
    }

    [Fact]
    public async Task SearchAsync_ShouldSetAllowsDifferentDropoff_WhenReturnDifferent()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE",
            ReturnLocation = "AGUAZUL"
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Assert
        result.Vehicles.Should().Contain(v => v.Id == "VH-001");
        var vehicle = result.Vehicles.First(v => v.Id == "VH-001");
        vehicle.AllowsDifferentDropoff.Should().BeTrue();
    }

    [Fact]
    public async Task SearchAsync_ShouldFilterByClassCode_WhenProvided()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE",
            ClassCode = "SUV"
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Assert
        result.Vehicles.Should().NotBeEmpty();
        result.Vehicles.Should().OnlyContain(v => v.ClassCode.Equals("SUV", StringComparison.OrdinalIgnoreCase));
        result.Vehicles.Should().Contain(v => v.Id == "VH-002");
    }

    [Fact]
    public async Task SearchAsync_ShouldNotFilterByClassCode_WhenNotProvided()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE"
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Assert
        result.Vehicles.Should().NotBeEmpty();
        var classCodes = result.Vehicles.Select(v => v.ClassCode).Distinct();
        classCodes.Should().HaveCountGreaterThan(1); // Multiple class codes
    }

    [Fact]
    public async Task SearchAsync_ShouldPaginateCorrectly()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE",
            Page = 1,
            PageSize = 2
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Assert
        result.Vehicles.Should().HaveCount(2);
        result.Paging.Page.Should().Be(1);
        result.Paging.PageSize.Should().Be(2);
        result.Paging.Total.Should().BeGreaterThanOrEqualTo(2);
        result.Paging.TotalPages.Should().Be((int)Math.Ceiling(result.Paging.Total / (double)result.Paging.PageSize));
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnSecondPage()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE",
            Page = 2,
            PageSize = 2
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Assert
        result.Paging.Page.Should().Be(2);
        result.Paging.PageSize.Should().Be(2);
    }

    [Fact]
    public async Task SearchAsync_ShouldClampPageSizeTo100()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE",
            PageSize = 200 // Exceeds max
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Assert
        result.Paging.PageSize.Should().BeLessThanOrEqualTo(100);
    }

    [Fact]
    public async Task SearchAsync_ShouldDefaultToPage1_WhenPageIsZeroOrNegative()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE",
            Page = 0
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Assert
        result.Paging.Page.Should().Be(1);
    }

    [Fact]
    public async Task SearchAsync_ShouldThrowException_WhenPickupLocationNotFound()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "INVALID",
            PickupDepartment = "CASANARE"
        };

        // Act
        Func<Task> act = async () => await _searchService.SearchAsync(request);

        // Assert
        await act.Should().ThrowAsync<LocationNotFoundException>()
            .WithMessage("*INVALID*");
    }

    [Fact]
    public async Task SearchAsync_ShouldThrowException_WhenReturnLocationNotFound()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE",
            ReturnLocation = "INVALID"
        };

        // Act
        Func<Task> act = async () => await _searchService.SearchAsync(request);

        // Assert
        await act.Should().ThrowAsync<LocationNotFoundException>()
       .WithMessage("*INVALID*");
    }

    [Fact]
    public async Task SearchAsync_ShouldThrowException_WhenDepartmentDoesNotMatch()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CUNDINAMARCA" // Wrong department
        };

        // Act
        Func<Task> act = async () => await _searchService.SearchAsync(request);

        // Assert
        await act.Should().ThrowAsync<DepartmentMismatchException>()
               .WithMessage("*CUNDINAMARCA*CASANARE*");
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnCorrectPickupAndReturnLocations()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE",
            ReturnLocation = "AGUAZUL"
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Assert
        result.Pickup.MunicipioId.Should().Be("YOPAL");
        result.Pickup.DepartamentoId.Should().Be("CASANARE");
        result.Pickup.Nombre.Should().Be("Yopal");

        result.Return.MunicipioId.Should().Be("AGUAZUL");
        result.Return.DepartamentoId.Should().Be("CASANARE");
        result.Return.Nombre.Should().Be("Aguazul");
    }

    [Fact]
    public async Task SearchAsync_ShouldMapVehicleDetailsCorrectly()
    {
        // Arrange
        var request = new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE",
            ClassCode = "ECON"
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Assert
        result.Vehicles.Should().NotBeEmpty();
        var vehicle = result.Vehicles.First();
        vehicle.ClassCode.Should().Be("ECON");
        vehicle.ClassName.Should().Be("Económico");
        vehicle.Seats.Should().Be(5);
        vehicle.Transmission.Should().Be("AT");
        vehicle.Category.Should().Be("Car");
    }
}
