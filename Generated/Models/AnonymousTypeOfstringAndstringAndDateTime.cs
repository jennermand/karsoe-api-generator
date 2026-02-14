using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class AnonymousTypeOfstringAndstringAndDateTime
{
    [JsonPropertyName("status")]
    [Required]
    public string Status { get; set; }

    [JsonPropertyName("message")]
    [Required]
    public string Message { get; set; }

    [JsonPropertyName("timestamp")]
    [Required]
    public DateTime Timestamp { get; set; }
}
