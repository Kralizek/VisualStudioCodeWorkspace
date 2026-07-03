namespace CodeWorkspaceTool;

public static class WorkspaceFileLocator
{
    public const string Extension = ".code-workspace";

    /// <summary>
    /// Resolves the workspace file to operate on: the explicit path if given, otherwise the
    /// single *.code-workspace file in the current directory (mirroring `dotnet build`'s
    /// project-file discovery).
    /// </summary>
    public static string Resolve(string? explicitPath)
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
        var candidates = Directory.GetFiles(cwd, $"*{Extension}");

        return candidates.Length switch
        {
            0 => throw new CodeWorkspaceException(
                $"No {Extension} file was found in '{cwd}'. Specify one with --workspace, or create one with 'codews init'."),
            1 => candidates[0],
            _ => throw new CodeWorkspaceException(
                $"Multiple {Extension} files were found in '{cwd}'. Specify which one to use with --workspace."),
        };
    }
}
