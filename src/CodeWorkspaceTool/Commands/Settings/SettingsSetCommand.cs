using System.Text.Json.Nodes;

using CodeWorkspaceTool.Serialization;

using Spectre.Console;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Settings;

public sealed class SettingsSetCommand(IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    : Command<SettingsSetCommandSettings>
{
    protected override int Execute(CommandContext context, SettingsSetCommandSettings settings, CancellationToken cancellationToken)
    {
        var workspacePath = locator.Resolve(settings.Workspace);
        var document = repository.Load(workspacePath);

        var value = SettingValueParser.Parse(settings.Value, settings.Type);
        document.Settings ??= new JsonObject();
        document.Settings[settings.Key] = value;

        repository.Save(document, workspacePath);
        AnsiConsole.MarkupLineInterpolated($"[green]Set[/] {settings.Key} = {value.ToJsonString()}");
        return 0;
    }
}