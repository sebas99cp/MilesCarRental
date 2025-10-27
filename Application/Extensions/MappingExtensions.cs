namespace MilesCarRental.Application.Extensions;

using MilesCarRental.Application.DTOs;
using MilesCarRental.Domain.Entities;

public static class MappingExtensions
{
    public static LocationDto ToDto(this Location location)
        => new()
        {
            MunicipioId = location.MunicipioId,
            DepartamentoId = location.DepartamentoId,
            Nombre = location.Nombre
        };

    public static VehicleItemDto ToItemDto(this Vehicle vehicle, bool allowsDifferentDropoff)
      => new()
      {
          Id = vehicle.Id,
          ClassCode = vehicle.ClassCode,
          ClassName = vehicle.ClassName,
          Seats = vehicle.Seats,
          Transmission = vehicle.Transmission,
          Category = vehicle.Category,
          AllowsDifferentDropoff = allowsDifferentDropoff
      };
}
