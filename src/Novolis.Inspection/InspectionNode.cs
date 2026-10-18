namespace Novolis.Inspection;

/// <summary>Immutable node in an inspection tree.</summary>
public sealed record InspectionNode(
    string Name,
    InspectionValueKind Kind,
    string? Value = null,
    IReadOnlyList<InspectionNode>? Children = null)
{
    /// <summary>Child nodes, or an empty collection.</summary>
    public IReadOnlyList<InspectionNode> Items { get; } = Children ?? [];

    /// <summary>Creates a scalar node.</summary>
    public static InspectionNode Scalar(string name, object? value) =>
        new(name, value is string ? InspectionValueKind.Text : InspectionValueKind.Scalar, value?.ToString());
}
