using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Extension;

public sealed class ExtensionAddCommand : Command<ExtensionAddCommandSettings>
{
    protected override int Execute(CommandContext context, ExtensionAddCommandSettings settings, CancellationToken cancellationToken)
    {
        var workspacePath = WorkspaceFileLocator.Resolve(settings.Workspace);
        var document = WorkspaceDocumentSerializer.Load(workspacePath);

        document.Extensions ??= new ExtensionsBlock();
        var list = settings.Unwanted
            ? document.Extensions.UnwantedRecommendations ??= []
            : document.Extensions.Recommendations ??= [];

        foreach (var id in settings.Ids)
        {
            if (list.Any(x => string.Equals(x, id, StringComparison.OrdinalIgnoreCase)))
            {
                AnsiConsole.MarkupLineInterpolated($"[yellow]Skipped[/] {id} (already in workspace)");
                continue;
            }

            list.Add(id);
            AnsiConsole.MarkupLineInterpolated($"[green]Added[/] {id}");
        }

        WorkspaceDocumentSerializer.Save(document, workspacePath);
        return 0;
    }
}
