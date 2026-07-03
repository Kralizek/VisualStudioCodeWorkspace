using System.ComponentModel;

using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Extension;

public sealed class ExtensionRemoveCommandSettings : WorkspaceCommandSettings
{
    [CommandArgument(0, "<ID>")]
    [Description("One or more extension identifiers to remove.")]
    public string[] Ids { get; set; } = [];

    [CommandOption("--unwanted")]
    [Description("Remove from unwantedRecommendations instead of recommendations.")]
    public bool Unwanted { get; set; }
}