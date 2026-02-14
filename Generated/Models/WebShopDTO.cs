using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class WebShopDTO
{
    [JsonPropertyName("id")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("logoUrl")]
    public string? LogoUrl { get; set; }

    [JsonPropertyName("nickName")]
    public string? NickName { get; set; }
}
