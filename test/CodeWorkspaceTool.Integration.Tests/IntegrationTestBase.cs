using CodeWorkspaceTool.Serialization;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

namespace CodeWorkspaceTool.Integration.Tests;

/// <summary>
/// Exercises the real CommandApp (as built by CodewsCommandApp, the same factory Program.cs
/// uses) against real temp directories, with the real file-system-backed services - no fakes.
/// </summary>
public abstract class IntegrationTestBase
{
    protected string TempDirectory { get; private set; } = null!;

    private CommandApp _app = null!;

    [SetUp]
    public void BaseSetUp()
    {
        TempDirectory = Directory.CreateTempSubdirectory().FullName;

        var services = new ServiceCollection();
        services.AddSingleton<IWorkspaceFileLocator, FileSystemWorkspaceFileLocator>();
        services.AddSingleton<IWorkspaceRepository, JsonWorkspaceRepository>();
        _app = CodewsCommandApp.Create(new TypeRegistrar(services));
    }

    [TearDown]
    public void BaseTearDown() => Directory.Delete(TempDirectory, recursive: true);

    protected string PathUnder(params string[] segments) => Path.Combine([TempDirectory, .. segments]);

    protected int Run(params string[] args) => _app.Run(args);
}