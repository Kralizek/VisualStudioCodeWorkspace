using System.ComponentModel;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands;

/// <summary>
/// Base settings for every command that operates against an existing .code-workspace file.
/// </summary>
public abstract class WorkspaceCommandSettings : CommandSettings
{
    [CommandOption("-w|--workspace <FILE>")]
    [Description("The .code-workspace file to use. Defaults to the single one found in the current directory.")]
    public string? Workspace { get; set; }
}
