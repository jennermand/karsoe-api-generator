using FluentAssertions;
using karsoe_api_generator;
using Microsoft.OpenApi.Models;
using Xunit;

namespace karsoe_api_generator.Tests;

public class ModelGeneratorTests : IDisposable
{
    private readonly string _testOutputDirectory;
    private readonly GeneratorOptions _options;

    public ModelGeneratorTests()
    {
        _testOutputDirectory = Path.Combine(Path.GetTempPath(), $"ModelGeneratorTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testOutputDirectory);

        _options = new GeneratorOptions
        {
            InputPath = "test.json",
            OutputDirectory = _testOutputDirectory,
            Namespace = "TestNamespace",
            AddValidationAttributes = true
        };
    }

    public void Dispose()
    {
        if (Directory.Exists(_testOutputDirectory))
        {
            Directory.Delete(_testOutputDirectory, true);
        }
    }

    [Fact]
    public void GenerateModels_WithSimpleSchema_CreatesModelFile()
    {
        // Arrange
        var generator = new ModelGenerator(_options);
        var schemas = new Dictionary<string, OpenApiSchema>
        {
            ["Product"] = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["id"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                    ["name"] = new OpenApiSchema { Type = "string" }
                },
                Required = new HashSet<string> { "id", "name" }
            }
        };

        // Act
        generator.GenerateModels(schemas, _testOutputDirectory);

        // Assert
        var modelPath = Path.Combine(_testOutputDirectory, "Models", "Product.cs");
        File.Exists(modelPath).Should().BeTrue();
        
        var content = File.ReadAllText(modelPath);
        content.Should().Contain("namespace TestNamespace.Models;");
        content.Should().Contain("public class Product");
        content.Should().Contain("public int Id { get; set; }");
        content.Should().Contain("public string Name { get; set; }");
    }

