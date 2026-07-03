using System.ComponentModel;

using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Folder;

public sealed class FolderRemoveCommandSettings : WorkspaceCommandSettings
{
    [CommandArgument(0, "<PATH>")]
    [Description("One or more folder paths to remove.")]
    public string[] Paths { get; set; } = [];
}