using CodeWorkspaceTool.Commands.Extension;
using CodeWorkspaceTool.Commands.Folder;
using CodeWorkspaceTool.Commands.Init;
using CodeWorkspaceTool.Commands.Settings;

using Spectre.Console;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool;

/// <summary>
/// Builds the codews command tree. Shared by Program.cs and by integration tests, so the two
/// can never drift apart.
/// </summary>
public static class CodeWorkspaceCommandApp
{
    public static CommandApp Create(ITypeRegistrar registrar)
    {
        var app = new CommandApp(registrar);

        app.Configure(config =>
        {
            config.SetApplicationName("codews");

            config.AddCommand<InitCommand>("init")
                .WithDescription("Create a new .code-workspace file.");

            config.AddBranch("folder", folder =>
            {
                folder.SetDescription("Manage the folders included in a workspace.");
                folder.AddCommand<FolderAddCommand>("add").WithDescription("Add one or more folders.");
                folder.AddCommand<FolderRemoveCommand>("remove").WithDescription("Remove one or more folders.");
                folder.AddCommand<FolderListCommand>("list").WithDescription("List the folders in a workspace.");
            });

            config.AddBranch("extension", extension =>
            {
                extension.SetDescription("Manage recommended and unwanted extensions.");
                extension.AddCommand<ExtensionAddCommand>("add").WithDescription("Add one or more extensions.");
                extension.AddCommand<ExtensionRemoveCommand>("remove").WithDescription("Remove one or more extensions.");
                extension.AddCommand<ExtensionListCommand>("list").WithDescription("List the extensions in a workspace.");
            });

            config.AddBranch("settings", settings =>
            {
                settings.SetDescription("Manage workspace settings.");
                settings.AddCommand<SettingsSetCommand>("set").WithDescription("Set a setting.");
                settings.AddCommand<SettingsUnsetCommand>("unset").WithDescription("Remove a setting.");
                settings.AddCommand<SettingsListCommand>("list").WithDescription("List the settings in a workspace.");
            });

            config.SetExceptionHandler((ex, _) =>
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Error:[/] {ex.Message}");
                return -1;
            });
        });

        return app;
    }
}