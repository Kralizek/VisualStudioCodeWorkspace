using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Tests.Unit;

/// <summary>
/// Command&lt;TSettings&gt;.Execute is protected, but it's reachable through the public
/// ICommand.ExecuteAsync it implements explicitly - so commands can be unit tested directly,
/// without going through argument parsing or a real CommandApp.
/// </summary>
internal static class CommandTestHelpers
{
    public static Task<int> ExecuteAsync(ICommand command, CommandSettings settings) =>
        command.ExecuteAsync(CreateContext(), settings, CancellationToken.None);

    private static CommandContext CreateContext() =>
        new([], A.Fake<IRemainingArguments>(), "test", null);
}
