using CodeWorkspaceTool.Commands.Extension;
using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;

namespace CodeWorkspaceTool.Tests.Unit.Commands.Extension;

[TestFixture]
public class ExtensionRemoveCommandTests
{
    private const string WorkspacePathFake = "/workspace/demo.code-workspace";

    [Test, AutoFakeItEasyData]
    public async Task Prunes_the_extensions_block_once_both_lists_are_empty(
        IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var document = new WorkspaceDocument
        {
            Extensions = new ExtensionsBlock { Recommendations = ["ms-dotnettools.csharp"] },
        };
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var command = new ExtensionRemoveCommand(locator, repository);
        var settings = new ExtensionRemoveCommandSettings { Ids = ["ms-dotnettools.csharp"] };

        var exitCode = await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(exitCode, Is.EqualTo(0));
        Assert.That(document.Extensions, Is.Null);
    }

    [Test, AutoFakeItEasyData]
    public async Task Keeps_the_extensions_block_when_the_other_list_still_has_entries(
        IWorkspaceFileLocator locator, IWorkspaceRepository repository)
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

        var command = new ExtensionRemoveCommand(locator, repository);
        var settings = new ExtensionRemoveCommandSettings { Ids = ["ms-dotnettools.csharp"] };

        await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(document.Extensions, Is.Not.Null);
        Assert.That(document.Extensions!.Recommendations, Is.Null);
        Assert.That(document.Extensions.UnwantedRecommendations, Is.EqualTo(new[] { "ms-vscode.cpptools" }));
    }

    [Test, AutoFakeItEasyData]
    public void Throws_when_the_extension_is_not_in_the_workspace(
        IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var document = new WorkspaceDocument();
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var command = new ExtensionRemoveCommand(locator, repository);
        var settings = new ExtensionRemoveCommandSettings { Ids = ["nope.nope"] };

        Assert.That(
            async () => await CommandTestHelpers.ExecuteAsync(command, settings),
            Throws.TypeOf<CodeWorkspaceException>());
    }
}
