using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class ProductDTO
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

    [JsonPropertyName("webshopName")]
    public string? WebshopName { get; set; }

    [JsonPropertyName("relatedProductId")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? RelatedProductId { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("visible")]
    public bool? Visible { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("price")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)(?:\\.\\d+)?(?:[eE][+-]?\\d+)?$")]
    public object? Price { get; set; }

    [JsonPropertyName("sourceUrl")]
    public string? SourceUrl { get; set; }

    [JsonPropertyName("distilleryId")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? DistilleryId { get; set; }

    [JsonPropertyName("webshopId")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? WebshopId { get; set; }

    [JsonPropertyName("lastUpdate")]
    public DateTime? LastUpdate { get; set; }

    [JsonPropertyName("json")]
    public string? Json { get; set; }

    [JsonPropertyName("imageguid")]
    public string? Imageguid { get; set; }

    [JsonPropertyName("distillery")]
    public object? Distillery { get; set; }

    [JsonPropertyName("images")]
    public List<ImageDTO> Images { get; set; }

    [JsonPropertyName("prices")]
    public List<PriceDTO> Prices { get; set; }

    [JsonPropertyName("producttypeNavigation")]
    public object? ProducttypeNavigation { get; set; }

    [JsonPropertyName("webshop")]
    public object? Webshop { get; set; }
}
