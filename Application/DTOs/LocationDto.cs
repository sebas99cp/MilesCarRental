namespace MilesCarRental.Application.DTOs;

public sealed class LocationDto
{
    public string MunicipioId { get; set; } = default!;
    public string DepartamentoId { get; set; } = default!;
    public string Nombre { get; set; } = default!;
}
