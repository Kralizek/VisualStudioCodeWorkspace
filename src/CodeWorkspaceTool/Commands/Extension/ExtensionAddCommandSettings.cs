using System.ComponentModel;

using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Extension;

public sealed class ExtensionAddCommandSettings : WorkspaceCommandSettings
{
    [CommandArgument(0, "<ID>")]
    [Description("One or more extension identifiers (e.g. ms-dotnettools.csharp).")]
    public string[] Ids { get; set; } = [];

    [CommandOption("--unwanted")]
    [Description("Add to unwantedRecommendations instead of recommendations.")]
    public bool Unwanted { get; set; }
}