using FluentAssertions;
using karsoe_api_generator;
using Xunit;

namespace karsoe_api_generator.Tests;

public class GeneratorOptionsTests
{
    [Fact]
    public void Parse_WithNoArguments_ReturnsDefaultOptions()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.InputPath.Should().Be("swagger.json");
        options.OutputDirectory.Should().Be("./Generated");
        options.Namespace.Should().Be("GeneratedApi");
        options.GenerateAuthHandler.Should().BeTrue();
        options.EnableRetryPolicy.Should().BeTrue();
        options.EnableLogging.Should().BeTrue();
        options.UseAsyncSuffix.Should().BeTrue();
        options.GenerateReadme.Should().BeTrue();
        options.AddValidationAttributes.Should().BeTrue();
    }

    [Fact]
    public void Parse_WithInputPath_SetsInputPath()
    {
        // Arrange
        var args = new[] { "--input", "custom.json" };

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.InputPath.Should().Be("custom.json");
    }

    [Fact]
    public void Parse_WithShortInputFlag_SetsInputPath()
    {
        // Arrange
        var args = new[] { "-i", "api.yaml" };

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.InputPath.Should().Be("api.yaml");
    }

    [Fact]
    public void Parse_WithOutputDirectory_SetsOutputDirectory()
    {
        // Arrange
        var args = new[] { "--output", "./Output" };

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.OutputDirectory.Should().Be("./Output");
    }

    [Fact]
    public void Parse_WithShortOutputFlag_SetsOutputDirectory()
    {
        // Arrange
        var args = new[] { "-o", "./Client" };

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.OutputDirectory.Should().Be("./Client");
    }

    [Fact]
    public void Parse_WithNamespace_SetsNamespace()
    {
        // Arrange
        var args = new[] { "--namespace", "MyApp.ApiClient" };

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.Namespace.Should().Be("MyApp.ApiClient");
    }

    [Fact]
    public void Parse_WithShortNamespaceFlag_SetsNamespace()
    {
        // Arrange
        var args = new[] { "-n", "Custom.Namespace" };

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.Namespace.Should().Be("Custom.Namespace");
    }

    [Fact]
    public void Parse_WithNoAuth_DisablesAuthHandler()
    {
        // Arrange
        var args = new[] { "--no-auth" };

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.GenerateAuthHandler.Should().BeFalse();
    }

    [Fact]
    public void Parse_WithNoRetry_DisablesRetryPolicy()
    {
        // Arrange
        var args = new[] { "--no-retry" };

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.EnableRetryPolicy.Should().BeFalse();
    }

    [Fact]
    public void Parse_WithNoLogging_DisablesLogging()
    {
        // Arrange
        var args = new[] { "--no-logging" };

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.EnableLogging.Should().BeFalse();
    }

    [Fact]
    public void Parse_WithNoReadme_DisablesReadmeGeneration()
    {
        // Arrange
        var args = new[] { "--no-readme" };

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.GenerateReadme.Should().BeFalse();
    }

    [Fact]
    public void Parse_WithNoValidation_DisablesValidationAttributes()
    {
        // Arrange
        var args = new[] { "--no-validation" };

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.AddValidationAttributes.Should().BeFalse();
    }

    [Fact]
    public void Parse_WithMultipleOptions_SetsAllCorrectly()
    {
        // Arrange
        var args = new[]
        {
            "-i", "openapi.json",
            "-o", "./Generated",
            "-n", "MyApi",
            "--no-auth",
            "--no-retry"
        };

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.InputPath.Should().Be("openapi.json");
        options.OutputDirectory.Should().Be("./Generated");
        options.Namespace.Should().Be("MyApi");
        options.GenerateAuthHandler.Should().BeFalse();
        options.EnableRetryPolicy.Should().BeFalse();
        options.EnableLogging.Should().BeTrue();
    }

    [Fact]
    public void Parse_WithCaseInsensitiveFlags_WorksCorrectly()
    {
        // Arrange
        var args = new[] { "--INPUT", "test.json", "--OUTPUT", "./out" };

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.InputPath.Should().Be("test.json");
        options.OutputDirectory.Should().Be("./out");
    }

    [Fact]
    public void Parse_WithMissingFlagValue_UsesDefault()
    {
        // Arrange
        var args = new[] { "--input" }; // Missing value

        // Act
        var options = GeneratorOptions.Parse(args);

        // Assert
        options.InputPath.Should().Be("swagger.json");
    }
}
