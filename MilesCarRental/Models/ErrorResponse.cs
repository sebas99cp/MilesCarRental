namespace MilesCarRental.Api.Models;

/// <summary>
/// Represents an error response following RFC 7807 Problem Details standard.
/// </summary>
public sealed class ErrorResponse
{
    /// <summary>
    /// A URI reference that identifies the problem type.
    /// </summary>
    public string Type { get; set; } = "about:blank";

    /// <summary>
    /// A short, human-readable summary of the problem type.
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// The HTTP status code.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// A human-readable explanation specific to this occurrence of the problem.
    /// </summary>
    public string Detail { get; set; } = default!;

    /// <summary>
    /// A URI reference that identifies the specific occurrence of the problem.
    /// </summary>
    public string Instance { get; set; } = default!;

    /// <summary>
    /// Error code for programmatic handling.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Timestamp when the error occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Additional error-specific data.
    /// </summary>
    public Dictionary<string, object>? Extensions { get; set; }

    /// <summary>
    /// Trace ID for correlation across services.
    /// </summary>
    public string? TraceId { get; set; }
}
