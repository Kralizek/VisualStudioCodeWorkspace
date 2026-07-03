using CodeWorkspaceTool;
using CodeWorkspaceTool.Commands.Init;
using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;
namespace Tests.Commands.Init;

[TestFixture]
[TestOf(typeof(InitCommand))]
public class InitCommandTests
{
    private string _tempDirectory = null!;

    [SetUp]
    public void SetUp() => _tempDirectory = Directory.CreateTempSubdirectory().FullName;

    [TearDown]
    public void TearDown() => Directory.Delete(_tempDirectory, recursive: true);

    [Test, AutoDataProvider]
    public async Task Creates_a_new_workspace_file_named_after_the_output_directory(
        [Frozen] IWorkspaceRepository repository, InitCommand sut)
    {
        var settings = new InitCommandSettings { OutputDirectory = _tempDirectory };
        var expectedFileName = Path.GetFileName(_tempDirectory) + WorkspaceFile.Extension;

        var exitCode = await CommandTestHelpers.ExecuteAsync(sut, settings);

        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(0));
            A.CallTo(() => repository.Save(
                    A<WorkspaceDocument>.That.Matches(d => d.Folders.Count == 0),
                    Path.Combine(_tempDirectory, expectedFileName)))
                .MustHaveHappenedOnceExactly();
        });
    }

    [Test, AutoDataProvider]
    public async Task Uses_the_explicit_name_when_given(
        [Frozen] IWorkspaceRepository repository, InitCommand sut)
    {
        var settings = new InitCommandSettings { OutputDirectory = _tempDirectory, Name = "custom" };

        await CommandTestHelpers.ExecuteAsync(sut, settings);

        A.CallTo(() => repository.Save(A<WorkspaceDocument>._, Path.Combine(_tempDirectory, "custom.code-workspace")))
            .MustHaveHappenedOnceExactly();
    }

    [Test, AutoDataProvider]
    public void Throws_when_the_file_already_exists_and_force_was_not_given(
        [Frozen] IWorkspaceRepository repository, InitCommand sut)
    {
        var fileName = Path.GetFileName(_tempDirectory) + WorkspaceFile.Extension;
        File.WriteAllText(Path.Combine(_tempDirectory, fileName), "{}");
        var settings = new InitCommandSettings { OutputDirectory = _tempDirectory };

        Assert.Multiple(() =>
        {
            Assert.That(
                async () => await CommandTestHelpers.ExecuteAsync(sut, settings),
                Throws.TypeOf<CodeWorkspaceException>());
            A.CallTo(() => repository.Save(A<WorkspaceDocument>._, A<string>._)).MustNotHaveHappened();
        });
    }

    [Test, AutoDataProvider]
    public async Task Overwrites_an_existing_file_when_force_is_given(
        [Frozen] IWorkspaceRepository repository, InitCommand sut)
    {
        var fileName = Path.GetFileName(_tempDirectory) + WorkspaceFile.Extension;
        File.WriteAllText(Path.Combine(_tempDirectory, fileName), "{}");
        var settings = new InitCommandSettings { OutputDirectory = _tempDirectory, Force = true };

        var exitCode = await CommandTestHelpers.ExecuteAsync(sut, settings);

        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(0));
            A.CallTo(() => repository.Save(A<WorkspaceDocument>._, A<string>._)).MustHaveHappenedOnceExactly();
        });
    }
}