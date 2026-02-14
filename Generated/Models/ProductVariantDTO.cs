using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class ProductVariantDTO
{
    [JsonPropertyName("id")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("describtion")]
    public string? Describtion { get; set; }

    [JsonPropertyName("produkttypeid")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? Produkttypeid { get; set; }
}
