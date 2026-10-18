using Novolis.Inspection;

namespace Novolis.Inspection.Http;

/// <summary>Formats materialized HTTP messages into an inspection tree.</summary>
public sealed class HttpInspectionFormatter : IInspectionFormatter<HttpMessageInspection>
{
    /// <inheritdoc />
    public InspectionDocument Format(HttpMessageInspection value, string? title = null)
    {
        ArgumentNullException.ThrowIfNull(value);
        var headerNodes = value.Headers
            .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .Select(pair => new InspectionNode(
                pair.Key,
                InspectionValueKind.Property,
                Children: pair.Value.Select(item => InspectionNode.Scalar("value", item)).ToArray()))
            .ToArray();
        var children = new List<InspectionNode>
        {
            InspectionNode.Scalar("direction", value.Direction),
            InspectionNode.Scalar("method", value.Method),
            InspectionNode.Scalar("uri", value.Uri),
            value.StatusCode is null
                ? InspectionNode.Scalar("status", null)
                : InspectionNode.Scalar("status", value.StatusCode),
            new("headers", InspectionValueKind.Object, Children: headerNodes),
        };
        if (value.Body is not null)
        {
            children.Add(InspectionNode.Scalar("body", value.Body));
        }

        return new InspectionDocument(title ?? "HTTP message", new InspectionNode(
            "http",
            InspectionValueKind.Document,
            Children: children), "message/http");
    }

    /// <summary>Materializes and formats an HTTP request without blocking.</summary>
    public async ValueTask<InspectionDocument> FormatAsync(
        HttpRequestMessage request,
        string? title = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var body = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);
        var headers = request.Headers
            .Concat(request.Content is null
                ? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>()
                : request.Content.Headers)
            .GroupBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.SelectMany(pair => pair.Value).ToArray(),
                StringComparer.OrdinalIgnoreCase);
        return Format(new HttpMessageInspection(
            "request",
            request.Method.Method,
            request.RequestUri?.ToString() ?? string.Empty,
            null,
            headers,
            body), title);
    }
}
