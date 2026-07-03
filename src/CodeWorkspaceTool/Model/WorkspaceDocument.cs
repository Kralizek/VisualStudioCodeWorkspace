using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CodeWorkspaceTool.Model;

public sealed class WorkspaceDocument
{
    [JsonPropertyName("folders")]
    public List<WorkspaceFolder> Folders { get; set; } = [];

    [JsonPropertyName("settings")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? Settings { get; set; }

    [JsonPropertyName("extensions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ExtensionsBlock? Extensions { get; set; }

    /// <summary>
    /// Sections this tool does not model (e.g. "launch", "tasks") are captured here so a
    /// save never discards content the user configured through other means.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalSections { get; set; }
}
