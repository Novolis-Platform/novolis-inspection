namespace Novolis.Inspection;

/// <summary>Host-neutral structured inspection document.</summary>
public sealed record InspectionDocument(
    string Title,
    InspectionNode Root,
    string? SourceType = null);
