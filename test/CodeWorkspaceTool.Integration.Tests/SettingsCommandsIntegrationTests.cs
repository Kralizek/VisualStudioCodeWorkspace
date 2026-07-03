using System.Text.Json;

namespace Tests;

[TestFixture]
public class SettingsCommandsIntegrationTests : IntegrationTestBase
{
    private string _workspaceFile = null!;

    [SetUp]
    public void SetUp()
    {
        Run("init", "demo", "--output", TempDirectory);
        _workspaceFile = PathUnder("demo.code-workspace");
    }

    private JsonDocument ReadWorkspace() => JsonDocument.Parse(File.ReadAllText(_workspaceFile));

    [Test]
    public void Set_infers_types_and_unset_prunes_the_settings_object()
    {
        Assert.That(Run("settings", "set", "--workspace", _workspaceFile, "editor.formatOnSave", "true"), Is.EqualTo(0));
        Assert.That(Run("settings", "set", "--workspace", _workspaceFile, "editor.tabSize", "4"), Is.EqualTo(0));

        using (var afterSet = ReadWorkspace())
        {
            var settings = afterSet.RootElement.GetProperty("settings");
            Assert.That(settings.GetProperty("editor.formatOnSave").GetBoolean(), Is.True);
            Assert.That(settings.GetProperty("editor.tabSize").GetInt32(), Is.EqualTo(4));
        }

        Run("settings", "unset", "--workspace", _workspaceFile, "editor.formatOnSave");
        Run("settings", "unset", "--workspace", _workspaceFile, "editor.tabSize");

        using var afterUnset = ReadWorkspace();
        Assert.That(afterUnset.RootElement.TryGetProperty("settings", out _), Is.False);
    }

    [Test]
    public void Set_with_explicit_json_type_stores_an_array()
    {
        var exitCode = Run(
            "settings", "set", "--workspace", _workspaceFile,
            "[python].editor.rulers", "[80,120]", "--type", "json");

        Assert.That(exitCode, Is.EqualTo(0));
        using var document = ReadWorkspace();
        var rulers = document.RootElement.GetProperty("settings").GetProperty("[python].editor.rulers");
        Assert.That(rulers.EnumerateArray().Select(e => e.GetInt32()), Is.EqualTo(new[] { 80, 120 }));
    }

    [Test]
    public void Unset_a_key_that_is_not_set_fails()
    {
        var exitCode = Run("settings", "unset", "--workspace", _workspaceFile, "nope.nope");

        Assert.That(exitCode, Is.Not.EqualTo(0));
    }

    [Test]
    public void List_succeeds_in_table_and_json_format()
    {
        Run("settings", "set", "--workspace", _workspaceFile, "editor.formatOnSave", "true");

        Assert.Multiple(() =>
        {
            Assert.That(Run("settings", "list", "--workspace", _workspaceFile), Is.EqualTo(0));
            Assert.That(Run("settings", "list", "--workspace", _workspaceFile, "--format", "json"), Is.EqualTo(0));
        });
    }
}