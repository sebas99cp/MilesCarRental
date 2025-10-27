namespace MilesCarRental.Domain.Exceptions;

/// <summary>
/// Base exception for all domain-specific exceptions.
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// HTTP status code that should be returned for this exception.
    /// </summary>
    public abstract int StatusCode { get; }

    /// <summary>
    /// Error code for client-side handling.
    /// </summary>
    public abstract string ErrorCode { get; }

    protected DomainException(string message) : base(message)
    {
  }

    protected DomainException(string message, Exception innerException) 
     : base(message, innerException)
    {
    }
}
