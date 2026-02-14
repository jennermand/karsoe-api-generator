using Microsoft.OpenApi.Models;
using System.Text;

namespace karsoe_api_generator;

/// <summary>
/// Generates C# DTO models from OpenAPI schemas.
/// </summary>
public class ModelGenerator
{
    private readonly GeneratorOptions _options;
    private readonly HashSet<string> _generatedModels = new();
    private readonly List<ModelInfo> _modelMetadata = new();

    public IReadOnlyList<ModelInfo> GeneratedModels => _modelMetadata;

    public ModelGenerator(GeneratorOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Generates all DTO models from the schemas.
    /// </summary>
    public void GenerateModels(IDictionary<string, OpenApiSchema> schemas, string outputDirectory)
    {
        var modelsDir = Path.Combine(outputDirectory, "Models");
        Directory.CreateDirectory(modelsDir);

        Console.WriteLine($"\nGenerating models in: {modelsDir}");

        foreach (var schema in schemas)
        {
            GenerateModel(schema.Key, schema.Value, modelsDir);
        }

        Console.WriteLine($"Generated {_generatedModels.Count} unique models (skipped duplicates).");
    }

    private void GenerateModel(string schemaName, OpenApiSchema schema, string outputDirectory)
    {
        var className = TypeMapper.SanitizeClassName(schemaName);

        // Check if model already generated (circular reference protection)
        if (_generatedModels.Contains(className))
        {
            Console.WriteLine($"  Skipping {className} (already generated)");
            return;
        }

        _generatedModels.Add(className);
        Console.WriteLine($"  Generating {className}...");

        var code = GenerateModelCode(className, schema);
        var filePath = Path.Combine(outputDirectory, $"{className}.cs");
        File.WriteAllText(filePath, code);

        // Store metadata for README generation
        _modelMetadata.Add(new ModelInfo
        {
            Name = className,
            Description = schema.Description ?? $"{className} data transfer object",
            Properties = schema.Properties?.Select(p => new PropertyInfo
            {
                Name = TypeMapper.SanitizePropertyName(p.Key),
                Type = TypeMapper.MapToCSharpType(p.Value, IsPropertyNullable(schema, p.Key)),
                Description = p.Value.Description
            }).ToList() ?? new List<PropertyInfo>()
        });
    }

    private string GenerateModelCode(string className, OpenApiSchema schema)
    {
        var sb = new StringBuilder();

        // Using statements
        sb.AppendLine("using System.Text.Json.Serialization;");
        if (_options.AddValidationAttributes)
        {
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        }
        sb.AppendLine();

        // Namespace
        sb.AppendLine($"namespace {_options.Namespace}.Models;");
        sb.AppendLine();

        // XML documentation
        if (!string.IsNullOrWhiteSpace(schema.Description))
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine($"/// {EscapeXmlComment(schema.Description)}");
            sb.AppendLine("/// </summary>");
        }

        // Class declaration
        sb.AppendLine($"public class {className}");
        sb.AppendLine("{");

        // Properties
        if (schema.Properties != null && schema.Properties.Count > 0)
        {
            var properties = schema.Properties.ToList();
            for (int i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                GenerateProperty(sb, property.Key, property.Value, schema);

                // Add blank line between properties except for the last one
                if (i < properties.Count - 1)
                    sb.AppendLine();
            }
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private void GenerateProperty(StringBuilder sb, string propertyName, OpenApiSchema propertySchema, OpenApiSchema parentSchema)
    {
        var sanitizedName = TypeMapper.SanitizePropertyName(propertyName);
        var isNullable = IsPropertyNullable(parentSchema, propertyName);
        var csharpType = TypeMapper.MapToCSharpType(propertySchema, isNullable);

        // XML documentation
        if (!string.IsNullOrWhiteSpace(propertySchema.Description))
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    /// {EscapeXmlComment(propertySchema.Description)}");
            sb.AppendLine("    /// </summary>");
        }

        // JSON property name attribute (if different from C# property name)
        if (sanitizedName != propertyName)
        {
            sb.AppendLine($"    [JsonPropertyName(\"{propertyName}\")]");
        }

        // Validation attributes
        if (_options.AddValidationAttributes)
        {
            if (IsRequired(parentSchema, propertyName) && !isNullable)
            {
                sb.AppendLine("    [Required]");
            }

            if (propertySchema.Pattern != null)
            {
                var escapedPattern = propertySchema.Pattern.Replace("\\", "\\\\").Replace("\"", "\\\"");
                sb.AppendLine($"    [RegularExpression(@\"{escapedPattern}\")]");
            }

            if (propertySchema.MinLength.HasValue)
            {
                sb.AppendLine($"    [MinLength({propertySchema.MinLength.Value})]");
            }

            if (propertySchema.MaxLength.HasValue)
            {
                sb.AppendLine($"    [MaxLength({propertySchema.MaxLength.Value})]");
            }

            if (propertySchema.Minimum.HasValue)
            {
                sb.AppendLine($"    [Range({propertySchema.Minimum.Value}, {propertySchema.Maximum?.ToString() ?? "double.MaxValue"})]");
            }
        }

        // Property declaration
        sb.AppendLine($"    public {csharpType} {sanitizedName} {{ get; set; }}");
    }

    private bool IsRequired(OpenApiSchema schema, string propertyName)
    {
        return schema.Required?.Contains(propertyName) ?? false;
    }

    private bool IsPropertyNullable(OpenApiSchema schema, string propertyName)
    {
        if (schema.Properties == null || !schema.Properties.TryGetValue(propertyName, out var propertySchema))
            return true;

        // If it's in the required list, it's not nullable
        if (schema.Required?.Contains(propertyName) ?? false)
            return false;

        // If the property has nullable flag set
        if (propertySchema.Nullable)
            return true;

        // If it's a reference type (string, class), it's nullable by default in C# nullable context
        var type = propertySchema.Type;
        if (type == "string" || type == "object" || type == "array")
            return true;

        return true; // Default to nullable for optional properties
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
/// Metadata about a generated model.
/// </summary>
public class ModelInfo
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required List<PropertyInfo> Properties { get; set; }
}

/// <summary>
/// Metadata about a model property.
/// </summary>
public class PropertyInfo
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public string? Description { get; set; }
}
