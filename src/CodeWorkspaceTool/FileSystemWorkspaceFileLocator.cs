namespace CodeWorkspaceTool;

public sealed class FileSystemWorkspaceFileLocator : IWorkspaceFileLocator
{
    public string Resolve(string? explicitPath)
    {
        if (explicitPath is not null)
        {
            var fullPath = Path.GetFullPath(explicitPath);
            if (!File.Exists(fullPath))
            {
                throw new CodeWorkspaceException($"Workspace file '{explicitPath}' was not found.");
            }

            return fullPath;
        }

        var cwd = Directory.GetCurrentDirectory();
        var candidates = Directory.GetFiles(cwd, $"*{WorkspaceFile.Extension}");

        return candidates.Length switch
        {
            0 => throw new CodeWorkspaceException(
                $"No {WorkspaceFile.Extension} file was found in '{cwd}'. Specify one with --workspace, or create one with 'codews init'."),
            1 => candidates[0],
            _ => throw new CodeWorkspaceException(
                $"Multiple {WorkspaceFile.Extension} files were found in '{cwd}'. Specify which one to use with --workspace."),
        };
    }
}
