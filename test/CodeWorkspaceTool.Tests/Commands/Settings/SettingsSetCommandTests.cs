using System.Text.Json.Nodes;

using CodeWorkspaceTool.Commands.Settings;
using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;

namespace CodeWorkspaceTool.Tests.Commands.Settings;

[TestFixture]
[TestOf(typeof(SettingsSetCommand))]
public class SettingsSetCommandTests
{
    private const string WorkspacePathFake = "/workspace/demo.code-workspace";

    [Test, AutoDataProvider]
    public async Task Infers_a_bool_value_by_default(
        [Frozen] IWorkspaceFileLocator locator, [Frozen] IWorkspaceRepository repository, SettingsSetCommand sut)
    {
        var document = new WorkspaceDocument();
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var settings = new SettingsSetCommandSettings { Key = "editor.formatOnSave", Value = "true" };

        var exitCode = await CommandTestHelpers.ExecuteAsync(sut, settings);

        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(document.Settings!["editor.formatOnSave"]!.GetValue<bool>(), Is.True);
        });
    }

    [Test, AutoDataProvider]
    public async Task Forces_a_string_via_the_type_option(
        [Frozen] IWorkspaceFileLocator locator, [Frozen] IWorkspaceRepository repository, SettingsSetCommand sut)
    {
        var document = new WorkspaceDocument();
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var settings = new SettingsSetCommandSettings
        {
            Key = "editor.tabSize",
            Value = "4",
            Type = SettingValueType.String,
        };

        await CommandTestHelpers.ExecuteAsync(sut, settings);

        Assert.That(document.Settings!["editor.tabSize"]!.GetValue<string>(), Is.EqualTo("4"));
    }

    [Test, AutoDataProvider]
    public async Task Overwrites_an_existing_key(
        [Frozen] IWorkspaceFileLocator locator, [Frozen] IWorkspaceRepository repository, SettingsSetCommand sut)
    {
        var document = new WorkspaceDocument { Settings = new JsonObject { ["editor.tabSize"] = 2 } };
        A.CallTo(() => locator.Resolve(null)).Returns(WorkspacePathFake);
        A.CallTo(() => repository.Load(WorkspacePathFake)).Returns(document);

        var settings = new SettingsSetCommandSettings { Key = "editor.tabSize", Value = "4" };

        await CommandTestHelpers.ExecuteAsync(sut, settings);

        Assert.That(document.Settings["editor.tabSize"]!.GetValue<long>(), Is.EqualTo(4));
    }
}