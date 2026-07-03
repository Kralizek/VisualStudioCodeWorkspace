using System.ComponentModel;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Init;

public sealed class InitCommandSettings : CommandSettings
{
    [CommandArgument(0, "[NAME]")]
    [Description("Name for the workspace file, without the .code-workspace extension. Defaults to the output directory's name.")]
    public string? Name { get; set; }

    [CommandOption("-o|--output <DIRECTORY>")]
    [Description("Directory to create the workspace file in. Defaults to the current directory.")]
    public string? OutputDirectory { get; set; }

    [CommandOption("--force")]
    [Description("Overwrite the workspace file if it already exists.")]
    public bool Force { get; set; }
}
