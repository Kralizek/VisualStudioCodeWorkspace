using CodeWorkspaceTool.Commands.Init;
using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;

namespace CodeWorkspaceTool.Tests.Unit.Commands.Init;

[TestFixture]
public class InitCommandTests
{
    private string _tempDirectory = null!;

    [SetUp]
    public void SetUp() => _tempDirectory = Directory.CreateTempSubdirectory().FullName;

    [TearDown]
    public void TearDown() => Directory.Delete(_tempDirectory, recursive: true);

    [Test, AutoFakeItEasyData]
    public async Task Creates_a_new_workspace_file_named_after_the_output_directory(IWorkspaceRepository repository)
    {
        var command = new InitCommand(repository);
        var settings = new InitCommandSettings { OutputDirectory = _tempDirectory };
        var expectedFileName = Path.GetFileName(_tempDirectory) + WorkspaceFile.Extension;

        var exitCode = await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(exitCode, Is.EqualTo(0));
        A.CallTo(() => repository.Save(
                A<WorkspaceDocument>.That.Matches(d => d.Folders.Count == 0),
                Path.Combine(_tempDirectory, expectedFileName)))
            .MustHaveHappenedOnceExactly();
    }

    [Test, AutoFakeItEasyData]
    public async Task Uses_the_explicit_name_when_given(IWorkspaceRepository repository)
    {
        var command = new InitCommand(repository);
        var settings = new InitCommandSettings { OutputDirectory = _tempDirectory, Name = "custom" };

        await CommandTestHelpers.ExecuteAsync(command, settings);

        A.CallTo(() => repository.Save(A<WorkspaceDocument>._, Path.Combine(_tempDirectory, "custom.code-workspace")))
            .MustHaveHappenedOnceExactly();
    }

    [Test, AutoFakeItEasyData]
    public void Throws_when_the_file_already_exists_and_force_was_not_given(IWorkspaceRepository repository)
    {
        var fileName = Path.GetFileName(_tempDirectory) + WorkspaceFile.Extension;
        File.WriteAllText(Path.Combine(_tempDirectory, fileName), "{}");
        var command = new InitCommand(repository);
        var settings = new InitCommandSettings { OutputDirectory = _tempDirectory };

        Assert.That(
            async () => await CommandTestHelpers.ExecuteAsync(command, settings),
            Throws.TypeOf<CodeWorkspaceException>());
        A.CallTo(() => repository.Save(A<WorkspaceDocument>._, A<string>._)).MustNotHaveHappened();
    }

    [Test, AutoFakeItEasyData]
    public async Task Overwrites_an_existing_file_when_force_is_given(IWorkspaceRepository repository)
    {
        var fileName = Path.GetFileName(_tempDirectory) + WorkspaceFile.Extension;
        File.WriteAllText(Path.Combine(_tempDirectory, fileName), "{}");
        var command = new InitCommand(repository);
        var settings = new InitCommandSettings { OutputDirectory = _tempDirectory, Force = true };

        var exitCode = await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(exitCode, Is.EqualTo(0));
        A.CallTo(() => repository.Save(A<WorkspaceDocument>._, A<string>._)).MustHaveHappenedOnceExactly();
    }
}
