using System.Text.Json;
using CodeWorkspaceTool.Model;

namespace CodeWorkspaceTool.Serialization;

public sealed class JsonWorkspaceRepository : IWorkspaceRepository
{
    // .code-workspace files are hand-edited in VS Code, which tolerates comments and
    // trailing commas; we accept the same on read even though we don't preserve them on write.
    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
    };

    public WorkspaceDocument Load(string path)
    {
        var json = File.ReadAllText(path);
        try
        {
            return JsonSerializer.Deserialize<WorkspaceDocument>(json, ReadOptions)
                   ?? throw new CodeWorkspaceException($"Workspace file '{path}' is empty.");
        }
        catch (JsonException ex)
        {
            throw new CodeWorkspaceException($"Workspace file '{path}' is not valid JSON: {ex.Message}");
        }
    }

    public void Save(WorkspaceDocument document, string path)
    {
        var json = JsonSerializer.Serialize(document, WriteOptions);
        File.WriteAllText(path, json + Environment.NewLine);
    }
}
