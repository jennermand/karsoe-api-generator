using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class GinsDTO
{
    [JsonPropertyName("id")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? Id { get; set; }

    [JsonPropertyName("name")]
    [Required]
    public string Name { get; set; }

    [JsonPropertyName("productType")]
    public string? ProductType { get; set; }

    [JsonPropertyName("productVariant")]
    public string? ProductVariant { get; set; }

    [JsonPropertyName("products")]
    public List<ProductDTO> Products { get; set; }

    [JsonPropertyName("created")]
    public DateTime? Created { get; set; }

    [JsonPropertyName("updated")]
    public DateTime? Updated { get; set; }

    [JsonPropertyName("deleted")]
    public DateTime? Deleted { get; set; }

    [JsonPropertyName("suggested")]
    public DateTime? Suggested { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("problems")]
    public object? Problems { get; set; }

    [JsonPropertyName("sharedAlbum")]
    public object? SharedAlbum { get; set; }

    [JsonPropertyName("distillery")]
    public object? Distillery { get; set; }

    [JsonPropertyName("webshops")]
    public List<WebShopDTO> Webshops { get; set; }
}
