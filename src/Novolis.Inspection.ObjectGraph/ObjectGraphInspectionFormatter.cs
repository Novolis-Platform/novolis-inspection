using System.Collections;
using System.Reflection;
using Novolis.Inspection;

namespace Novolis.Inspection.ObjectGraph;

/// <summary>Formats arbitrary object graphs with cycle and depth protection.</summary>
public sealed class ObjectGraphInspectionFormatter : IInspectionFormatter<object?>
{
    /// <summary>Maximum property depth before a node is summarized.</summary>
    public int MaxDepth { get; init; } = 8;

    /// <inheritdoc />
    public InspectionDocument Format(object? value, string? title = null)
    {
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
        return new InspectionDocument(
            title ?? "Object",
            Build("root", value, 0, visited),
            value?.GetType().FullName);
    }

    private InspectionNode Build(
        string name,
        object? value,
        int depth,
        HashSet<object> visited)
    {
        if (value is null || IsScalar(value))
        {
            return InspectionNode.Scalar(name, value);
        }

        if (depth >= MaxDepth)
        {
            return new InspectionNode(name, InspectionValueKind.Object, "max depth reached");
        }

        if (!value.GetType().IsValueType && !visited.Add(value))
        {
            return new InspectionNode(name, InspectionValueKind.Object, "cycle");
        }

        if (value is IEnumerable sequence)
        {
            var items = new List<InspectionNode>();
            var index = 0;
            foreach (var item in sequence)
            {
                items.Add(Build($"[{index++}]", item, depth + 1, visited));
            }

            return new InspectionNode(name, InspectionValueKind.Collection, Children: items);
        }

        var properties = new List<InspectionNode>();
        foreach (var property in value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetIndexParameters().Length > 0 || property.GetMethod is null)
            {
                continue;
            }

            try
            {
                properties.Add(Build(property.Name, property.GetValue(value), depth + 1, visited));
            }
            catch (Exception exception) when (exception is not OutOfMemoryException)
            {
                properties.Add(new InspectionNode(
                    property.Name,
                    InspectionValueKind.Error,
                    exception.Message));
            }
        }

        return new InspectionNode(
            name,
            InspectionValueKind.Object,
            value.GetType().FullName,
            properties);
    }

    private static bool IsScalar(object value) =>
        value is string
            or char
            or bool
            or byte
            or sbyte
            or short
            or ushort
            or int
            or uint
            or long
            or ulong
            or float
            or double
            or decimal
            or Guid
            or DateTime
            or DateTimeOffset
            or TimeSpan
            || value.GetType().IsEnum;
}
