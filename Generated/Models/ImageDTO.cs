using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class ImageDTO
{
    [JsonPropertyName("imageId")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? ImageId { get; set; }

    [JsonPropertyName("imageguid")]
    public string? Imageguid { get; set; }

    [JsonPropertyName("imageType")]
    public string? ImageType { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("recordedAt")]
    public DateTime? RecordedAt { get; set; }

    [JsonPropertyName("productId")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? ProductId { get; set; }

    [JsonPropertyName("localPath")]
    public string? LocalPath { get; set; }
}
