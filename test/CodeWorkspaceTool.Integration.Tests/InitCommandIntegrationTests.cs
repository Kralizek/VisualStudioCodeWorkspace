using System.Text.Json;

using CodeWorkspaceTool.Commands.Init;

namespace Tests;

[TestFixture]
[TestOf(typeof(InitCommand))]
public class InitCommandIntegrationTests : IntegrationTestBase
{
    [Test]
    public void Creates_a_workspace_file_named_after_the_directory()
    {
        var exitCode = Run("init", "--output", TempDirectory);

        var expectedFile = PathUnder(Path.GetFileName(TempDirectory) + ".code-workspace");
        using var document = JsonDocument.Parse(File.ReadAllText(expectedFile));
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(File.Exists(expectedFile), Is.True);
            Assert.That(document.RootElement.GetProperty("folders").GetArrayLength(), Is.EqualTo(0));
        });
    }

    [Test]
    public void Fails_when_the_file_already_exists_without_force()
    {
        Run("init", "--output", TempDirectory);

        var exitCode = Run("init", "--output", TempDirectory);

        Assert.That(exitCode, Is.Not.EqualTo(0));
    }

    [Test]
    public void Overwrites_with_force()
    {
        Run("init", "--output", TempDirectory);

        var exitCode = Run("init", "--output", TempDirectory, "--force");

        Assert.That(exitCode, Is.EqualTo(0));
    }
}