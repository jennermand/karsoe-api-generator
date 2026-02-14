using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class TokenRequest
{
    [JsonPropertyName("username")]
    [Required]
    public string Username { get; set; }

    [JsonPropertyName("password")]
    [Required]
    public string Password { get; set; }
}
