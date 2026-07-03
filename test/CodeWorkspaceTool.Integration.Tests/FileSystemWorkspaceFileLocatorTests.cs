using CodeWorkspaceTool;
namespace Tests;

[TestFixture]
[TestOf(typeof(FileSystemWorkspaceFileLocator))]
[NonParallelizable] // mutates the process-wide current directory
public class FileSystemWorkspaceFileLocatorTests
{
    private readonly FileSystemWorkspaceFileLocator _locator = new();
    private string _tempDirectory = null!;
    private string _originalDirectory = null!;

    [SetUp]
    public void SetUp()
    {
        _tempDirectory = Directory.CreateTempSubdirectory().FullName;
        _originalDirectory = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(_tempDirectory);
    }

    [TearDown]
    public void TearDown()
    {
        Directory.SetCurrentDirectory(_originalDirectory);
        Directory.Delete(_tempDirectory, recursive: true);
    }

    [Test]
    public void Throws_when_no_workspace_file_exists()
    {
        Assert.That(() => _locator.Resolve(null), Throws.TypeOf<CodeWorkspaceException>());
    }

    [Test]
    public void Finds_the_single_workspace_file_in_the_current_directory()
    {
        var expected = Path.Combine(_tempDirectory, "demo.code-workspace");
        File.WriteAllText(expected, "{}");

        var resolved = _locator.Resolve(null);

        Assert.That(resolved, Is.EqualTo(expected));
    }

    [Test]
    public void Throws_when_multiple_workspace_files_exist()
    {
        File.WriteAllText(Path.Combine(_tempDirectory, "a.code-workspace"), "{}");
        File.WriteAllText(Path.Combine(_tempDirectory, "b.code-workspace"), "{}");

        Assert.That(() => _locator.Resolve(null), Throws.TypeOf<CodeWorkspaceException>());
    }

    [Test]
    public void Uses_the_explicit_path_regardless_of_the_current_directory()
    {
        var elsewhere = Directory.CreateTempSubdirectory().FullName;
        try
        {
            var expected = Path.Combine(elsewhere, "explicit.code-workspace");
            File.WriteAllText(expected, "{}");

            var resolved = _locator.Resolve(expected);

            Assert.That(resolved, Is.EqualTo(expected));
        }
        finally
        {
            Directory.Delete(elsewhere, recursive: true);
        }
    }

    [Test]
    public void Throws_when_the_explicit_path_does_not_exist()
    {
        var missing = Path.Combine(_tempDirectory, "missing.code-workspace");

        Assert.That(() => _locator.Resolve(missing), Throws.TypeOf<CodeWorkspaceException>());
    }
}