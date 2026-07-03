using System.ComponentModel;

using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Settings;

public sealed class SettingsUnsetCommandSettings : WorkspaceCommandSettings
{
    [CommandArgument(0, "<KEY>")]
    [Description("The setting id to remove.")]
    public string Key { get; set; } = string.Empty;
}