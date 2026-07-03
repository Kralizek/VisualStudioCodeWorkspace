using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Folder;

public sealed class FolderAddCommand(IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    : Command<FolderAddCommandSettings>
{
    protected override int Execute(CommandContext context, FolderAddCommandSettings settings, CancellationToken cancellationToken)
    {
        var workspacePath = locator.Resolve(settings.Workspace);
        var workspaceDirectory = Path.GetDirectoryName(workspacePath)!;
        var document = repository.Load(workspacePath);

        var name = settings.Paths.Length == 1 ? settings.Name : null;

        foreach (var path in settings.Paths)
        {
            if (!Directory.Exists(path))
            {
                throw new CodeWorkspaceException($"Directory '{path}' was not found.");
            }

            if (document.Folders.Any(f => WorkspacePath.PointsToSameFolder(workspaceDirectory, f.Path, path)))
            {
                AnsiConsole.MarkupLineInterpolated($"[yellow]Skipped[/] {path} (already in workspace)");
                continue;
            }

            var stored = WorkspacePath.ToStored(workspaceDirectory, path);
            document.Folders.Add(new WorkspaceFolder { Path = stored, Name = name });
            AnsiConsole.MarkupLineInterpolated($"[green]Added[/] {stored}");
        }

        repository.Save(document, workspacePath);
        return 0;
    }
}
