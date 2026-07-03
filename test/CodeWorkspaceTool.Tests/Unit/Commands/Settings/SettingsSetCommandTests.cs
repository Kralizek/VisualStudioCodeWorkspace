using CodeWorkspaceTool.Commands.Settings;
using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;

namespace CodeWorkspaceTool.Tests.Unit.Commands.Settings;

[TestFixture]
public class SettingsSetCommandTests
{
    private const string WorkspacePathFake = "/workspace/demo.code-workspace";

    [Test, AutoFakeItEasyData]
    public async Task Infers_a_bool_value_by_default(IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var document = new WorkspaceDocument();
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var command = new SettingsSetCommand(locator, repository);
        var settings = new SettingsSetCommandSettings { Key = "editor.formatOnSave", Value = "true" };

        var exitCode = await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(exitCode, Is.EqualTo(0));
        Assert.That(document.Settings!["editor.formatOnSave"]!.GetValue<bool>(), Is.True);
    }

    [Test, AutoFakeItEasyData]
    public async Task Forces_a_string_via_the_type_option(IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var document = new WorkspaceDocument();
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var command = new SettingsSetCommand(locator, repository);
        var settings = new SettingsSetCommandSettings
        {
            Key = "editor.tabSize",
            Value = "4",
            Type = SettingValueType.String,
        };

        await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(document.Settings!["editor.tabSize"]!.GetValue<string>(), Is.EqualTo("4"));
    }

    [Test, AutoFakeItEasyData]
    public async Task Overwrites_an_existing_key(IWorkspaceFileLocator locator, IWorkspaceRepository repository)
    {
        var document = new WorkspaceDocument();
        document.Settings = new System.Text.Json.Nodes.JsonObject { ["editor.tabSize"] = 2 };
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var command = new SettingsSetCommand(locator, repository);
        var settings = new SettingsSetCommandSettings { Key = "editor.tabSize", Value = "4" };

        await CommandTestHelpers.ExecuteAsync(command, settings);

        Assert.That(document.Settings["editor.tabSize"]!.GetValue<long>(), Is.EqualTo(4));
    }
}
