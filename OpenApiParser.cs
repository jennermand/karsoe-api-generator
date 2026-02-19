using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace karsoe_api_generator;

/// <summary>
/// Parses OpenAPI specification files.
/// </summary>
public class OpenApiParser
{
    public OpenApiDocument Document { get; private set; } = null!;
    public string? ApiTitle { get; private set; }
    public string? ApiDescription { get; private set; }
    public string? BaseUrl { get; private set; }

    /// <summary>
    /// Loads and parses an OpenAPI specification file.
    /// </summary>
    public void LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"OpenAPI file not found: {filePath}");
        }

        Console.WriteLine($"Loading OpenAPI specification from: {filePath}");

        using var stream = File.OpenRead(filePath);
        var reader = new OpenApiStreamReader();
        var result = reader.Read(stream, out var diagnostic);

        if (diagnostic.Errors.Count > 0)
        {
            Console.WriteLine("Errors while reading OpenAPI document:");
            foreach (var error in diagnostic.Errors)
            {
                Console.WriteLine($"  - {error.Message}");
            }
            throw new InvalidOperationException("Failed to parse OpenAPI document.");
        }

        if (diagnostic.Warnings.Count > 0)
        {
            Console.WriteLine("Warnings while reading OpenAPI document:");
            foreach (var warning in diagnostic.Warnings)
            {
                Console.WriteLine($"  - {warning.Message}");
            }
        }

        Document = result;
        ApiTitle = Document.Info?.Title ?? "API";
        ApiDescription = Document.Info?.Description;
        BaseUrl = Document.Servers?.FirstOrDefault()?.Url;

        Console.WriteLine($"Successfully loaded: {ApiTitle}");
        Console.WriteLine($"  Version: {Document.Info?.Version}");
        Console.WriteLine($"  Base URL: {BaseUrl}");
        Console.WriteLine($"  Schemas: {Document.Components?.Schemas?.Count ?? 0}");
        Console.WriteLine($"  Paths: {Document.Paths?.Count ?? 0}");
    }

    /// <summary>
    /// Gets all schema definitions from the OpenAPI document.
    /// </summary>
    public IDictionary<string, OpenApiSchema> GetSchemas()
    {
        return Document.Components?.Schemas ?? new Dictionary<string, OpenApiSchema>();
    }

    /// <summary>
    /// Gets all path items (endpoints) from the OpenAPI document.
    /// </summary>
    public IDictionary<string, OpenApiPathItem> GetPaths()
    {
        return Document.Paths ?? new Dictionary<string, OpenApiPathItem>();
    }

    /// <summary>
    /// Gets all operations grouped by tag.
    /// </summary>
    public Dictionary<string, List<OperationInfo>> GetOperationsByTag()
    {
        var operationsByTag = new Dictionary<string, List<OperationInfo>>();

        foreach (var path in GetPaths())
        {
            foreach (var operation in path.Value.Operations)
            {
                var operationInfo = new OperationInfo
                {
                    Path = path.Key,
                    Method = operation.Key,
                    Operation = operation.Value,
                    OperationId = operation.Value.OperationId
                };

                var tags = operation.Value.Tags?.Select(t => t.Name).ToList() ?? new List<string> { "Default" };

                foreach (var tag in tags)
                {
                    if (!operationsByTag.ContainsKey(tag))
                    {
                        operationsByTag[tag] = new List<OperationInfo>();
                    }
                    operationsByTag[tag].Add(operationInfo);
                }
            }
        }

        return operationsByTag;
    }
}

/// <summary>
/// Information about an API operation (endpoint).
/// </summary>
public class OperationInfo
{
    public required string Path { get; set; }
    public OperationType Method { get; set; }
    public required OpenApiOperation Operation { get; set; }
    public string? OperationId { get; set; }

    /// <summary>
    /// Generates a method name for this operation.
    /// </summary>
    public string GetMethodName(bool includeAsyncSuffix = true, bool useHttpMethodNames = false)
    {
        if (!string.IsNullOrWhiteSpace(OperationId))
        {
            var sanitizedName = TypeMapper.SanitizeClassName(OperationId);
            return includeAsyncSuffix && !sanitizedName.EndsWith("Async") ? sanitizedName + "Async" : sanitizedName;
        }

        // Generate name from HTTP method and path
        var pathParts = Path.Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Where(p => !p.StartsWith("{"))
            .Select(p => TypeMapper.SanitizeClassName(p));

        var methodPrefix = useHttpMethodNames
            ? Method switch
            {
                OperationType.Get => "Get",
                OperationType.Post => "Post",
                OperationType.Put => "Put",
                OperationType.Delete => "Delete",
                OperationType.Patch => "Patch",
                _ => Method.ToString()
            }
            : Method switch
            {
                OperationType.Get => "Get",
                OperationType.Post => "Create",
                OperationType.Put => "Update",
                OperationType.Delete => "Delete",
                OperationType.Patch => "Patch",
                _ => Method.ToString()
            };

        var name = methodPrefix + string.Join("", pathParts);
        return includeAsyncSuffix ? name + "Async" : name;
    }

    /// <summary>
    /// Gets the return type for this operation.
    /// </summary>
    public string GetReturnType()
    {
        var response = Operation.Responses?.FirstOrDefault(r => r.Key.StartsWith("2")).Value;
        if (response == null)
            return "Task";

        var content = response.Content?.FirstOrDefault(c => c.Key.Contains("json")).Value;
        if (content?.Schema == null)
            return "Task";

        var schemaType = TypeMapper.MapToCSharpType(content.Schema);
        return $"Task<{schemaType}>";
    }
}
