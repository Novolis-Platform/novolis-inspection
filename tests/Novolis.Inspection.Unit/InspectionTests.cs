using Novolis.Inspection.Http;
using Novolis.Inspection.Json;
using Novolis.Inspection.ObjectGraph;
using Novolis.Inspection.Xml;

namespace Novolis.Inspection.Unit;

public sealed class InspectionTests
{
    [Test]
    public async Task Json_formatter_builds_nested_tree()
    {
        var document = new JsonInspectionFormatter().Format("""{"items":[1,2]}""");

        await Assert.That(document.Root.Items.Single(item => item.Name == "items").Items).Count().IsEqualTo(2);
    }

    [Test]
    public async Task Xml_formatter_includes_attributes_and_text()
    {
        var document = new XmlInspectionFormatter().Format("""<root id="1">value</root>""");

        await Assert.That(document.Root.Items).Count().IsEqualTo(2);
    }

    [Test]
    public async Task Object_formatter_marks_cycles()
    {
        var value = new Circular();
        value.Next = value;

        var document = new ObjectGraphInspectionFormatter().Format(value);

        await Assert.That(document.Root.Items.Single(item => item.Name == "Next").Value).IsEqualTo("cycle");
    }

    [Test]
    public async Task Http_formatter_exposes_headers_and_body()
    {
        var document = new HttpInspectionFormatter().Format(new HttpMessageInspection(
            "request",
            "GET",
            "https://example.test",
            null,
            new Dictionary<string, string[]> { ["X-Test"] = ["one"] },
            "body"));

        await Assert.That(document.Root.Items.Any(item => item.Name == "body")).IsTrue();
    }

    private sealed class Circular
    {
        public Circular? Next { get; set; }
    }
}
