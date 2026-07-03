using CodeWorkspaceTool.Serialization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Folder;

public sealed class FolderRemoveCommand : Command<FolderRemoveCommandSettings>
{
    protected override int Execute(CommandContext context, FolderRemoveCommandSettings settings, CancellationToken cancellationToken)
    {
        var workspacePath = WorkspaceFileLocator.Resolve(settings.Workspace);
        var workspaceDirectory = Path.GetDirectoryName(workspacePath)!;
        var document = WorkspaceDocumentSerializer.Load(workspacePath);

        foreach (var path in settings.Paths)
        {
            var match = document.Folders
                .FirstOrDefault(f => WorkspacePath.PointsToSameFolder(workspaceDirectory, f.Path, path));

            if (match is null)
            {
                throw new CodeWorkspaceException($"Folder '{path}' is not in the workspace.");
            }

            document.Folders.Remove(match);
            AnsiConsole.MarkupLineInterpolated($"[green]Removed[/] {match.Path}");
        }

        WorkspaceDocumentSerializer.Save(document, workspacePath);
        return 0;
    }
}
