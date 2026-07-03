using CodeWorkspaceTool.Model;
using CodeWorkspaceTool.Serialization;

namespace CodeWorkspaceTool.Tests.Integration;

[TestFixture]
public class JsonWorkspaceRepositoryTests
{
    private readonly JsonWorkspaceRepository _repository = new();
    private string _tempDirectory = null!;

    [SetUp]
    public void SetUp() => _tempDirectory = Directory.CreateTempSubdirectory().FullName;

    [TearDown]
    public void TearDown() => Directory.Delete(_tempDirectory, recursive: true);

    private string PathTo(string fileName) => Path.Combine(_tempDirectory, fileName);

    [Test]
    public void Save_then_load_round_trips_a_document()
    {
        var path = PathTo("demo.code-workspace");
        var document = new WorkspaceDocument
        {
            Folders = [new WorkspaceFolder { Path = "api", Name = "API" }],
        };

        _repository.Save(document, path);
        var loaded = _repository.Load(path);

        Assert.That(loaded.Folders, Has.Count.EqualTo(1));
        Assert.That(loaded.Folders[0].Path, Is.EqualTo("api"));
        Assert.That(loaded.Folders[0].Name, Is.EqualTo("API"));
    }

    [Test]
    public void Load_tolerates_comments_and_trailing_commas()
    {
        var path = PathTo("demo.code-workspace");
        File.WriteAllText(path, """
            {
              // a JSONC comment, as VS Code itself allows
              "folders": [
                { "path": "api" },
              ],
            }
            """);

        var document = _repository.Load(path);

        Assert.That(document.Folders, Has.Count.EqualTo(1));
        Assert.That(document.Folders[0].Path, Is.EqualTo("api"));
    }

    [Test]
    public void Load_throws_a_friendly_error_for_invalid_json()
    {
        var path = PathTo("demo.code-workspace");
        File.WriteAllText(path, "not json");

        Assert.That(() => _repository.Load(path), Throws.TypeOf<CodeWorkspaceException>());
    }

    [Test]
    public void Save_preserves_unmodeled_sections_from_a_previous_load()
    {
        var path = PathTo("demo.code-workspace");
        File.WriteAllText(path, """
            {
              "folders": [],
              "launch": { "configurations": [{ "name": "Launch", "type": "coreclr" }] }
            }
            """);

        var document = _repository.Load(path);
        document.Folders.Add(new WorkspaceFolder { Path = "api" });
        _repository.Save(document, path);

        var rewritten = File.ReadAllText(path);
        Assert.That(rewritten, Does.Contain("\"launch\""));
        Assert.That(rewritten, Does.Contain("\"coreclr\""));
    }
}
