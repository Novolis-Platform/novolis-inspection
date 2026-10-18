namespace Novolis.Inspection;

/// <summary>Semantic kind of an inspection node.</summary>
public enum InspectionValueKind
{
    /// <summary>A complete inspection document.</summary>
    Document,
    /// <summary>An object value.</summary>
    Object,
    /// <summary>A collection value.</summary>
    Collection,
    /// <summary>A named property.</summary>
    Property,
    /// <summary>A scalar value.</summary>
    Scalar,
    /// <summary>A textual value.</summary>
    Text,
    /// <summary>An error value.</summary>
    Error,
}
