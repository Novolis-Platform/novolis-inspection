using System.Text.Json;
using Novolis.Inspection;

namespace Novolis.Inspection.Json;

/// <summary>Formats JSON into an inspection tree.</summary>
public sealed class JsonInspectionFormatter : IInspectionFormatter<string>
{
    /// <inheritdoc />
    public InspectionDocument Format(string value, string? title = null)
    {
        ArgumentNullException.ThrowIfNull(value);
        using var document = JsonDocument.Parse(value);
        return new InspectionDocument(
            title ?? "JSON",
            Build(document.RootElement, "$"),
            "application/json");
    }

    private static InspectionNode Build(JsonElement element, string name) =>
        element.ValueKind switch
        {
            JsonValueKind.Object => new InspectionNode(
                name,
                InspectionValueKind.Object,
                Children: element.EnumerateObject()
                    .Select(property => Build(property.Value, property.Name))
                    .ToArray()),
            JsonValueKind.Array => new InspectionNode(
                name,
                InspectionValueKind.Collection,
                Children: element.EnumerateArray()
                    .Select((item, index) => Build(item, $"[{index}]"))
                    .ToArray()),
            JsonValueKind.String => InspectionNode.Scalar(name, element.GetString()),
            JsonValueKind.Null => InspectionNode.Scalar(name, null),
            _ => InspectionNode.Scalar(name, element.GetRawText()),
        };
}
