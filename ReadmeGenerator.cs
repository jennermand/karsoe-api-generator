using System.Text;

namespace karsoe_api_generator;

/// <summary>
/// Generates README.md documentation for the generated API client.
/// </summary>
public class ReadmeGenerator
{
    private readonly GeneratorOptions _options;
    private readonly OpenApiParser _parser;

    public ReadmeGenerator(GeneratorOptions options, OpenApiParser parser)
    {
        _options = options;
        _parser = parser;
    }

    /// <summary>
    /// Generates a comprehensive README.md file.
    /// </summary>
    public void GenerateReadme(
        string outputDirectory,
        IReadOnlyList<ModelInfo> models,
        IReadOnlyList<EndpointInfo> endpoints)
    {
        Console.WriteLine("\nGenerating README.md...");

        var sb = new StringBuilder();

        // Title and description
        sb.AppendLine($"# {_parser.ApiTitle ?? "API Client"}");
        sb.AppendLine();
        if (!string.IsNullOrWhiteSpace(_parser.ApiDescription))
        {
            sb.AppendLine(_parser.ApiDescription);
            sb.AppendLine();
        }

        sb.AppendLine("This is an auto-generated C# API client.");
        sb.AppendLine();

        // Table of Contents
        GenerateTableOfContents(sb);

        // Installation
        GenerateInstallationSection(sb);

        // Quick Start
        GenerateQuickStartSection(sb);

        // Authentication
        if (_options.GenerateAuthHandler)
        {
            GenerateAuthenticationSection(sb);
        }

        // Configuration
        GenerateConfigurationSection(sb);

        // Usage Examples
        GenerateUsageExamplesSection(sb, endpoints);

        // Available Endpoints
        GenerateEndpointsSection(sb, endpoints);

        // Models
        GenerateModelsSection(sb, models);

        // Error Handling
        GenerateErrorHandlingSection(sb);

        // Advanced Features
        GenerateAdvancedFeaturesSection(sb);

        var filePath = Path.Combine(outputDirectory, "README.md");
        File.WriteAllText(filePath, sb.ToString());

        Console.WriteLine($"README.md generated successfully.");
    }

    private void GenerateTableOfContents(StringBuilder sb)
    {
        sb.AppendLine("## Table of Contents");
        sb.AppendLine();
        sb.AppendLine("- [Installation](#installation)");
        sb.AppendLine("- [Quick Start](#quick-start)");
        if (_options.GenerateAuthHandler)
        {
            sb.AppendLine("- [Authentication](#authentication)");
        }
        sb.AppendLine("- [Configuration](#configuration)");
        sb.AppendLine("- [Usage Examples](#usage-examples)");
        sb.AppendLine("- [Available Endpoints](#available-endpoints)");
        sb.AppendLine("- [Models](#models)");
        sb.AppendLine("- [Error Handling](#error-handling)");
        sb.AppendLine("- [Advanced Features](#advanced-features)");
        sb.AppendLine();
    }

    private void GenerateInstallationSection(StringBuilder sb)
    {
        sb.AppendLine("## Installation");
        sb.AppendLine();
        sb.AppendLine("### Prerequisites");
        sb.AppendLine();
        sb.AppendLine($"- .NET {(_options.Namespace.Contains("10") ? "10.0" : "8.0")} or higher");
        sb.AppendLine();
        sb.AppendLine("### Required NuGet Packages");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine("dotnet add package Microsoft.Extensions.Http");
        sb.AppendLine("dotnet add package System.Text.Json");
        if (_options.EnableRetryPolicy)
        {
            sb.AppendLine("dotnet add package Microsoft.Extensions.Http.Polly");
            sb.AppendLine("dotnet add package Polly");
        }
        if (_options.EnableLogging)
        {
            sb.AppendLine("dotnet add package Microsoft.Extensions.Logging");
        }
        sb.AppendLine("```");
        sb.AppendLine();
    }

