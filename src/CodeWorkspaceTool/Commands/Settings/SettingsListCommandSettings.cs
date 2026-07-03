using System.ComponentModel;

using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Settings;

public sealed class SettingsListCommandSettings : WorkspaceCommandSettings
{
    [CommandOption("--format <FORMAT>")]
    [Description("Output format: table or json.")]
    [DefaultValue(OutputFormat.Table)]
    public OutputFormat Format { get; set; }
}