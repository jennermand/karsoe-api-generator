using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class PriceDTO
{
    [JsonPropertyName("id")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? Id { get; set; }

    [JsonPropertyName("recordedAt")]
    public DateTime? RecordedAt { get; set; }

    [JsonPropertyName("value")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)(?:\\.\\d+)?(?:[eE][+-]?\\d+)?$")]
    public object? Value { get; set; }

    [JsonPropertyName("productId")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? ProductId { get; set; }

    [JsonPropertyName("webshopId")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? WebshopId { get; set; }
}
