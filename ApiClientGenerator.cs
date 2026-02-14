using Microsoft.OpenApi.Models;
using System.Text;

namespace karsoe_api_generator;

/// <summary>
/// Generates the API client class with typed methods for all endpoints.
/// </summary>
public class ApiClientGenerator
{
    private readonly GeneratorOptions _options;
    private readonly List<EndpointInfo> _endpointMetadata = new();
    private readonly HashSet<string> _generatedMethodNames = new();

    public IReadOnlyList<EndpointInfo> GeneratedEndpoints => _endpointMetadata;

    public ApiClientGenerator(GeneratorOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Generates the API client class.
    /// </summary>
    public void GenerateClient(OpenApiParser parser, string outputDirectory)
    {
        Console.WriteLine($"\nGenerating API client...");

        var operationsByTag = parser.GetOperationsByTag();
        var code = GenerateClientCode(parser, operationsByTag);
        
        var filePath = Path.Combine(outputDirectory, "ApiClient.cs");
        File.WriteAllText(filePath, code);

        Console.WriteLine($"Generated API client with {_endpointMetadata.Count} methods.");
    }

    private string GenerateClientCode(OpenApiParser parser, Dictionary<string, List<OperationInfo>> operationsByTag)
    {
        var sb = new StringBuilder();

        // Using statements
        sb.AppendLine("using System.Text;");
        sb.AppendLine("using System.Text.Json;");
        sb.AppendLine($"using {_options.Namespace}.Models;");
        if (_options.EnableLogging)
        {
            sb.AppendLine("using Microsoft.Extensions.Logging;");
        }
        sb.AppendLine();

        // Namespace
        sb.AppendLine($"namespace {_options.Namespace};");
        sb.AppendLine();

        // Class documentation
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// API client for {parser.ApiTitle ?? "the API"}.");
        if (!string.IsNullOrWhiteSpace(parser.ApiDescription))
        {
            sb.AppendLine($"/// {EscapeXmlComment(parser.ApiDescription)}");
        }
        sb.AppendLine("/// </summary>");

        // Class declaration
        sb.AppendLine("public class ApiClient");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly HttpClient _httpClient;");
        if (_options.EnableLogging)
        {
            sb.AppendLine("    private readonly ILogger<ApiClient>? _logger;");
        }
        sb.AppendLine("    private readonly JsonSerializerOptions _jsonOptions;");
        sb.AppendLine();

        // Constructor
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Initializes a new instance of the ApiClient.");
        sb.AppendLine("    /// </summary>");
        sb.Append("    public ApiClient(HttpClient httpClient");
        if (_options.EnableLogging)
        {
            sb.Append(", ILogger<ApiClient>? logger = null");
        }
        sb.AppendLine(")");
        sb.AppendLine("    {");
        sb.AppendLine("        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));");
        if (_options.EnableLogging)
        {
            sb.AppendLine("        _logger = logger;");
        }
        sb.AppendLine("        _jsonOptions = new JsonSerializerOptions");
        sb.AppendLine("        {");
        sb.AppendLine("            PropertyNameCaseInsensitive = true,");
        sb.AppendLine("            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull");
        sb.AppendLine("        };");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Generate methods grouped by tag
        foreach (var tag in operationsByTag.OrderBy(t => t.Key))
        {
            sb.AppendLine($"    #region {tag.Key}");
            sb.AppendLine();

            foreach (var operation in tag.Value)
            {
                GenerateMethod(sb, operation);
                sb.AppendLine();
            }

            sb.AppendLine($"    #endregion");
            sb.AppendLine();
        }

        // Helper methods
        GenerateHelperMethods(sb);

        sb.AppendLine("}");

        return sb.ToString();
    }

    private void GenerateMethod(StringBuilder sb, OperationInfo operation)
    {
        var methodName = operation.GetMethodName(_options.UseAsyncSuffix);
        
        // Check for duplicate method names and skip
        if (_generatedMethodNames.Contains(methodName))
        {
            Console.WriteLine($"  Skipping duplicate method: {methodName}");
            return;
        }
        _generatedMethodNames.Add(methodName);
        
        var returnType = operation.GetReturnType();
        
        // Extract parameters
        var pathParams = operation.Operation.Parameters?
            .Where(p => p.In == ParameterLocation.Path)
            .ToList() ?? new List<OpenApiParameter>();
        
        var queryParams = operation.Operation.Parameters?
            .Where(p => p.In == ParameterLocation.Query)
            .ToList() ?? new List<OpenApiParameter>();

        var hasRequestBody = operation.Operation.RequestBody != null;
        string? bodyTypeName = null;

        if (hasRequestBody)
        {
            var content = operation.Operation.RequestBody?.Content?.FirstOrDefault(c => c.Key.Contains("json")).Value;
            if (content?.Schema != null)
            {
                bodyTypeName = TypeMapper.MapToCSharpType(content.Schema);
            }
        }

        // XML documentation
        sb.AppendLine("    /// <summary>");
        var summary = operation.Operation.Summary ?? operation.Operation.Description ?? $"{operation.Method} {operation.Path}";
        sb.AppendLine($"    /// {EscapeXmlComment(summary)}");
        sb.AppendLine("    /// </summary>");

        // Method signature
        sb.Append($"    public async {returnType} {methodName}(");
        
        var parameters = new List<string>();
        
        // Path parameters (required)
        foreach (var param in pathParams)
        {
            var paramType = TypeMapper.MapToCSharpType(param.Schema, !param.Required);
            parameters.Add($"{paramType} {ToCamelCase(param.Name)}");
        }

        // Request body (required)
        if (hasRequestBody && bodyTypeName != null)
        {
            parameters.Add($"{bodyTypeName} request");
        }

        // Query parameters (optional - must come after required)
        foreach (var param in queryParams)
        {
            var paramType = TypeMapper.MapToCSharpType(param.Schema, !param.Required);
            parameters.Add($"{paramType} {ToCamelCase(param.Name)} = default!");
        }

        // Cancellation token (optional)
        parameters.Add("CancellationToken cancellationToken = default");

        sb.AppendLine(string.Join(", ", parameters) + ")");
        sb.AppendLine("    {");

        // Logging
        if (_options.EnableLogging)
        {
            sb.AppendLine($"        _logger?.LogDebug(\"Calling {methodName}\");");
            sb.AppendLine();
        }

        // Build URL
        var urlPath = operation.Path;
        foreach (var param in pathParams)
        {
            var paramName = ToCamelCase(param.Name);
            urlPath = urlPath.Replace($"{{{param.Name}}}", $"{{{paramName}}}");
        }

        if (queryParams.Any())
        {
            sb.AppendLine($"        var url = $\"{urlPath}\";");
            sb.AppendLine("        var queryParams = new List<string>();");
            foreach (var param in queryParams)
            {
                var paramName = ToCamelCase(param.Name);
                sb.AppendLine($"        if ({paramName} != default) queryParams.Add($\"{param.Name}={{{paramName}}}\");");
            }
            sb.AppendLine("        if (queryParams.Any()) url += \"?\" + string.Join(\"&\", queryParams);");
        }
        else
        {
            sb.AppendLine($"        var url = $\"{urlPath}\";");
        }
        sb.AppendLine();

        // HTTP request
        var httpMethod = operation.Method.ToString().ToUpperInvariant();
        var isReturningData = !returnType.Equals("Task", StringComparison.OrdinalIgnoreCase);

        if (hasRequestBody && bodyTypeName != null)
        {
            sb.AppendLine("        var content = new StringContent(");
            sb.AppendLine("            JsonSerializer.Serialize(request, _jsonOptions),");
            sb.AppendLine("            Encoding.UTF8,");
            sb.AppendLine("            \"application/json\");");
            sb.AppendLine();
            sb.AppendLine($"        var response = await _httpClient.{GetHttpClientMethod(operation.Method)}(url, content, cancellationToken);");
        }
        else if (operation.Method == OperationType.Post || operation.Method == OperationType.Put || operation.Method == OperationType.Patch)
        {
            sb.AppendLine($"        var response = await _httpClient.{GetHttpClientMethod(operation.Method)}(url, null, cancellationToken);");
        }
        else
        {
            sb.AppendLine($"        var response = await _httpClient.{GetHttpClientMethod(operation.Method)}(url, cancellationToken);");
        }

        sb.AppendLine("        await EnsureSuccessStatusCodeAsync(response);");
        sb.AppendLine();

        // Deserialize response
        if (isReturningData)
        {
            sb.AppendLine("        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);");
            // Extract the type from Task<T> - remove "Task<" from beginning and ">" from end
            var innerType = returnType.StartsWith("Task<") ? returnType[5..^1] : returnType;
            
            // Check if it's a value type (primitive or struct) - these can't be null in deserialization
            var isValueType = IsValueType(innerType);
            
            if (isValueType)
            {
                sb.AppendLine($"        return JsonSerializer.Deserialize<{innerType}>(responseContent, _jsonOptions);");
            }
            else
            {
                sb.AppendLine($"        return JsonSerializer.Deserialize<{innerType}>(responseContent, _jsonOptions)");
                sb.AppendLine($"            ?? throw new InvalidOperationException(\"Failed to deserialize response\");");
            }
        }

        sb.AppendLine("    }");

        // Store metadata
        _endpointMetadata.Add(new EndpointInfo
        {
            MethodName = methodName,
            HttpMethod = httpMethod,
            Path = operation.Path,
            Summary = summary,
            ReturnType = returnType,
            Tag = operation.Operation.Tags?.FirstOrDefault()?.Name ?? "Default"
        });
    }

    private void GenerateHelperMethods(StringBuilder sb)
    {
        sb.AppendLine("    private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (!response.IsSuccessStatusCode)");
        sb.AppendLine("        {");
        sb.AppendLine("            var content = await response.Content.ReadAsStringAsync();");
        if (_options.EnableLogging)
        {
            sb.AppendLine("            _logger?.LogError(\"API request failed with status {StatusCode}: {Content}\",");
            sb.AppendLine("                response.StatusCode, content);");
        }
        sb.AppendLine("            throw new HttpRequestException(");
        sb.AppendLine("                $\"Request failed with status {response.StatusCode}: {content}\",");
        sb.AppendLine("                null,");
        sb.AppendLine("                response.StatusCode);");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
    }

    private string GetHttpClientMethod(OperationType operationType)
    {
        return operationType switch
        {
            OperationType.Get => "GetAsync",
            OperationType.Post => "PostAsync",
            OperationType.Put => "PutAsync",
            OperationType.Delete => "DeleteAsync",
            OperationType.Patch => "PatchAsync",
            _ => "SendAsync"
        };
    }

    private bool IsValueType(string typeName)
    {
        // Remove nullable suffix if present
        var baseType = typeName.TrimEnd('?');
        
        // Check for common value types
        return baseType switch
        {
            "bool" or "byte" or "sbyte" or "char" or
            "short" or "ushort" or "int" or "uint" or
            "long" or "ulong" or "float" or "double" or "decimal" or
            "DateTime" or "DateTimeOffset" or "DateOnly" or "TimeOnly" or
            "Guid" or "TimeSpan" => true,
            _ => false
        };
    }

    private string ToCamelCase(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length == 0)
            return text;

        return char.ToLowerInvariant(text[0]) + text[1..];
    }

    private string EscapeXmlComment(string text)
    {
        return text.Replace("&", "&amp;")
                   .Replace("<", "&lt;")
                   .Replace(">", "&gt;")
                   .Replace("\"", "&quot;")
                   .Replace("'", "&apos;");
    }
}

/// <summary>
/// Metadata about a generated API endpoint.
/// </summary>
public class EndpointInfo
{
    public required string MethodName { get; set; }
    public required string HttpMethod { get; set; }
    public required string Path { get; set; }
    public required string Summary { get; set; }
    public required string ReturnType { get; set; }
    public required string Tag { get; set; }
}
