using FluentAssertions;
using karsoe_api_generator;
using Microsoft.OpenApi.Models;
using Xunit;

namespace karsoe_api_generator.Tests;

public class TypeMapperTests
{
    [Theory]
    [InlineData("integer", "int32", "int")]
    [InlineData("integer", "int64", "long")]
    [InlineData("integer", null, "int")]
    [InlineData("number", "float", "float")]
    [InlineData("number", "double", "double")]
    [InlineData("number", "decimal", "decimal")]
    [InlineData("number", null, "double")]
    [InlineData("string", null, "string")]
    [InlineData("string", "date-time", "DateTime")]
    [InlineData("string", "date", "DateOnly")]
    [InlineData("string", "time", "TimeOnly")]
    [InlineData("string", "uuid", "Guid")]
    [InlineData("string", "byte", "byte[]")]
    [InlineData("string", "binary", "byte[]")]
    [InlineData("boolean", null, "bool")]
    public void MapToCSharpType_WithPrimitiveTypes_ReturnsCorrectType(string type, string? format, string expectedType)
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = type,
            Format = format
        };

        // Act
        var result = TypeMapper.MapToCSharpType(schema);

        // Assert
        result.Should().Be(expectedType);
    }

    [Fact]
    public void MapToCSharpType_WithNullableInteger_ReturnsNullableInt()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = "integer",
            Format = "int32"
        };

        // Act
        var result = TypeMapper.MapToCSharpType(schema, isNullable: true);

        // Assert
        result.Should().Be("int?");
    }

    [Fact]
    public void MapToCSharpType_WithNullableString_ReturnsNullableString()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = "string"
        };

        // Act
        var result = TypeMapper.MapToCSharpType(schema, isNullable: true);

        // Assert
        result.Should().Be("string?");
    }

    [Fact]
    public void MapToCSharpType_WithArray_ReturnsListOfType()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = "array",
            Items = new OpenApiSchema { Type = "string" }
        };

        // Act
        var result = TypeMapper.MapToCSharpType(schema);

        // Assert
        result.Should().Be("List<string>");
    }

    [Fact]
    public void MapToCSharpType_WithArrayOfObjects_ReturnsListOfObject()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = "array",
            Items = new OpenApiSchema { Type = "object" }
        };

        // Act
        var result = TypeMapper.MapToCSharpType(schema);

        // Assert
        result.Should().Be("List<object>");
    }

    [Fact]
    public void MapToCSharpType_WithArrayNoItems_ReturnsListOfObject()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = "array"
        };

        // Act
        var result = TypeMapper.MapToCSharpType(schema);

        // Assert
        result.Should().Be("List<object>");
    }

    [Fact]
    public void MapToCSharpType_WithObjectAndAdditionalProperties_ReturnsDictionary()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = "object",
            AdditionalProperties = new OpenApiSchema { Type = "string" }
        };

        // Act
        var result = TypeMapper.MapToCSharpType(schema);

        // Assert
        result.Should().Be("Dictionary<string, string>");
    }

    [Fact]
    public void MapToCSharpType_WithObjectNoAdditionalProperties_ReturnsObject()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = "object"
        };

        // Act
        var result = TypeMapper.MapToCSharpType(schema);

        // Assert
        result.Should().Be("object");
    }

    [Fact]
    public void MapToCSharpType_WithReference_ReturnsReferencedType()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Reference = new OpenApiReference
            {
                Id = "ProductDTO",
                Type = ReferenceType.Schema
            }
        };

        // Act
        var result = TypeMapper.MapToCSharpType(schema);

        // Assert
        result.Should().Be("ProductDTO");
    }

    [Fact]
    public void MapToCSharpType_WithNullableReference_ReturnsNullableType()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Reference = new OpenApiReference
            {
                Id = "UserDTO",
                Type = ReferenceType.Schema
            }
        };

        // Act
        var result = TypeMapper.MapToCSharpType(schema, isNullable: true);

        // Assert
        result.Should().Be("UserDTO?");
    }

    [Fact]
    public void MapToCSharpType_WithOneOf_ReturnsFirstNonNullType()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            OneOf = new List<OpenApiSchema>
            {
                new OpenApiSchema { Type = "null" },
                new OpenApiSchema { Type = "string" }
            }
        };

        // Act
        var result = TypeMapper.MapToCSharpType(schema);

        // Assert
        result.Should().Be("string?");
    }

    [Fact]
    public void MapToCSharpType_WithFormatButNoType_InfersTypeFromFormat()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Format = "int64"
        };

        // Act
        var result = TypeMapper.MapToCSharpType(schema);

        // Assert
        result.Should().Be("long");
    }

    [Fact]
    public void GetTypeNameFromReference_WithStandardReference_ReturnsTypeName()
    {
        // Arrange
        var referenceId = "#/components/schemas/ProductDTO";

        // Act
        var result = TypeMapper.GetTypeNameFromReference(referenceId);

        // Assert
        result.Should().Be("ProductDTO");
    }

    [Fact]
    public void GetTypeNameFromReference_WithSimpleId_ReturnsId()
    {
        // Arrange
        var referenceId = "SimpleType";

        // Act
        var result = TypeMapper.GetTypeNameFromReference(referenceId);

        // Assert
        result.Should().Be("SimpleType");
    }

    [Theory]
    [InlineData("productName", "ProductName")]
    [InlineData("user_id", "User_id")]
    [InlineData("firstName", "FirstName")]
    [InlineData("is-active", "Is_active")]
    [InlineData("123invalid", "_123invalid")]
    [InlineData("class", "@Class")]
    [InlineData("public", "@Public")]
    [InlineData("string", "@String")]
    public void SanitizePropertyName_WithVariousInputs_ReturnsSanitizedName(string input, string expected)
    {
        // Act
        var result = TypeMapper.SanitizePropertyName(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", "Property")]
    [InlineData("   ", "Property")]
    [InlineData(null, "Property")]
    public void SanitizePropertyName_WithEmptyOrNull_ReturnsDefaultProperty(string? input, string expected)
    {
        // Act
        var result = TypeMapper.SanitizePropertyName(input!);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("product", "Product")]
    [InlineData("ProductDTO", "ProductDTO")]
    [InlineData("user-model", "Usermodel")]
    [InlineData("my class", "Myclass")]
    [InlineData("123Model", "_123Model")]
    public void SanitizeClassName_WithVariousInputs_ReturnsSanitizedName(string input, string expected)
    {
        // Act
        var result = TypeMapper.SanitizeClassName(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", "Model")]
    [InlineData("   ", "Model")]
    [InlineData(null, "Model")]
    public void SanitizeClassName_WithEmptyOrNull_ReturnsDefaultModel(string? input, string expected)
    {
        // Act
        var result = TypeMapper.SanitizeClassName(input!);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void MapToCSharpType_WithUnknownType_ReturnsObject()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = "unknown"
        };

        // Act
        var result = TypeMapper.MapToCSharpType(schema);

        // Assert
        result.Should().Be("object");
    }

    [Fact]
    public void SanitizePropertyName_WithSpecialCharacters_ReplacesWithUnderscore()
    {
        // Arrange
        var input = "test@prop$name#123";

        // Act
        var result = TypeMapper.SanitizePropertyName(input);

        // Assert
        result.Should().Be("Test_prop_name_123");
    }

    [Fact]
    public void SanitizeClassName_WithSpecialCharacters_ReplacesWithUnderscore()
    {
        // Arrange
        var input = "test@class$name#123";

        // Act
        var result = TypeMapper.SanitizeClassName(input);

        // Assert
        result.Should().Be("Test_class_name_123");
    }
}