    private void GenerateQuickStartSection(StringBuilder sb)
    {
        sb.AppendLine("## Quick Start");
        sb.AppendLine();
        sb.AppendLine("### 1. Add to Dependency Injection");
        sb.AppendLine();
        sb.AppendLine("```csharp");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine($"using {_options.Namespace};");
        sb.AppendLine();
        sb.AppendLine("// In your startup/program configuration");
        sb.AppendLine("services.AddHttpClient<ApiClient>(client =>");
        sb.AppendLine("{");
        sb.AppendLine($"    client.BaseAddress = new Uri(\"{_parser.BaseUrl ?? "https://api.example.com"}\");");
        sb.AppendLine("});");
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("### 2. Inject and Use");
        sb.AppendLine();
        sb.AppendLine("```csharp");
        sb.AppendLine("public class MyService");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly ApiClient _apiClient;");
        sb.AppendLine();
        sb.AppendLine("    public MyService(ApiClient apiClient)");
        sb.AppendLine("    {");
        sb.AppendLine("        _apiClient = apiClient;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public async Task DoSomething()");
        sb.AppendLine("    {");
        sb.AppendLine("        // Call API methods");
        sb.AppendLine("        var result = await _apiClient.GetSomeDataAsync();");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine("```");
        sb.AppendLine();
    }

    private void GenerateAuthenticationSection(StringBuilder sb)
    {
        sb.AppendLine("## Authentication");
        sb.AppendLine();
        sb.AppendLine("This API uses token-based authentication. Configure authentication when setting up the client:");
        sb.AppendLine();
        sb.AppendLine("```csharp");
        sb.AppendLine("services.AddHttpClient<ApiClient>(client =>");
        sb.AppendLine("{");
        sb.AppendLine($"    client.BaseAddress = new Uri(\"{_parser.BaseUrl ?? "https://api.example.com"}\");");
        sb.AppendLine("    client.DefaultRequestHeaders.Authorization = ");
        sb.AppendLine("        new AuthenticationHeaderValue(\"Bearer\", \"your-token-here\");");
        sb.AppendLine("});");
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("Or obtain a token dynamically:");
        sb.AppendLine();
        sb.AppendLine("```csharp");
        sb.AppendLine("// First, authenticate to get a token");
        sb.AppendLine("var token = await _apiClient.AuthTokenAsync(new TokenRequest");
        sb.AppendLine("{");
        sb.AppendLine("    Username = \"your-username\",");
        sb.AppendLine("    Password = \"your-password\"");
        sb.AppendLine("});");
        sb.AppendLine();
        sb.AppendLine("// Then configure the client with the token");
        sb.AppendLine("_httpClient.DefaultRequestHeaders.Authorization = ");
        sb.AppendLine("    new AuthenticationHeaderValue(\"Bearer\", token);");
        sb.AppendLine("```");
        sb.AppendLine();
    }

    private void GenerateConfigurationSection(StringBuilder sb)
    {
        sb.AppendLine("## Configuration");
        sb.AppendLine();
        sb.AppendLine("### Base URL");
        sb.AppendLine();
        sb.AppendLine($"The default base URL is `{_parser.BaseUrl ?? "https://api.example.com"}`. You can override it:");
        sb.AppendLine();
        sb.AppendLine("```csharp");
        sb.AppendLine("services.AddHttpClient<ApiClient>(client =>");
        sb.AppendLine("{");
        sb.AppendLine("    client.BaseAddress = new Uri(\"https://your-custom-url.com\");");
        sb.AppendLine("});");
        sb.AppendLine("```");
        sb.AppendLine();

        if (_options.EnableRetryPolicy)
        {
            sb.AppendLine("### Retry Policy");
            sb.AppendLine();
            sb.AppendLine("The client includes automatic retry logic for transient failures:");
            sb.AppendLine();
            sb.AppendLine("```csharp");
            sb.AppendLine("using Polly;");
            sb.AppendLine("using Polly.Extensions.Http;");
            sb.AppendLine();
            sb.AppendLine("services.AddHttpClient<ApiClient>(client =>");
            sb.AppendLine("{");
            sb.AppendLine($"    client.BaseAddress = new Uri(\"{_parser.BaseUrl ?? "https://api.example.com"}\");");
            sb.AppendLine("})");
            sb.AppendLine(".AddPolicyHandler(HttpPolicyExtensions");
            sb.AppendLine("    .HandleTransientHttpError()");
            sb.AppendLine("    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)");
            sb.AppendLine("    .WaitAndRetryAsync(3, retryAttempt => ");
            sb.AppendLine("        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));");
            sb.AppendLine("```");
            sb.AppendLine();
        }

        if (_options.EnableLogging)
        {
            sb.AppendLine("### Logging");
            sb.AppendLine();
            sb.AppendLine("The client supports logging through Microsoft.Extensions.Logging:");
            sb.AppendLine();
            sb.AppendLine("```csharp");
            sb.AppendLine("// Logging is automatically configured if ILogger<ApiClient> is registered");
            sb.AppendLine("services.AddLogging(builder =>");
            sb.AppendLine("{");
            sb.AppendLine("    builder.AddConsole();");
            sb.AppendLine("    builder.SetMinimumLevel(LogLevel.Debug);");
            sb.AppendLine("});");
            sb.AppendLine("```");
            sb.AppendLine();
        }
    }

    private void GenerateUsageExamplesSection(StringBuilder sb, IReadOnlyList<EndpointInfo> endpoints)
    {
        sb.AppendLine("## Usage Examples");
        sb.AppendLine();

        var endpointsByTag = endpoints.GroupBy(e => e.Tag).Take(3);

        foreach (var tagGroup in endpointsByTag)
        {
            sb.AppendLine($"### {tagGroup.Key}");
            sb.AppendLine();

            var exampleEndpoints = tagGroup.Take(2);
            foreach (var endpoint in exampleEndpoints)
            {
                sb.AppendLine($"#### {endpoint.Summary}");
                sb.AppendLine();
                sb.AppendLine("```csharp");
                
                if (endpoint.ReturnType == "Task")
                {
                    sb.AppendLine($"await _apiClient.{endpoint.MethodName}();");
                }
                else
                {
                    var varName = ToCamelCase(endpoint.MethodName.Replace("Async", "").Replace("Get", "").Replace("Create", "").Replace("Update", "").Replace("Delete", ""));
                    if (string.IsNullOrWhiteSpace(varName)) varName = "result";
                    sb.AppendLine($"var {varName} = await _apiClient.{endpoint.MethodName}();");
                }
                
                sb.AppendLine("```");
                sb.AppendLine();
            }
        }
    }

    private void GenerateEndpointsSection(StringBuilder sb, IReadOnlyList<EndpointInfo> endpoints)
    {
        sb.AppendLine("## Available Endpoints");
        sb.AppendLine();

        var groupedByTag = endpoints.GroupBy(e => e.Tag).OrderBy(g => g.Key);

        foreach (var group in groupedByTag)
        {
            sb.AppendLine($"### {group.Key}");
            sb.AppendLine();
            sb.AppendLine("| Method | HTTP | Path | Returns |");
            sb.AppendLine("|--------|------|------|---------|");

            foreach (var endpoint in group.OrderBy(e => e.Path))
            {
                var returnType = endpoint.ReturnType.Replace("Task<", "").Replace(">", "");
                if (returnType == "Task") returnType = "void";
                
                sb.AppendLine($"| `{endpoint.MethodName}` | {endpoint.HttpMethod} | `{endpoint.Path}` | `{returnType}` |");
            }

            sb.AppendLine();
        }
    }

    private void GenerateModelsSection(StringBuilder sb, IReadOnlyList<ModelInfo> models)
    {
        sb.AppendLine("## Models");
        sb.AppendLine();
        sb.AppendLine($"The client includes {models.Count} data transfer object (DTO) models:");
        sb.AppendLine();

        foreach (var model in models.OrderBy(m => m.Name))
        {
            sb.AppendLine($"### {model.Name}");
            sb.AppendLine();
            sb.AppendLine(model.Description);
            sb.AppendLine();

            if (model.Properties.Any())
            {
                sb.AppendLine("| Property | Type | Description |");
                sb.AppendLine("|----------|------|-------------|");

                foreach (var prop in model.Properties)
                {
                    var desc = prop.Description ?? "";
                    sb.AppendLine($"| `{prop.Name}` | `{prop.Type}` | {desc} |");
                }

                sb.AppendLine();
            }
        }
    }

    private void GenerateErrorHandlingSection(StringBuilder sb)
    {
        sb.AppendLine("## Error Handling");
        sb.AppendLine();
        sb.AppendLine("The client throws `HttpRequestException` for failed requests:");
        sb.AppendLine();
        sb.AppendLine("```csharp");
        sb.AppendLine("try");
        sb.AppendLine("{");
        sb.AppendLine("    var result = await _apiClient.GetDataAsync();");
        sb.AppendLine("}");
        sb.AppendLine("catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)");
        sb.AppendLine("{");
        sb.AppendLine("    // Handle 404 Not Found");
        sb.AppendLine("}");
        sb.AppendLine("catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)");
        sb.AppendLine("{");
        sb.AppendLine("    // Handle 401 Unauthorized");
        sb.AppendLine("}");
        sb.AppendLine("catch (HttpRequestException ex)");
        sb.AppendLine("{");
        sb.AppendLine("    // Handle other HTTP errors");
        sb.AppendLine("}");
        sb.AppendLine("```");
        sb.AppendLine();
    }

    private void GenerateAdvancedFeaturesSection(StringBuilder sb)
    {
        sb.AppendLine("## Advanced Features");
        sb.AppendLine();
        sb.AppendLine("### Cancellation Tokens");
        sb.AppendLine();
        sb.AppendLine("All methods support cancellation:");
        sb.AppendLine();
        sb.AppendLine("```csharp");
        sb.AppendLine("var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));");
        sb.AppendLine("var result = await _apiClient.GetDataAsync(cancellationToken: cts.Token);");
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("### Custom Headers");
        sb.AppendLine();
        sb.AppendLine("Add custom headers to the HttpClient:");
        sb.AppendLine();
        sb.AppendLine("```csharp");
        sb.AppendLine("services.AddHttpClient<ApiClient>(client =>");
        sb.AppendLine("{");
        sb.AppendLine($"    client.BaseAddress = new Uri(\"{_parser.BaseUrl ?? "https://api.example.com"}\");");
        sb.AppendLine("    client.DefaultRequestHeaders.Add(\"X-Custom-Header\", \"value\");");
        sb.AppendLine("});");
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine($"*Generated by karsoe-api-generator on {DateTime.Now:yyyy-MM-dd HH:mm:ss}*");
        sb.AppendLine();
    }

    private string ToCamelCase(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length == 0)
            return text;

        return char.ToLowerInvariant(text[0]) + text[1..];
    }
}
