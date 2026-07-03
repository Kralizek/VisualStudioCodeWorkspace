using CodeWorkspaceTool.Serialization;

using Spectre.Console;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Settings;

public sealed class SettingsUnsetCommand(IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    : Command<SettingsUnsetCommandSettings>
{
    protected override int Execute(CommandContext context, SettingsUnsetCommandSettings settings, CancellationToken cancellationToken)
    {
        var workspacePath = locator.Resolve(settings.Workspace);
        var document = repository.Load(workspacePath);

        if (document.Settings is null || !document.Settings.Remove(settings.Key))
        {
            throw new CodeWorkspaceException($"Setting '{settings.Key}' is not set.");
        }

        if (document.Settings.Count == 0)
        {
            document.Settings = null;
        }

        repository.Save(document, workspacePath);
        AnsiConsole.MarkupLineInterpolated($"[green]Unset[/] {settings.Key}");
        return 0;
    }
}