using CodeWorkspaceTool.Commands.Extension;
using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;

namespace CodeWorkspaceTool.Tests.Unit.Commands.Extension;

[TestFixture]
public class ExtensionAddCommandTests
{
    private const string WorkspacePathFake = "/workspace/demo.code-workspace";

    [Test, AutoFakeItEasyData]
    public async Task Adds_a_recommended_extension(IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var document = new WorkspaceDocument();
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var command = new ExtensionAddCommand(locator, repository);
        var settings = new ExtensionAddCommandSettings { Ids = ["ms-dotnettools.csharp"] };

        var exitCode = await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(exitCode, Is.EqualTo(0));
        Assert.That(document.Extensions!.Recommendations, Is.EqualTo(new[] { "ms-dotnettools.csharp" }));
        Assert.That(document.Extensions.UnwantedRecommendations, Is.Null);
    }

    [Test, AutoFakeItEasyData]
    public async Task Adds_an_unwanted_extension_when_the_flag_is_set(
        IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var document = new WorkspaceDocument();
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var command = new ExtensionAddCommand(locator, repository);
        var settings = new ExtensionAddCommandSettings { Ids = ["ms-vscode.cpptools"], Unwanted = true };

        await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(document.Extensions!.UnwantedRecommendations, Is.EqualTo(new[] { "ms-vscode.cpptools" }));
        Assert.That(document.Extensions.Recommendations, Is.Null);
    }

    [Test, AutoFakeItEasyData]
    public async Task Does_not_duplicate_an_id_already_present_case_insensitively(
        IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var document = new WorkspaceDocument
        {
            Extensions = new ExtensionsBlock { Recommendations = ["ms-dotnettools.csharp"] },
        };
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var command = new ExtensionAddCommand(locator, repository);
        var settings = new ExtensionAddCommandSettings { Ids = ["MS-DOTNETTOOLS.CSHARP"] };

        await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(document.Extensions.Recommendations, Has.Count.EqualTo(1));
    }
}
