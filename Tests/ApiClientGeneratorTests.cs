using FluentAssertions;
using karsoe_api_generator;
using Microsoft.OpenApi.Models;
using Xunit;

namespace karsoe_api_generator.Tests;

public class ApiClientGeneratorTests : IDisposable
{
    private readonly string _testOutputDirectory;
    private readonly GeneratorOptions _options;

    public ApiClientGeneratorTests()
    {
        _testOutputDirectory = Path.Combine(Path.GetTempPath(), $"ApiClientTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testOutputDirectory);

        _options = new GeneratorOptions
        {
            InputPath = "test.json",
            OutputDirectory = _testOutputDirectory,
            Namespace = "TestNamespace",
            EnableLogging = true,
            EnableRetryPolicy = true,
            UseAsyncSuffix = true
        };
    }

    public void Dispose()
    {
        if (Directory.Exists(_testOutputDirectory))
        {
            Directory.Delete(_testOutputDirectory, true);
        }
    }

    private OpenApiParser CreateParserWithBasicApi()
    {
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = "Test API",
                Version = "1.0.0",
                Description = "Test API Description"
            },
            Servers = new List<OpenApiServer>
            {
                new OpenApiServer { Url = "https://api.example.com" }
            },
            Paths = new OpenApiPaths
            {
                ["/products"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new OpenApiOperation
                        {
                            OperationId = "GetProducts",
                            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } },
                            Summary = "Get all products",
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Success",
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "array",
                                                Items = new OpenApiSchema
                                                {
                                                    Reference = new OpenApiReference 
                                                    { 
                                                        Id = "Product",
                                                        Type = ReferenceType.Schema
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var parser = new OpenApiParser();
        typeof(OpenApiParser).GetProperty("Document")!.SetValue(parser, document);
        typeof(OpenApiParser).GetProperty("ApiTitle")!.SetValue(parser, document.Info.Title);
        typeof(OpenApiParser).GetProperty("ApiDescription")!.SetValue(parser, document.Info.Description);
        typeof(OpenApiParser).GetProperty("BaseUrl")!.SetValue(parser, document.Servers[0].Url);

        return parser;
    }

    [Fact]
    public void GenerateClient_CreatesApiClientFile()
    {
        // Arrange
        var generator = new ApiClientGenerator(_options);
        var parser = CreateParserWithBasicApi();

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        var clientPath = Path.Combine(_testOutputDirectory, "ApiClient.cs");
        File.Exists(clientPath).Should().BeTrue();
    }

    [Fact]
    public void GenerateClient_IncludesNamespace()
    {
        // Arrange
        var generator = new ApiClientGenerator(_options);
        var parser = CreateParserWithBasicApi();

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        var clientPath = Path.Combine(_testOutputDirectory, "ApiClient.cs");
        var content = File.ReadAllText(clientPath);
        content.Should().Contain("namespace TestNamespace;");
    }

    [Fact]
    public void GenerateClient_IncludesRequiredUsings()
    {
        // Arrange
        var generator = new ApiClientGenerator(_options);
        var parser = CreateParserWithBasicApi();

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        var clientPath = Path.Combine(_testOutputDirectory, "ApiClient.cs");
        var content = File.ReadAllText(clientPath);
        content.Should().Contain("using System.Text;");
        content.Should().Contain("using System.Text.Json;");
        content.Should().Contain("using TestNamespace.Models;");
    }

    [Fact]
    public void GenerateClient_WithLoggingEnabled_IncludesLoggerUsing()
    {
        // Arrange
        var generator = new ApiClientGenerator(_options);
        var parser = CreateParserWithBasicApi();

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        var clientPath = Path.Combine(_testOutputDirectory, "ApiClient.cs");
        var content = File.ReadAllText(clientPath);
        content.Should().Contain("using Microsoft.Extensions.Logging;");
        content.Should().Contain("private readonly ILogger<ApiClient>? _logger;");
    }

    [Fact]
    public void GenerateClient_WithLoggingDisabled_ExcludesLogger()
    {
        // Arrange
        var optionsWithoutLogging = new GeneratorOptions
        {
            InputPath = "test.json",
            OutputDirectory = _testOutputDirectory,
            Namespace = "TestNamespace",
            EnableLogging = false
        };
        var generator = new ApiClientGenerator(optionsWithoutLogging);
        var parser = CreateParserWithBasicApi();

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        var clientPath = Path.Combine(_testOutputDirectory, "ApiClient.cs");
        var content = File.ReadAllText(clientPath);
        content.Should().NotContain("ILogger");
    }

    [Fact]
    public void GenerateClient_IncludesApiDocumentation()
    {
        // Arrange
        var generator = new ApiClientGenerator(_options);
        var parser = CreateParserWithBasicApi();

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        var clientPath = Path.Combine(_testOutputDirectory, "ApiClient.cs");
        var content = File.ReadAllText(clientPath);
        content.Should().Contain("/// <summary>");
        content.Should().Contain("/// API client for Test API.");
        content.Should().Contain("/// Test API Description");
    }

    [Fact]
    public void GenerateClient_IncludesConstructor()
    {
        // Arrange
        var generator = new ApiClientGenerator(_options);
        var parser = CreateParserWithBasicApi();

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        var clientPath = Path.Combine(_testOutputDirectory, "ApiClient.cs");
        var content = File.ReadAllText(clientPath);
        content.Should().Contain("public ApiClient(HttpClient httpClient");
        content.Should().Contain("_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));");
    }

    [Fact]
    public void GenerateClient_GeneratesMethodsGroupedByTag()
    {
        // Arrange
        var generator = new ApiClientGenerator(_options);
        var parser = CreateParserWithBasicApi();

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        var clientPath = Path.Combine(_testOutputDirectory, "ApiClient.cs");
        var content = File.ReadAllText(clientPath);
        content.Should().Contain("#region Products");
        content.Should().Contain("#endregion");
    }

    [Fact]
    public void GenerateClient_GeneratesMethodWithCorrectSignature()
    {
        // Arrange
        var generator = new ApiClientGenerator(_options);
        var parser = CreateParserWithBasicApi();

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        var clientPath = Path.Combine(_testOutputDirectory, "ApiClient.cs");
        var content = File.ReadAllText(clientPath);
        content.Should().Contain("public async Task<List<Product>> GetProductsAsync(");
        content.Should().Contain("CancellationToken cancellationToken = default");
    }

    [Fact]
    public void GenerateClient_IncludesHelperMethods()
    {
        // Arrange
        var generator = new ApiClientGenerator(_options);
        var parser = CreateParserWithBasicApi();

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        var clientPath = Path.Combine(_testOutputDirectory, "ApiClient.cs");
        var content = File.ReadAllText(clientPath);
        content.Should().Contain("private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)");
    }

    [Fact]
    public void GenerateClient_WithOperationWithPathParameters_GeneratesCorrectMethod()
    {
        // Arrange
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo { Title = "Test API", Version = "1.0" },
            Paths = new OpenApiPaths
            {
                ["/products/{id}"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new OpenApiOperation
                        {
                            OperationId = "GetProduct",
                            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } },
                            Parameters = new List<OpenApiParameter>
                            {
                                new OpenApiParameter
                                {
                                    Name = "id",
                                    In = ParameterLocation.Path,
                                    Required = true,
                                    Schema = new OpenApiSchema { Type = "integer", Format = "int32" }
                                }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse { Description = "Success" }
                            }
                        }
                    }
                }
            }
        };

