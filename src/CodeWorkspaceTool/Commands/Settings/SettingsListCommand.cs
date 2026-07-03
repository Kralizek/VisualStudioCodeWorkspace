using System.Text.Json;

using CodeWorkspaceTool.Serialization;

using Spectre.Console;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Settings;

public sealed class SettingsListCommand(IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    : Command<SettingsListCommandSettings>
{
    protected override int Execute(CommandContext context, SettingsListCommandSettings settings, CancellationToken cancellationToken)
    {
        var workspacePath = locator.Resolve(settings.Workspace);
        var document = repository.Load(workspacePath);

        if (settings.Format == OutputFormat.Json)
        {
            AnsiConsole.WriteLine(JsonSerializer.Serialize(document.Settings, new JsonSerializerOptions { WriteIndented = true }));
            return 0;
        }

        if (document.Settings is null || document.Settings.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]No settings configured.[/]");
            return 0;
        }

        var table = new Table().AddColumn("Key").AddColumn("Value");
        foreach (var (key, value) in document.Settings)
        {
            table.AddRow(key.EscapeMarkup(), (value?.ToJsonString() ?? "null").EscapeMarkup());
        }

        AnsiConsole.Write(table);
        return 0;
    }
}