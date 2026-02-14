using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class ProductShortDTO
{
    [JsonPropertyName("id")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("created")]
    public DateTime? Created { get; set; }

    [JsonPropertyName("updated")]
    public DateTime? Updated { get; set; }

    [JsonPropertyName("productType")]
    public string? ProductType { get; set; }

    [JsonPropertyName("productTypeId")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? ProductTypeId { get; set; }

    [JsonPropertyName("webshop")]
    public string? Webshop { get; set; }

    [JsonPropertyName("relatedProductId")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? RelatedProductId { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("visible")]
    public bool? Visible { get; set; }
}
