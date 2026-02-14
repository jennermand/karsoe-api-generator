using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ManagerApi.Models;

public class ProblemsDTO
{
    [JsonPropertyName("duplicates")]
    public List<ProductRelationsDuplicateDTO> Duplicates { get; set; }

    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; }

    [JsonPropertyName("warnings")]
    public List<string> Warnings { get; set; }

    [JsonPropertyName("infos")]
    public List<string> Infos { get; set; }

    [JsonPropertyName("hasProblems")]
    public bool? HasProblems { get; set; }
}
