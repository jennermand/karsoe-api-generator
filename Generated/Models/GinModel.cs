using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class GinModel
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("abv")]
    public string? Abv { get; set; }

    [JsonPropertyName("distillery")]
    public string? Distillery { get; set; }

    [JsonPropertyName("distilleryGuess")]
    public string? DistilleryGuess { get; set; }

    [JsonPropertyName("distilleryHomePage")]
    public string? DistilleryHomePage { get; set; }
}
