namespace MilesCarRental.Api.Swagger;

using MilesCarRental.Application.DTOs;
using Swashbuckle.AspNetCore.Filters;

public class VehicleSearchRequestExample : IExamplesProvider<VehicleSearchRequestDto>
{
    public VehicleSearchRequestDto GetExamples()
    {
        return new VehicleSearchRequestDto
        {
            PickupLocation = "YOPAL",
            PickupDepartment = "CASANARE",
            ReturnLocation = "AGUAZUL",
            ClassCode = "SUV",
            Page = 1,
            PageSize = 20
        };
    }
}

public class VehicleSearchResponseExample : IExamplesProvider<VehicleSearchResponseDto>
{
    public VehicleSearchResponseDto GetExamples()
    {
        return new VehicleSearchResponseDto
        {
            Market = "CASANARE",
            Pickup = new LocationDto
            {
                MunicipioId = "YOPAL",
                DepartamentoId = "CASANARE",
                Nombre = "Yopal"
            },
            Return = new LocationDto
            {
                MunicipioId = "AGUAZUL",
                DepartamentoId = "CASANARE",
                Nombre = "Aguazul"
            },
            Criteria = new
            {
                ClassCode = "SUV",
                Page = 1,
                PageSize = 20
            },
            Paging = new PagingDto
            {
                Total = 2,
                Page = 1,
                PageSize = 20,
                TotalPages = 1
            },
            Vehicles = new List<VehicleItemDto>
       {
     new()
      {
        Id = "VH-009",
  ClassCode = "SUV",
    ClassName = "SUV Compacto",
       Seats = 5,
Transmission = "AT",
          Category = "SUV",
           AllowsDifferentDropoff = true
     },
       new()
    {
Id = "VH-012",
        ClassCode = "SUV",
     ClassName = "SUV Grande",
    Seats = 7,
   Transmission = "AT",
     Category = "SUV",
          AllowsDifferentDropoff = false
             }
   }
        };
    }
}
