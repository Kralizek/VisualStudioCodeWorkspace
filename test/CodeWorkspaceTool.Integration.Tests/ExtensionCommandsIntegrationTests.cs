using System.Text.Json;

namespace Tests;

[TestFixture]
public class ExtensionCommandsIntegrationTests : IntegrationTestBase
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
    public void Add_recommended_and_unwanted_then_remove_both_prunes_the_block()
    {
        Assert.That(Run("extension", "add", "--workspace", _workspaceFile, "ms-dotnettools.csharp"), Is.EqualTo(0));
        Assert.That(Run("extension", "add", "--workspace", _workspaceFile, "--unwanted", "ms-vscode.cpptools"), Is.EqualTo(0));

        using (var afterAdd = ReadWorkspace())
        {
            var extensions = afterAdd.RootElement.GetProperty("extensions");
            Assert.That(extensions.GetProperty("recommendations")[0].GetString(), Is.EqualTo("ms-dotnettools.csharp"));
            Assert.That(extensions.GetProperty("unwantedRecommendations")[0].GetString(), Is.EqualTo("ms-vscode.cpptools"));
        }

        Run("extension", "remove", "--workspace", _workspaceFile, "ms-dotnettools.csharp");
        Run("extension", "remove", "--workspace", _workspaceFile, "--unwanted", "ms-vscode.cpptools");

        using var afterRemove = ReadWorkspace();
        Assert.That(afterRemove.RootElement.TryGetProperty("extensions", out _), Is.False);
    }

    [Test]
    public void Removing_an_extension_not_present_fails()
    {
        var exitCode = Run("extension", "remove", "--workspace", _workspaceFile, "nope.nope");

        Assert.That(exitCode, Is.Not.EqualTo(0));
    }

    [Test]
    public void List_succeeds_in_table_and_json_format()
    {
        Run("extension", "add", "--workspace", _workspaceFile, "ms-dotnettools.csharp");

        Assert.Multiple(() =>
        {
            Assert.That(Run("extension", "list", "--workspace", _workspaceFile), Is.EqualTo(0));
            Assert.That(Run("extension", "list", "--workspace", _workspaceFile, "--format", "json"), Is.EqualTo(0));
        });
    }
}