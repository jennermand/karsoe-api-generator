using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class SharedAlbumDTO
{
    [JsonPropertyName("id")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? Id { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("images")]
    public List<ImageDTO> Images { get; set; }
}
