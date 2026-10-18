namespace Novolis.Inspection;

/// <summary>Formats a source value into an inspection document.</summary>
public interface IInspectionFormatter<in T>
{
    /// <summary>Formats a value.</summary>
    InspectionDocument Format(T value, string? title = null);
}
