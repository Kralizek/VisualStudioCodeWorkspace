using CodeWorkspaceTool;
using CodeWorkspaceTool.Commands.Folder;
using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;
namespace Tests.Commands.Folder;

[TestFixture]
[TestOf(typeof(FolderRemoveCommand))]
public class FolderRemoveCommandTests
{
    private const string WorkspacePathFake = "/workspace/demo.code-workspace";

    [Test, AutoDataProvider]
    public async Task Removes_a_matching_folder(
        [Frozen] IWorkspaceFileLocator locator, [Frozen] IWorkspaceRepository repository, FolderRemoveCommand sut)
    {
        var document = new WorkspaceDocument { Folders = [new WorkspaceFolder { Path = "api" }] };
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var settings = new FolderRemoveCommandSettings { Paths = ["/workspace/api"] };

        var exitCode = await CommandTestHelpers.ExecuteAsync(sut, settings);

        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(document.Folders, Is.Empty);
            A.CallTo(() => repository.Save(document, WorkspacePathFake)).MustHaveHappenedOnceExactly();
        });
    }

    [Test, AutoDataProvider]
    public void Throws_when_the_folder_is_not_in_the_workspace(
        [Frozen] IWorkspaceFileLocator locator, [Frozen] IWorkspaceRepository repository, FolderRemoveCommand sut)
    {
        var document = new WorkspaceDocument();
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var settings = new FolderRemoveCommandSettings { Paths = ["/workspace/api"] };

        Assert.Multiple(() =>
        {
            Assert.That(
                async () => await CommandTestHelpers.ExecuteAsync(sut, settings),
                Throws.TypeOf<CodeWorkspaceException>());
            A.CallTo(() => repository.Save(A<WorkspaceDocument>._, A<string>._)).MustNotHaveHappened();
        });
    }
}