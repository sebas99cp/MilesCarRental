namespace MilesCarRental.Domain.Exceptions;

public sealed class DepartmentMismatchException : DomainException
{
    public override int StatusCode => 400;
    public override string ErrorCode => "DEPARTMENT_MISMATCH";

    public string ProvidedDepartment { get; }
    public string ActualDepartment { get; }
    public string MunicipioId { get; }

    public DepartmentMismatchException(string providedDepartment, string actualDepartment, string municipioId)
        : base($"The provided department '{providedDepartment}' does not match the municipality '{municipioId}' which belongs to '{actualDepartment}'.")
    {
        ProvidedDepartment = providedDepartment;
        ActualDepartment = actualDepartment;
        MunicipioId = municipioId;
    }
}
