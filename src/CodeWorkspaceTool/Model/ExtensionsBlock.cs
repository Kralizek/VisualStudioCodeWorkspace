using System.Text.Json.Serialization;

namespace CodeWorkspaceTool.Model;

public sealed class ExtensionsBlock
{
    [JsonPropertyName("recommendations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Recommendations { get; set; }

    [JsonPropertyName("unwantedRecommendations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? UnwantedRecommendations { get; set; }

    public bool IsEmpty =>
        (Recommendations is null || Recommendations.Count == 0) &&
        (UnwantedRecommendations is null || UnwantedRecommendations.Count == 0);
}
