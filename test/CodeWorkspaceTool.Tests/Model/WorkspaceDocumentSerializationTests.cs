using System.Text.Json;

using CodeWorkspaceTool.Model;

namespace Tests.Model;

[TestFixture]
[TestOf(typeof(WorkspaceDocument))]
public class WorkspaceDocumentSerializationTests
{
    // Regression test: extension data typed as JsonObject serializes as an invalid, unkeyed
    // nested object on this SDK's reflection-based serializer. Dictionary<string, JsonElement>
    // is the type that actually flattens unmodeled sections back into the parent object.
    [Test]
    public void Unmodeled_sections_round_trip_untouched()
    {
        const string json = """
            {
              "folders": [],
              "launch": { "configurations": [{ "name": "Launch", "type": "coreclr" }] }
            }
            """;

        var document = JsonSerializer.Deserialize<WorkspaceDocument>(json)!;
        var rewritten = JsonSerializer.Serialize(document);

        using var parsed = JsonDocument.Parse(rewritten);
        var hasLaunch = parsed.RootElement.TryGetProperty("launch", out var launch);
        Assert.Multiple(() =>
        {
            Assert.That(hasLaunch, Is.True);
            Assert.That(launch.GetProperty("configurations")[0].GetProperty("name").GetString(), Is.EqualTo("Launch"));
        });
    }

    [Test]
    public void Settings_and_extensions_are_omitted_when_absent()
    {
        var document = new WorkspaceDocument();

        var json = JsonSerializer.Serialize(document);

        Assert.Multiple(() =>
        {
            Assert.That(json, Does.Not.Contain("settings"));
            Assert.That(json, Does.Not.Contain("extensions"));
        });
    }

    [Test]
    public void Folder_name_is_omitted_when_null_but_present_when_set()
    {
        var document = new WorkspaceDocument
        {
            Folders =
            [
                new WorkspaceFolder { Path = "src" },
                new WorkspaceFolder { Path = ".", Name = "Root" },
            ],
        };

        var json = JsonSerializer.Serialize(document);

        using var parsed = JsonDocument.Parse(json);
        var folders = parsed.RootElement.GetProperty("folders");
        Assert.Multiple(() =>
        {
            Assert.That(folders[0].TryGetProperty("name", out _), Is.False);
            Assert.That(folders[1].GetProperty("name").GetString(), Is.EqualTo("Root"));
        });
    }
}