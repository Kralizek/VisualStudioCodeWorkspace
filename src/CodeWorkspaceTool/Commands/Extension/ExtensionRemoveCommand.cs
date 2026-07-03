using CodeWorkspaceTool.Serialization;

using Spectre.Console;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Extension;

public sealed class ExtensionRemoveCommand(IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    : Command<ExtensionRemoveCommandSettings>
{
    protected override int Execute(CommandContext context, ExtensionRemoveCommandSettings settings, CancellationToken cancellationToken)
    {
        var workspacePath = locator.Resolve(settings.Workspace);
        var document = repository.Load(workspacePath);

        var list = settings.Unwanted
            ? document.Extensions?.UnwantedRecommendations
            : document.Extensions?.Recommendations;

        foreach (var id in settings.Ids)
        {
            var match = list?.FirstOrDefault(x => string.Equals(x, id, StringComparison.OrdinalIgnoreCase));
            if (match is null)
            {
                throw new CodeWorkspaceException($"Extension '{id}' is not in the workspace.");
            }

            list!.Remove(match);
            AnsiConsole.MarkupLineInterpolated($"[green]Removed[/] {match}");
        }

        if (list is { Count: 0 })
        {
            if (settings.Unwanted)
            {
                document.Extensions!.UnwantedRecommendations = null;
            }
            else
            {
                document.Extensions!.Recommendations = null;
            }
        }

        if (document.Extensions is { IsEmpty: true })
        {
            document.Extensions = null;
        }

        repository.Save(document, workspacePath);
        return 0;
    }
}