    [Fact]
    public void GenerateModels_WithValidationAttributes_IncludesValidation()
    {
        // Arrange
        var generator = new ModelGenerator(_options);
        var schemas = new Dictionary<string, OpenApiSchema>
        {
            ["User"] = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["email"] = new OpenApiSchema 
                    { 
                        Type = "string",
                        Pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$",
                        MinLength = 5,
                        MaxLength = 100
                    },
                    ["age"] = new OpenApiSchema 
                    { 
                        Type = "integer",
                        Minimum = 0,
                        Maximum = 120
                    }
                },
                Required = new HashSet<string> { "email" }
            }
        };

        // Act
        generator.GenerateModels(schemas, _testOutputDirectory);

        // Assert
        var modelPath = Path.Combine(_testOutputDirectory, "Models", "User.cs");
        var content = File.ReadAllText(modelPath);
        content.Should().Contain("using System.ComponentModel.DataAnnotations;");
        content.Should().Contain("[Required]");
        content.Should().Contain("[RegularExpression");
        content.Should().Contain("[MinLength(5)]");
        content.Should().Contain("[MaxLength(100)]");
        content.Should().Contain("[Range(0, 120)]");
    }

    [Fact]
    public void GenerateModels_WithoutValidationAttributes_ExcludesValidation()
    {
        // Arrange
        var optionsWithoutValidation = new GeneratorOptions
        {
            InputPath = "test.json",
            OutputDirectory = _testOutputDirectory,
            Namespace = "TestNamespace",
            AddValidationAttributes = false
        };
        var generator = new ModelGenerator(optionsWithoutValidation);
        var schemas = new Dictionary<string, OpenApiSchema>
        {
            ["Product"] = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["name"] = new OpenApiSchema { Type = "string", MinLength = 1 }
                },
                Required = new HashSet<string> { "name" }
            }
        };

        // Act
        generator.GenerateModels(schemas, _testOutputDirectory);

        // Assert
        var modelPath = Path.Combine(_testOutputDirectory, "Models", "Product.cs");
        var content = File.ReadAllText(modelPath);
        content.Should().NotContain("[Required]");
        content.Should().NotContain("[MinLength");
    }

    [Fact]
    public void GenerateModels_WithDescription_IncludesXmlDocumentation()
    {
        // Arrange
        var generator = new ModelGenerator(_options);
        var schemas = new Dictionary<string, OpenApiSchema>
        {
            ["Product"] = new OpenApiSchema
            {
                Type = "object",
                Description = "Product information",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["name"] = new OpenApiSchema 
                    { 
                        Type = "string",
                        Description = "Product name"
                    }
                }
            }
        };

        // Act
        generator.GenerateModels(schemas, _testOutputDirectory);

        // Assert
        var modelPath = Path.Combine(_testOutputDirectory, "Models", "Product.cs");
        var content = File.ReadAllText(modelPath);
        content.Should().Contain("/// <summary>");
        content.Should().Contain("/// Product information");
        content.Should().Contain("/// Product name");
        content.Should().Contain("/// </summary>");
    }

    [Fact]
    public void GenerateModels_WithJsonPropertyName_AddsJsonPropertyAttribute()
    {
        // Arrange
        var generator = new ModelGenerator(_options);
        var schemas = new Dictionary<string, OpenApiSchema>
        {
            ["Product"] = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["product_name"] = new OpenApiSchema { Type = "string" }
                }
            }
        };

        // Act
        generator.GenerateModels(schemas, _testOutputDirectory);

        // Assert
        var modelPath = Path.Combine(_testOutputDirectory, "Models", "Product.cs");
        var content = File.ReadAllText(modelPath);
        content.Should().Contain("[JsonPropertyName(\"product_name\")]");
        content.Should().Contain("public string? Product_name { get; set; }");
    }

    [Fact]
    public void GenerateModels_WithMultipleSchemas_CreatesAllModels()
    {
        // Arrange
        var generator = new ModelGenerator(_options);
        var schemas = new Dictionary<string, OpenApiSchema>
        {
            ["Product"] = new OpenApiSchema 
            { 
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["id"] = new OpenApiSchema { Type = "integer" }
                }
            },
            ["User"] = new OpenApiSchema 
            { 
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["id"] = new OpenApiSchema { Type = "integer" }
                }
            },
            ["Order"] = new OpenApiSchema 
            { 
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["id"] = new OpenApiSchema { Type = "integer" }
                }
            }
        };

        // Act
        generator.GenerateModels(schemas, _testOutputDirectory);

        // Assert
        File.Exists(Path.Combine(_testOutputDirectory, "Models", "Product.cs")).Should().BeTrue();
        File.Exists(Path.Combine(_testOutputDirectory, "Models", "User.cs")).Should().BeTrue();
        File.Exists(Path.Combine(_testOutputDirectory, "Models", "Order.cs")).Should().BeTrue();
        generator.GeneratedModels.Should().HaveCount(3);
    }

    [Fact]
    public void GenerateModels_WithDuplicateSchemaNames_SkipsDuplicates()
    {
        // Arrange
        var generator = new ModelGenerator(_options);
        var schemas = new Dictionary<string, OpenApiSchema>
        {
            ["Product"] = new OpenApiSchema { Type = "object" }
        };

        // Act
        generator.GenerateModels(schemas, _testOutputDirectory);
        generator.GenerateModels(schemas, _testOutputDirectory); // Generate twice

        // Assert
        generator.GeneratedModels.Should().HaveCount(1);
    }

    [Fact]
    public void GenerateModels_StoresMetadataForGeneratedModels()
    {
        // Arrange
        var generator = new ModelGenerator(_options);
        var schemas = new Dictionary<string, OpenApiSchema>
        {
            ["Product"] = new OpenApiSchema
            {
                Type = "object",
                Description = "Product model",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["id"] = new OpenApiSchema 
                    { 
                        Type = "integer",
                        Description = "Product ID"
                    },
                    ["name"] = new OpenApiSchema 
                    { 
                        Type = "string",
                        Description = "Product name"
                    }
                }
            }
        };

        // Act
        generator.GenerateModels(schemas, _testOutputDirectory);

        // Assert
        var metadata = generator.GeneratedModels;
        metadata.Should().HaveCount(1);
        metadata[0].Name.Should().Be("Product");
        metadata[0].Description.Should().Be("Product model");
        metadata[0].Properties.Should().HaveCount(2);
        metadata[0].Properties[0].Name.Should().Be("Id");
        metadata[0].Properties[0].Type.Should().Be("int?");
        metadata[0].Properties[1].Name.Should().Be("Name");
        metadata[0].Properties[1].Type.Should().Be("string?");
    }

    [Fact]
    public void GenerateModels_WithNestedObjects_GeneratesCorrectTypes()
    {
        // Arrange
        var generator = new ModelGenerator(_options);
        var schemas = new Dictionary<string, OpenApiSchema>
        {
            ["Order"] = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["items"] = new OpenApiSchema 
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
                    },
                    ["metadata"] = new OpenApiSchema
                    {
                        Type = "object",
                        AdditionalProperties = new OpenApiSchema { Type = "string" }
                    }
                }
            }
        };

        // Act
        generator.GenerateModels(schemas, _testOutputDirectory);

        // Assert
        var modelPath = Path.Combine(_testOutputDirectory, "Models", "Order.cs");
        var content = File.ReadAllText(modelPath);
        content.Should().Contain("List<Product>");
        content.Should().Contain("Items");
        content.Should().Contain("Dictionary<string, string>");
        content.Should().Contain("Metadata");
    }

    [Fact]
    public void GenerateModels_WithSpecialCharactersInSchemaName_SanitizesName()
    {
        // Arrange
        var generator = new ModelGenerator(_options);
        var schemas = new Dictionary<string, OpenApiSchema>
        {
            ["Product-DTO"] = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["id"] = new OpenApiSchema { Type = "integer" }
                }
            }
        };

        // Act
        generator.GenerateModels(schemas, _testOutputDirectory);

        // Assert
        var modelPath = Path.Combine(_testOutputDirectory, "Models", "ProductDTO.cs");
        File.Exists(modelPath).Should().BeTrue();
        var content = File.ReadAllText(modelPath);
        content.Should().Contain("public class ProductDTO");
    }

    [Fact]
    public void GenerateModels_WithSwaggerJson_GeneratesRelatedProductDTOWithCorrectTypes()
    {
        // Arrange - use the actual swagger.json
        var swaggerPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "swagger.json");
        if (!File.Exists(swaggerPath))
        {
            // Skip if swagger.json not found (e.g., in different test environment)
            return;
        }

        var options = new GeneratorOptions
        {
            InputPath = swaggerPath,
            OutputDirectory = _testOutputDirectory,
            Namespace = "GeneratedApi",
            AddValidationAttributes = true
        };

        var parser = new OpenApiParser();
        parser.LoadFromFile(swaggerPath);

        var generator = new ModelGenerator(options);

        // Act
        generator.GenerateModels(parser.GetSchemas(), Path.Combine(_testOutputDirectory, "Models"));

        // Assert
        var modelPath = Path.Combine(_testOutputDirectory, "Models", "RelatedProductDTO.cs");
        File.Exists(modelPath).Should().BeTrue();
        var content = File.ReadAllText(modelPath);

        // Check that the oneOf properties are correctly typed
        content.Should().Contain("public ProblemsDTO? Problems { get; set; }");
        content.Should().Contain("public SharedAlbumDTO? SharedAlbum { get; set; }");
        content.Should().Contain("public DistilleryDTO? Distillery { get; set; }");

        // Ensure they are not object?
        content.Should().NotContain("public object? Problems { get; set; }");
        content.Should().NotContain("public object? SharedAlbum { get; set; }");
        content.Should().NotContain("public object? Distillery { get; set; }");
    }
}
