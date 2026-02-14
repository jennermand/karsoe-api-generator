using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class DecodeTokenRequest
{
    [JsonPropertyName("token")]
    [Required]
    public string Token { get; set; }
}
