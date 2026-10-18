using System.Xml.Linq;
using Novolis.Inspection;

namespace Novolis.Inspection.Xml;

/// <summary>Formats XML into an inspection tree.</summary>
public sealed class XmlInspectionFormatter : IInspectionFormatter<string>
{
    /// <inheritdoc />
    public InspectionDocument Format(string value, string? title = null)
    {
        ArgumentNullException.ThrowIfNull(value);
        var root = XElement.Parse(value, LoadOptions.PreserveWhitespace);
        return new InspectionDocument(title ?? "XML", Build(root), "application/xml");
    }

    private static InspectionNode Build(XElement element)
    {
        var children = new List<InspectionNode>();
        children.AddRange(element.Attributes().Select(attribute =>
            InspectionNode.Scalar($"@{attribute.Name.LocalName}", attribute.Value)));
        children.AddRange(element.Elements().Select(Build));
        if (!string.IsNullOrWhiteSpace(element.Value) && !element.Elements().Any())
        {
            children.Add(InspectionNode.Scalar("#text", element.Value));
        }

        return new InspectionNode(
            element.Name.LocalName,
            InspectionValueKind.Object,
            Children: children);
    }
}
