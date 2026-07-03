using CodeWorkspaceTool.Model;

namespace CodeWorkspaceTool.Serialization;

public interface IWorkspaceRepository
{
    WorkspaceDocument Load(string path);

    void Save(WorkspaceDocument document, string path);
}
