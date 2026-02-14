using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class ProductRelationsDuplicateDTO
{
    [JsonPropertyName("productid")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? Productid { get; set; }

    [JsonPropertyName("relation")]
    [RegularExpression(@"^-?(?:0|[1-9]\\d*)$")]
    public object? Relation { get; set; }

    [JsonPropertyName("created")]
    public DateTime? Created { get; set; }

    [JsonPropertyName("updated")]
    public DateTime? Updated { get; set; }

    [JsonPropertyName("deleted")]
    public DateTime? Deleted { get; set; }
}
