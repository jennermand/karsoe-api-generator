using Microsoft.OpenApi.Models;

namespace karsoe_api_generator;

/// <summary>
/// Maps OpenAPI types to C# types.
/// </summary>
public static class TypeMapper
{
    /// <summary>
    /// Converts an OpenAPI schema to a C# type name.
    /// </summary>
    public static string MapToCSharpType(OpenApiSchema schema, bool isNullable = false)
    {
        if (schema.Reference != null)
        {
            // Referenced type - extract the schema name
            var typeName = GetTypeNameFromReference(schema.Reference.Id);
            return isNullable ? $"{typeName}?" : typeName;
        }

        if (schema.Type == "array")
        {
            var itemType = schema.Items != null 
                ? MapToCSharpType(schema.Items, false) 
                : "object";
            return $"List<{itemType}>";
        }

        if (schema.Type == "object")
        {
            if (schema.AdditionalProperties != null)
            {
                var valueType = MapToCSharpType(schema.AdditionalProperties, false);
                return $"Dictionary<string, {valueType}>";
            }
            return "object";
        }

        // Handle oneOf (used for nullable references in the swagger.json)
        if (schema.OneOf != null && schema.OneOf.Count > 0)
        {
            var nonNullSchema = schema.OneOf.FirstOrDefault(s => s.Type != "null" || s.Reference != null);
            if (nonNullSchema != null)
            {
                return MapToCSharpType(nonNullSchema, true);
            }
        }

        // If type is missing but format is present, infer type from format
        string? schemaType = schema.Type;
        if (string.IsNullOrEmpty(schemaType) && !string.IsNullOrEmpty(schema.Format))
        {
            schemaType = InferTypeFromFormat(schema.Format);
        }

        var baseType = schemaType switch
        {
            "integer" => MapIntegerType(schema.Format),
            "number" => MapNumberType(schema.Format),
            "string" => MapStringType(schema.Format),
            "boolean" => "bool",
            _ => "object"
        };

        // Apply nullability for value types
        if (isNullable && IsValueType(baseType))
        {
            return $"{baseType}?";
        }
        else if (isNullable && !IsValueType(baseType))
        {
            return $"{baseType}?";
        }

        return baseType;
    }

    private static string MapIntegerType(string? format)
    {
        return format switch
        {
            "int64" => "long",
            "int32" => "int",
            _ => "int"
        };
    }

    private static string MapNumberType(string? format)
    {
        return format switch
        {
            "float" => "float",
            "double" => "double",
            "decimal" => "decimal",
            _ => "double"
        };
    }

    private static string MapStringType(string? format)
    {
        return format switch
        {
            "date-time" => "DateTime",
            "date" => "DateOnly",
            "time" => "TimeOnly",
            "uuid" => "Guid",
            "byte" => "byte[]",
            "binary" => "byte[]",
            _ => "string"
        };
    }

    /// <summary>
    /// Infers the OpenAPI type from a format specifier when type is missing.
    /// </summary>
    private static string? InferTypeFromFormat(string? format)
    {
        return format switch
        {
            "int32" or "int64" => "integer",
            "float" or "double" or "decimal" => "number",
            "date-time" or "date" or "time" or "uuid" or "byte" or "binary" => "string",
            _ => null
        };
    }

    private static bool IsValueType(string typeName)
    {
        return typeName switch
        {
            "int" or "long" or "short" or "byte" or "sbyte" or
            "uint" or "ulong" or "ushort" or
            "float" or "double" or "decimal" or
            "bool" or "char" or
            "DateTime" or "DateOnly" or "TimeOnly" or "Guid" => true,
            _ => false
        };
    }

    /// <summary>
    /// Extracts a clean type name from an OpenAPI reference ID.
    /// </summary>
    public static string GetTypeNameFromReference(string referenceId)
    {
        // Reference ID format is typically like: "#/components/schemas/ProductDTO"
        var parts = referenceId.Split('/');
        return parts.Length > 0 ? parts[^1] : referenceId;
    }

    /// <summary>
    /// Sanitizes a property name to be a valid C# identifier.
    /// </summary>
    public static string SanitizePropertyName(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return "Property";

        // Capitalize first letter
        var sanitized = char.ToUpperInvariant(propertyName[0]) + propertyName[1..];

        // Replace invalid characters
        sanitized = new string(sanitized.Select(c => char.IsLetterOrDigit(c) || c == '_' ? c : '_').ToArray());

        // Ensure it doesn't start with a number
        if (char.IsDigit(sanitized[0]))
            sanitized = "_" + sanitized;

        // Avoid C# keywords
        if (IsCSharpKeyword(sanitized))
            sanitized = "@" + sanitized;

        return sanitized;
    }

    /// <summary>
    /// Sanitizes a class name to be a valid C# identifier.
    /// </summary>
    public static string SanitizeClassName(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return "Model";

        var sanitized = className.Replace(" ", "").Replace("-", "");
        
        // Ensure PascalCase
        if (sanitized.Length > 0 && char.IsLower(sanitized[0]))
            sanitized = char.ToUpperInvariant(sanitized[0]) + sanitized[1..];

        // Replace invalid characters
        sanitized = new string(sanitized.Select(c => char.IsLetterOrDigit(c) || c == '_' ? c : '_').ToArray());

        // Ensure it doesn't start with a number
        if (char.IsDigit(sanitized[0]))
            sanitized = "_" + sanitized;

        return sanitized;
    }

    private static bool IsCSharpKeyword(string identifier)
    {
        var keywords = new HashSet<string>
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
            "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
            "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
            "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this",
            "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
            "using", "virtual", "void", "volatile", "while"
        };

        return keywords.Contains(identifier.ToLowerInvariant());
    }
}
