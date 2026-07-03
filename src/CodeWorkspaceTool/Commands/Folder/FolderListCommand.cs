using System.Text.Json;
using CodeWorkspaceTool.Serialization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Folder;

public sealed class FolderListCommand : Command<FolderListCommandSettings>
{
    protected override int Execute(CommandContext context, FolderListCommandSettings settings, CancellationToken cancellationToken)
    {
        var workspacePath = WorkspaceFileLocator.Resolve(settings.Workspace);
        var workspaceDirectory = Path.GetDirectoryName(workspacePath)!;
        var document = WorkspaceDocumentSerializer.Load(workspacePath);

        if (settings.Format == OutputFormat.Json)
        {
            AnsiConsole.WriteLine(JsonSerializer.Serialize(document.Folders, new JsonSerializerOptions { WriteIndented = true }));
            return 0;
        }

        if (document.Folders.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]No folders in this workspace.[/]");
            return 0;
        }

        var table = new Table().AddColumn("Path").AddColumn("Name").AddColumn("Full path");
        foreach (var folder in document.Folders)
        {
            table.AddRow(
                folder.Path.EscapeMarkup(),
                folder.Name?.EscapeMarkup() ?? "[grey]-[/]",
                WorkspacePath.ToFullPath(workspaceDirectory, folder.Path).EscapeMarkup());
        }

        AnsiConsole.Write(table);
        return 0;
    }
}
