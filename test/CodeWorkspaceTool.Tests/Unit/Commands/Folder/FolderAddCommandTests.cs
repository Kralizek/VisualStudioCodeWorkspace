using CodeWorkspaceTool.Commands.Folder;
using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;

namespace CodeWorkspaceTool.Tests.Unit.Commands.Folder;

[TestFixture]
public class FolderAddCommandTests
{
    private const string WorkspacePathFake = "/workspace/demo.code-workspace";
    private const string WorkspaceDirectory = "/workspace";

    private string _tempRoot = null!;

    [SetUp]
    public void SetUp() => _tempRoot = Directory.CreateTempSubdirectory().FullName;

    [TearDown]
    public void TearDown() => Directory.Delete(_tempRoot, recursive: true);

    private string CreateSubdirectory(string name)
    {
        var path = Path.Combine(_tempRoot, name);
        Directory.CreateDirectory(path);
        return path;
    }

    [Test, AutoFakeItEasyData]
    public async Task Adds_a_new_folder_and_saves_the_document(
        IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var folderPath = CreateSubdirectory("api");
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(new WorkspaceDocument());

        WorkspaceDocument? saved = null;
        A.CallTo(() => repository.Save(A<WorkspaceDocument>._, WorkspacePathFake))
            .Invokes((WorkspaceDocument d, string _) => saved = d);

        var command = new FolderAddCommand(locator, repository);
        var settings = new FolderAddCommandSettings { Paths = [folderPath] };

        var exitCode = await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(exitCode, Is.EqualTo(0));
        Assert.That(saved!.Folders, Has.Count.EqualTo(1));
        Assert.That(WorkspacePath.ToFullPath(WorkspaceDirectory, saved.Folders[0].Path), Is.EqualTo(folderPath));
    }

    [Test, AutoFakeItEasyData]
    public async Task Assigns_the_display_name_when_adding_a_single_folder(
        IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var folderPath = CreateSubdirectory("api");
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(new WorkspaceDocument());

        WorkspaceDocument? saved = null;
        A.CallTo(() => repository.Save(A<WorkspaceDocument>._, WorkspacePathFake))
            .Invokes((WorkspaceDocument d, string _) => saved = d);

        var command = new FolderAddCommand(locator, repository);
        var settings = new FolderAddCommandSettings { Paths = [folderPath], Name = "API" };

        await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(saved!.Folders[0].Name, Is.EqualTo("API"));
    }

    [Test, AutoFakeItEasyData]
    public async Task Skips_a_folder_that_is_already_in_the_workspace_without_duplicating_it(
        IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var folderPath = CreateSubdirectory("api");
        var existingDocument = new WorkspaceDocument
        {
            Folders = [new WorkspaceFolder { Path = WorkspacePath.ToStored(WorkspaceDirectory, folderPath) }],
        };
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(existingDocument);

        var command = new FolderAddCommand(locator, repository);
        var settings = new FolderAddCommandSettings { Paths = [folderPath] };

        await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(existingDocument.Folders, Has.Count.EqualTo(1));
    }

    [Test, AutoFakeItEasyData]
    public void Throws_when_the_directory_does_not_exist(
        IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var missingPath = Path.Combine(_tempRoot, "does-not-exist");
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(new WorkspaceDocument());

        var command = new FolderAddCommand(locator, repository);
        var settings = new FolderAddCommandSettings { Paths = [missingPath] };

        Assert.That(
            async () => await CommandTestHelpers.ExecuteAsync(command, settings),
            Throws.TypeOf<CodeWorkspaceException>());
        A.CallTo(() => repository.Save(A<WorkspaceDocument>._, A<string>._)).MustNotHaveHappened();
    }
}
