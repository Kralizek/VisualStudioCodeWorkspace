using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Folder;

public sealed class FolderAddCommandSettings : WorkspaceCommandSettings
{
    [CommandArgument(0, "<PATH>")]
    [Description("One or more folder paths to add.")]
    public string[] Paths { get; set; } = [];

    [CommandOption("--name <LABEL>")]
    [Description("Display name for the folder. Only valid when adding a single path.")]
    public string? Name { get; set; }

    public override ValidationResult Validate()
    {
        if (Paths.Length > 1 && Name is not null)
        {
            return ValidationResult.Error("--name can only be used when adding a single folder.");
        }

        return base.Validate();
    }
}
