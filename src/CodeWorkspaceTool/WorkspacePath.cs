namespace CodeWorkspaceTool;

/// <summary>
/// Converts between paths as typed by the user (relative to the current directory) and
/// paths as stored in a .code-workspace file (relative to the workspace file's own directory,
/// always using forward slashes as VS Code expects).
/// </summary>
public static class WorkspacePath
{
    public static string ToStored(string workspaceDirectory, string inputPath)
    {
        var full = Path.GetFullPath(inputPath);
        var relative = Path.GetRelativePath(workspaceDirectory, full);
        return relative.Replace(Path.DirectorySeparatorChar, '/');
    }

    public static string ToFullPath(string workspaceDirectory, string storedPath)
    {
        var native = storedPath.Replace('/', Path.DirectorySeparatorChar);
        return Path.GetFullPath(Path.Combine(workspaceDirectory, native));
    }

    public static bool PointsToSameFolder(string workspaceDirectory, string storedPath, string candidatePath)
    {
        var a = ToFullPath(workspaceDirectory, storedPath);
        var b = Path.GetFullPath(candidatePath);
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        return string.Equals(a, b, comparison);
    }
}
