namespace MilesCarRental.Domain.Exceptions;

public sealed class LocationNotFoundException : DomainException
{
    public override int StatusCode => 404;
    public override string ErrorCode => "LOCATION_NOT_FOUND";

    public string MunicipioId { get; }

    public LocationNotFoundException(string municipioId)
        : base($"Location with MunicipioId '{municipioId}' was not found.")
    {
        MunicipioId = municipioId;
    }

    public LocationNotFoundException(string municipioId, Exception innerException)
        : base($"Location with MunicipioId '{municipioId}' was not found.", innerException)
    {
        MunicipioId = municipioId;
    }
}
