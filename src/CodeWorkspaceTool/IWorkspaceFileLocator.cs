namespace CodeWorkspaceTool;

public interface IWorkspaceFileLocator
{
    /// <summary>
    /// Resolves the workspace file to operate on: the explicit path if given, otherwise the
    /// single *.code-workspace file in the current directory (mirroring `dotnet build`'s
    /// project-file discovery).
    /// </summary>
    string Resolve(string? explicitPath);
}
