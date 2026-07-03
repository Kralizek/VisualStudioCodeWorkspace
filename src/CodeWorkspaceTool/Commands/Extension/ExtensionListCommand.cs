using System.Text.Json;

using CodeWorkspaceTool.Serialization;

using Spectre.Console;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Extension;

public sealed class ExtensionListCommand(IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    : Command<ExtensionListCommandSettings>
{
    protected override int Execute(CommandContext context, ExtensionListCommandSettings settings, CancellationToken cancellationToken)
    {
        var workspacePath = locator.Resolve(settings.Workspace);
        var document = repository.Load(workspacePath);

        if (settings.Format == OutputFormat.Json)
        {
            AnsiConsole.WriteLine(JsonSerializer.Serialize(document.Extensions, new JsonSerializerOptions { WriteIndented = true }));
            return 0;
        }

        var recommended = document.Extensions?.Recommendations ?? [];
        var unwanted = document.Extensions?.UnwantedRecommendations ?? [];

        if (recommended.Count == 0 && unwanted.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]No extensions configured.[/]");
            return 0;
        }

        var table = new Table().AddColumn("Id").AddColumn("Kind");
        foreach (var id in recommended)
        {
            table.AddRow(id.EscapeMarkup(), "Recommended");
        }

        foreach (var id in unwanted)
        {
            table.AddRow(id.EscapeMarkup(), "Unwanted");
        }

        AnsiConsole.Write(table);
        return 0;
    }
}