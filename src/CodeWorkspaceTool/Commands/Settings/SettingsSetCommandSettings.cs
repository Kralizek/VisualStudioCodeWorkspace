using System.ComponentModel;

using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Settings;

public sealed class SettingsSetCommandSettings : WorkspaceCommandSettings
{
    [CommandArgument(0, "<KEY>")]
    [Description("The setting id, e.g. editor.formatOnSave.")]
    public string Key { get; set; } = string.Empty;

    [CommandArgument(1, "<VALUE>")]
    [Description("The value to assign.")]
    public string Value { get; set; } = string.Empty;

    [CommandOption("--type <TYPE>")]
    [Description("How to interpret VALUE: auto (default), string, bool, int, number, or json.")]
    [DefaultValue(SettingValueType.Auto)]
    public SettingValueType Type { get; set; }
}