        var parser = new OpenApiParser();
        typeof(OpenApiParser).GetProperty("Document")!.SetValue(parser, document);
        typeof(OpenApiParser).GetProperty("ApiTitle")!.SetValue(parser, "Test API");

        var generator = new ApiClientGenerator(_options);

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        var clientPath = Path.Combine(_testOutputDirectory, "ApiClient.cs");
        var content = File.ReadAllText(clientPath);
        content.Should().Contain("int id");
        content.Should().Contain("var url = $\"/products/{id}\";");
    }

    [Fact]
    public void GenerateClient_WithQueryParameters_GeneratesQueryString()
    {
        // Arrange
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo { Title = "Test API", Version = "1.0" },
            Paths = new OpenApiPaths
            {
                ["/products"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new OpenApiOperation
                        {
                            OperationId = "SearchProducts",
                            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } },
                            Parameters = new List<OpenApiParameter>
                            {
                                new OpenApiParameter
                                {
                                    Name = "search",
                                    In = ParameterLocation.Query,
                                    Required = false,
                                    Schema = new OpenApiSchema { Type = "string" }
                                }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse { Description = "Success" }
                            }
                        }
                    }
                }
            }
        };

        var parser = new OpenApiParser();
        typeof(OpenApiParser).GetProperty("Document")!.SetValue(parser, document);
        typeof(OpenApiParser).GetProperty("ApiTitle")!.SetValue(parser, "Test API");

        var generator = new ApiClientGenerator(_options);

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        var clientPath = Path.Combine(_testOutputDirectory, "ApiClient.cs");
        var content = File.ReadAllText(clientPath);
        content.Should().Contain("string? search = default!");
        content.Should().Contain("var queryParams = new List<string>();");
        content.Should().Contain("if (search != default) queryParams.Add($\"search={search}\");");
    }

    [Fact]
    public void GenerateClient_StoresEndpointMetadata()
    {
        // Arrange
        var generator = new ApiClientGenerator(_options);
        var parser = CreateParserWithBasicApi();

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        var metadata = generator.GeneratedEndpoints;
        metadata.Should().HaveCount(1);
        metadata[0].MethodName.Should().Be("GetProductsAsync");
        metadata[0].HttpMethod.Should().Be("GET");
        metadata[0].Path.Should().Be("/products");
        metadata[0].ReturnType.Should().Be("Task<List<Product>>");
        metadata[0].Tag.Should().Be("Products");
    }

    [Fact]
    public void GenerateClient_WithDuplicateMethodNames_SkipsDuplicates()
    {
        // Arrange
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo { Title = "Test API", Version = "1.0" },
            Paths = new OpenApiPaths
            {
                ["/products"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new OpenApiOperation
                        {
                            OperationId = "GetProducts",
                            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse { Description = "Success" }
                            }
                        }
                    }
                },
                ["/v2/products"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new OpenApiOperation
                        {
                            OperationId = "GetProducts", // Same operation ID
                            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Products" } },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse { Description = "Success" }
                            }
                        }
                    }
                }
            }
        };

        var parser = new OpenApiParser();
        typeof(OpenApiParser).GetProperty("Document")!.SetValue(parser, document);
        typeof(OpenApiParser).GetProperty("ApiTitle")!.SetValue(parser, "Test API");

        var generator = new ApiClientGenerator(_options);

        // Act
        generator.GenerateClient(parser, _testOutputDirectory);

        // Assert
        generator.GeneratedEndpoints.Should().HaveCount(1);
    }
}
