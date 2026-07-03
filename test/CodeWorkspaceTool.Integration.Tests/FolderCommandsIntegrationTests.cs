using System.Text.Json;

namespace Tests;

[TestFixture]
public class FolderCommandsIntegrationTests : IntegrationTestBase
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
    public void Add_then_list_then_remove_round_trips_through_the_real_file()
    {
        var apiDir = PathUnder("api");
        Directory.CreateDirectory(apiDir);

        var addExitCode = Run("folder", "add", "--workspace", _workspaceFile, apiDir);
        Assert.That(addExitCode, Is.EqualTo(0));

        using (var afterAdd = ReadWorkspace())
        {
            var folders = afterAdd.RootElement.GetProperty("folders");
            Assert.That(folders.GetArrayLength(), Is.EqualTo(1));
            Assert.That(folders[0].GetProperty("path").GetString(), Is.EqualTo("api"));
        }

        var listExitCode = Run("folder", "list", "--workspace", _workspaceFile);
        Assert.That(listExitCode, Is.EqualTo(0));

        var removeExitCode = Run("folder", "remove", "--workspace", _workspaceFile, apiDir);
        Assert.That(removeExitCode, Is.EqualTo(0));

        using var afterRemove = ReadWorkspace();
        Assert.That(afterRemove.RootElement.GetProperty("folders").GetArrayLength(), Is.EqualTo(0));
    }

    [Test]
    public void Adding_the_same_folder_twice_does_not_duplicate_it()
    {
        var apiDir = PathUnder("api");
        Directory.CreateDirectory(apiDir);

        Run("folder", "add", "--workspace", _workspaceFile, apiDir);
        var secondExitCode = Run("folder", "add", "--workspace", _workspaceFile, apiDir);

        Assert.That(secondExitCode, Is.EqualTo(0));
        using var document = ReadWorkspace();
        Assert.That(document.RootElement.GetProperty("folders").GetArrayLength(), Is.EqualTo(1));
    }

    [Test]
    public void Adding_a_missing_directory_fails_and_leaves_the_file_untouched()
    {
        var before = File.ReadAllText(_workspaceFile);

        var exitCode = Run("folder", "add", "--workspace", _workspaceFile, PathUnder("does-not-exist"));

        Assert.That(exitCode, Is.Not.EqualTo(0));
        Assert.That(File.ReadAllText(_workspaceFile), Is.EqualTo(before));
    }

    [Test]
    public void Removing_a_folder_not_in_the_workspace_fails()
    {
        var exitCode = Run("folder", "remove", "--workspace", _workspaceFile, PathUnder("nope"));

        Assert.That(exitCode, Is.Not.EqualTo(0));
    }

    [Test]
    public void Unmanaged_sections_survive_a_folder_add()
    {
        File.WriteAllText(_workspaceFile, """
            {
              "folders": [],
              "launch": { "configurations": [{ "name": "Launch", "type": "coreclr" }] }
            }
            """);
        var apiDir = PathUnder("api");
        Directory.CreateDirectory(apiDir);

        var exitCode = Run("folder", "add", "--workspace", _workspaceFile, apiDir);

        Assert.That(exitCode, Is.EqualTo(0));
        using var document = ReadWorkspace();
        Assert.That(document.RootElement.GetProperty("launch")
            .GetProperty("configurations")[0].GetProperty("name").GetString(), Is.EqualTo("Launch"));
    }
}