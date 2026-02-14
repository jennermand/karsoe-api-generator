using FluentAssertions;
using karsoe_api_generator;
using Microsoft.OpenApi.Models;
using Xunit;

namespace karsoe_api_generator.Tests;

public class ReadmeGeneratorTests : IDisposable
{
    private readonly string _testOutputDirectory;
    private readonly GeneratorOptions _options;
    private readonly OpenApiParser _parser;

    public ReadmeGeneratorTests()
    {
        _testOutputDirectory = Path.Combine(Path.GetTempPath(), $"ReadmeTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testOutputDirectory);

        _options = new GeneratorOptions
        {
            InputPath = "test.json",
            OutputDirectory = _testOutputDirectory,
            Namespace = "TestNamespace",
            GenerateAuthHandler = true,
            EnableRetryPolicy = true,
            EnableLogging = true,
            GenerateReadme = true
        };

        // Create a parser with basic API info
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = "Test API",
                Version = "1.0.0",
                Description = "A comprehensive test API"
            },
            Servers = new List<OpenApiServer>
            {
                new OpenApiServer { Url = "https://api.example.com" }
            }
        };

        _parser = new OpenApiParser();
        typeof(OpenApiParser).GetProperty("Document")!.SetValue(_parser, document);
        typeof(OpenApiParser).GetProperty("ApiTitle")!.SetValue(_parser, document.Info.Title);
        typeof(OpenApiParser).GetProperty("ApiDescription")!.SetValue(_parser, document.Info.Description);
        typeof(OpenApiParser).GetProperty("BaseUrl")!.SetValue(_parser, document.Servers[0].Url);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testOutputDirectory))
        {
            Directory.Delete(_testOutputDirectory, true);
        }
    }

    [Fact]
    public void GenerateReadme_CreatesReadmeFile()
    {
        // Arrange
        var generator = new ReadmeGenerator(_options, _parser);
        var models = new List<ModelInfo>();
        var endpoints = new List<EndpointInfo>();

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        File.Exists(readmePath).Should().BeTrue();
    }

    [Fact]
    public void GenerateReadme_IncludesApiTitle()
    {
        // Arrange
        var generator = new ReadmeGenerator(_options, _parser);
        var models = new List<ModelInfo>();
        var endpoints = new List<EndpointInfo>();

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        var content = File.ReadAllText(readmePath);
        content.Should().Contain("# Test API");
    }

    [Fact]
    public void GenerateReadme_IncludesApiDescription()
    {
        // Arrange
        var generator = new ReadmeGenerator(_options, _parser);
        var models = new List<ModelInfo>();
        var endpoints = new List<EndpointInfo>();

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        var content = File.ReadAllText(readmePath);
        content.Should().Contain("A comprehensive test API");
    }

    [Fact]
    public void GenerateReadme_IncludesTableOfContents()
    {
        // Arrange
        var generator = new ReadmeGenerator(_options, _parser);
        var models = new List<ModelInfo>();
        var endpoints = new List<EndpointInfo>();

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        var content = File.ReadAllText(readmePath);
        content.Should().Contain("## Table of Contents");
        content.Should().Contain("- [Installation](#installation)");
        content.Should().Contain("- [Quick Start](#quick-start)");
        content.Should().Contain("- [Configuration](#configuration)");
    }

    [Fact]
    public void GenerateReadme_WithAuthEnabled_IncludesAuthenticationSection()
    {
        // Arrange
        var generator = new ReadmeGenerator(_options, _parser);
        var models = new List<ModelInfo>();
        var endpoints = new List<EndpointInfo>();

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        var content = File.ReadAllText(readmePath);
        content.Should().Contain("- [Authentication](#authentication)");
    }

    [Fact]
    public void GenerateReadme_WithAuthDisabled_ExcludesAuthenticationSection()
    {
        // Arrange
        var optionsWithoutAuth = new GeneratorOptions
        {
            InputPath = "test.json",
            OutputDirectory = _testOutputDirectory,
            Namespace = "TestNamespace",
            GenerateAuthHandler = false
        };
        var generator = new ReadmeGenerator(optionsWithoutAuth, _parser);
        var models = new List<ModelInfo>();
        var endpoints = new List<EndpointInfo>();

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        var content = File.ReadAllText(readmePath);
        content.Should().NotContain("- [Authentication](#authentication)");
    }

    [Fact]
    public void GenerateReadme_WithModels_IncludesModelSection()
    {
        // Arrange
        var generator = new ReadmeGenerator(_options, _parser);
        var models = new List<ModelInfo>
        {
            new ModelInfo
            {
                Name = "Product",
                Description = "Product model",
                Properties = new List<PropertyInfo>
                {
                    new PropertyInfo
                    {
                        Name = "Id",
                        Type = "int",
                        Description = "Product ID"
                    },
                    new PropertyInfo
                    {
                        Name = "Name",
                        Type = "string",
                        Description = "Product name"
                    }
                }
            }
        };
        var endpoints = new List<EndpointInfo>();

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        var content = File.ReadAllText(readmePath);
        content.Should().Contain("## Models");
        content.Should().Contain("### Product");
    }

    [Fact]
    public void GenerateReadme_WithEndpoints_IncludesEndpointSection()
    {
        // Arrange
        var generator = new ReadmeGenerator(_options, _parser);
        var models = new List<ModelInfo>();
        var endpoints = new List<EndpointInfo>
        {
            new EndpointInfo
            {
                MethodName = "GetProductsAsync",
                HttpMethod = "GET",
                Path = "/products",
                Summary = "Get all products",
                ReturnType = "Task<List<Product>>",
                Tag = "Products"
            }
        };

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        var content = File.ReadAllText(readmePath);
        content.Should().Contain("## Available Endpoints");
        content.Should().Contain("GetProductsAsync");
        content.Should().Contain("GET");
        content.Should().Contain("/products");
    }

    [Fact]
    public void GenerateReadme_IncludesInstallationSection()
    {
        // Arrange
        var generator = new ReadmeGenerator(_options, _parser);
        var models = new List<ModelInfo>();
        var endpoints = new List<EndpointInfo>();

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        var content = File.ReadAllText(readmePath);
        content.Should().Contain("## Installation");
    }

    [Fact]
    public void GenerateReadme_IncludesQuickStartSection()
    {
        // Arrange
        var generator = new ReadmeGenerator(_options, _parser);
        var models = new List<ModelInfo>();
        var endpoints = new List<EndpointInfo>();

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        var content = File.ReadAllText(readmePath);
        content.Should().Contain("## Quick Start");
    }

    [Fact]
    public void GenerateReadme_IncludesErrorHandlingSection()
    {
        // Arrange
        var generator = new ReadmeGenerator(_options, _parser);
        var models = new List<ModelInfo>();
        var endpoints = new List<EndpointInfo>();

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        var content = File.ReadAllText(readmePath);
        content.Should().Contain("## Error Handling");
    }

    [Fact]
    public void GenerateReadme_IncludesAdvancedFeaturesSection()
    {
        // Arrange
        var generator = new ReadmeGenerator(_options, _parser);
        var models = new List<ModelInfo>();
        var endpoints = new List<EndpointInfo>();

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        var content = File.ReadAllText(readmePath);
        content.Should().Contain("## Advanced Features");
    }

    [Fact]
    public void GenerateReadme_WithMultipleModels_ListsAll()
    {
        // Arrange
        var generator = new ReadmeGenerator(_options, _parser);
        var models = new List<ModelInfo>
        {
            new ModelInfo { Name = "Product", Description = "Product model", Properties = new List<PropertyInfo>() },
            new ModelInfo { Name = "User", Description = "User model", Properties = new List<PropertyInfo>() },
            new ModelInfo { Name = "Order", Description = "Order model", Properties = new List<PropertyInfo>() }
        };
        var endpoints = new List<EndpointInfo>();

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        var content = File.ReadAllText(readmePath);
        content.Should().Contain("### Product");
        content.Should().Contain("### User");
        content.Should().Contain("### Order");
    }

    [Fact]
    public void GenerateReadme_WithMultipleEndpoints_ListsAll()
    {
        // Arrange
        var generator = new ReadmeGenerator(_options, _parser);
        var models = new List<ModelInfo>();
        var endpoints = new List<EndpointInfo>
        {
            new EndpointInfo
            {
                MethodName = "GetProductsAsync",
                HttpMethod = "GET",
                Path = "/products",
                Summary = "Get products",
                ReturnType = "Task<List<Product>>",
                Tag = "Products"
            },
            new EndpointInfo
            {
                MethodName = "CreateProductAsync",
                HttpMethod = "POST",
                Path = "/products",
                Summary = "Create product",
                ReturnType = "Task<Product>",
                Tag = "Products"
            }
        };

        // Act
        generator.GenerateReadme(_testOutputDirectory, models, endpoints);

        // Assert
        var readmePath = Path.Combine(_testOutputDirectory, "README.md");
        var content = File.ReadAllText(readmePath);
        content.Should().Contain("GetProductsAsync");
        content.Should().Contain("CreateProductAsync");
    }
}
