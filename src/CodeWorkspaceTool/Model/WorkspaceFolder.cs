using System.Text.Json.Serialization;

namespace CodeWorkspaceTool.Model;

public sealed class WorkspaceFolder
{
    [JsonPropertyName("path")]
    public required string Path { get; set; }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }
}
