using CodeWorkspaceTool.Commands.Extension;
using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;

namespace CodeWorkspaceTool.Tests.Unit.Commands.Extension;

[TestFixture]
[TestOf(typeof(ExtensionRemoveCommand))]
public class ExtensionRemoveCommandTests
{
    private const string WorkspacePathFake = "/workspace/demo.code-workspace";

    [Test, AutoDataProvider]
    public async Task Prunes_the_extensions_block_once_both_lists_are_empty(
        [Frozen] IWorkspaceFileLocator locator, [Frozen] IWorkspaceRepository repository, ExtensionRemoveCommand sut)
    {
        var document = new WorkspaceDocument
        {
            Extensions = new ExtensionsBlock { Recommendations = ["ms-dotnettools.csharp"] },
        };
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var settings = new ExtensionRemoveCommandSettings { Ids = ["ms-dotnettools.csharp"] };

        var exitCode = await CommandTestHelpers.ExecuteAsync(sut, settings);

        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(document.Extensions, Is.Null);
        });
    }

    [Test, AutoDataProvider]
    public async Task Keeps_the_extensions_block_when_the_other_list_still_has_entries(
        [Frozen] IWorkspaceFileLocator locator, [Frozen] IWorkspaceRepository repository, ExtensionRemoveCommand sut)
    {
        var document = new WorkspaceDocument
        {
            Extensions = new ExtensionsBlock
            {
                Recommendations = ["ms-dotnettools.csharp"],
                UnwantedRecommendations = ["ms-vscode.cpptools"],
            },
        };
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var settings = new ExtensionRemoveCommandSettings { Ids = ["ms-dotnettools.csharp"] };

        await CommandTestHelpers.ExecuteAsync(sut, settings);

        Assert.Multiple(() =>
        {
            Assert.That(document.Extensions, Is.Not.Null);
            Assert.That(document.Extensions!.Recommendations, Is.Null);
            Assert.That(document.Extensions.UnwantedRecommendations, Is.EqualTo(new[] { "ms-vscode.cpptools" }));
        });
    }

    [Test, AutoDataProvider]
    public void Throws_when_the_extension_is_not_in_the_workspace(
        [Frozen] IWorkspaceFileLocator locator, [Frozen] IWorkspaceRepository repository, ExtensionRemoveCommand sut)
    {
        var document = new WorkspaceDocument();
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var settings = new ExtensionRemoveCommandSettings { Ids = ["nope.nope"] };

        Assert.That(
            async () => await CommandTestHelpers.ExecuteAsync(sut, settings),
            Throws.TypeOf<CodeWorkspaceException>());
    }
}
