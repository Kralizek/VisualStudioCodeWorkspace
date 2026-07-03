using System.ComponentModel;

using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Extension;

public sealed class ExtensionListCommandSettings : WorkspaceCommandSettings
{
    [CommandOption("--format <FORMAT>")]
    [Description("Output format: table or json.")]
    [DefaultValue(OutputFormat.Table)]
    public OutputFormat Format { get; set; }
}