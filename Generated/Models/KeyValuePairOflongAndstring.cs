using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class KeyValuePairOflongAndstring
{
    [JsonPropertyName("key")]
    [Required]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object Key { get; set; }

    [JsonPropertyName("value")]
    [Required]
    public string Value { get; set; }
}
