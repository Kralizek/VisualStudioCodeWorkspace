using CodeWorkspaceTool.Commands.Folder;
using CodeWorkspaceTool.Commands.Init;
using Spectre.Console;
using Spectre.Console.Cli;

var app = new CommandApp();

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

    config.SetExceptionHandler((ex, _) =>
    {
        AnsiConsole.MarkupLineInterpolated($"[red]Error:[/] {ex.Message}");
        return -1;
    });
});

return app.Run(args);
