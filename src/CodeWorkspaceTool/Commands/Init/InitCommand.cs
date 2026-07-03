using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Commands.Init;

public sealed class InitCommand : Command<InitCommandSettings>
{
    protected override int Execute(CommandContext context, InitCommandSettings settings, CancellationToken cancellationToken)
    {
        var outputDirectory = Path.GetFullPath(settings.OutputDirectory ?? Directory.GetCurrentDirectory());
        var name = settings.Name ?? Path.GetFileName(outputDirectory.TrimEnd(Path.DirectorySeparatorChar));
        var fileName = $"{name}{WorkspaceFileLocator.Extension}";
        var path = Path.Combine(outputDirectory, fileName);

        if (File.Exists(path) && !settings.Force)
        {
            throw new CodeWorkspaceException($"'{fileName}' already exists. Use --force to overwrite.");
        }

        Directory.CreateDirectory(outputDirectory);
        WorkspaceDocumentSerializer.Save(new WorkspaceDocument(), path);

        AnsiConsole.MarkupLineInterpolated($"[green]Created[/] {path}");
        return 0;
    }
}
