using System.Text.Json.Nodes;
using CodeWorkspaceTool.Commands.Settings;
using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;

namespace CodeWorkspaceTool.Tests.Unit.Commands.Settings;

[TestFixture]
public class SettingsUnsetCommandTests
{
    private const string WorkspacePathFake = "/workspace/demo.code-workspace";

    [Test, AutoFakeItEasyData]
    public async Task Removes_the_key_and_prunes_the_settings_object_when_it_becomes_empty(
        IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var document = new WorkspaceDocument { Settings = new JsonObject { ["editor.formatOnSave"] = true } };
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var command = new SettingsUnsetCommand(locator, repository);
        var settings = new SettingsUnsetCommandSettings { Key = "editor.formatOnSave" };

        var exitCode = await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(exitCode, Is.EqualTo(0));
        Assert.That(document.Settings, Is.Null);
    }

    [Test, AutoFakeItEasyData]
    public async Task Keeps_the_settings_object_when_other_keys_remain(
        IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var document = new WorkspaceDocument
        {
            Settings = new JsonObject { ["editor.formatOnSave"] = true, ["editor.tabSize"] = 4 },
        };
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var command = new SettingsUnsetCommand(locator, repository);
        var settings = new SettingsUnsetCommandSettings { Key = "editor.formatOnSave" };

        await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(document.Settings, Is.Not.Null);
        Assert.That(document.Settings!.ContainsKey("editor.tabSize"), Is.True);
    }

    [Test, AutoFakeItEasyData]
    public void Throws_when_the_key_is_not_set(IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var document = new WorkspaceDocument();
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var command = new SettingsUnsetCommand(locator, repository);
        var settings = new SettingsUnsetCommandSettings { Key = "nope.nope" };

        Assert.That(
            async () => await CommandTestHelpers.ExecuteAsync(command, settings),
            Throws.TypeOf<CodeWorkspaceException>());
    }
}
