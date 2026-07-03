using CodeWorkspaceTool.Commands.Folder;
using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;

namespace CodeWorkspaceTool.Tests.Unit.Commands.Folder;

[TestFixture]
public class FolderRemoveCommandTests
{
    private const string WorkspacePathFake = "/workspace/demo.code-workspace";

    [Test, AutoFakeItEasyData]
    public async Task Removes_a_matching_folder(IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var document = new WorkspaceDocument { Folders = [new WorkspaceFolder { Path = "api" }] };
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var command = new FolderRemoveCommand(locator, repository);
        var settings = new FolderRemoveCommandSettings { Paths = ["/workspace/api"] };

        var exitCode = await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(exitCode, Is.EqualTo(0));
        Assert.That(document.Folders, Is.Empty);
        A.CallTo(() => repository.Save(document, WorkspacePathFake)).MustHaveHappenedOnceExactly();
    }

    [Test, AutoFakeItEasyData]
    public void Throws_when_the_folder_is_not_in_the_workspace(
        IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var document = new WorkspaceDocument();
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var command = new FolderRemoveCommand(locator, repository);
        var settings = new FolderRemoveCommandSettings { Paths = ["/workspace/api"] };

        Assert.That(
            async () => await CommandTestHelpers.ExecuteAsync(command, settings),
            Throws.TypeOf<CodeWorkspaceException>());
        A.CallTo(() => repository.Save(A<WorkspaceDocument>._, A<string>._)).MustNotHaveHappened();
    }
}
