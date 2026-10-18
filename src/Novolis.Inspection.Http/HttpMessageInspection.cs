namespace Novolis.Inspection.Http;

/// <summary>Materialized HTTP message used by the inspection formatter.</summary>
public sealed record HttpMessageInspection(
    string Direction,
    string Method,
    string Uri,
    int? StatusCode,
    IReadOnlyDictionary<string, string[]> Headers,
    string? Body);
