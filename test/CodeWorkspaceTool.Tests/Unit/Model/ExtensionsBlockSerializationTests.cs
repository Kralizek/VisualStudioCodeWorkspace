using System.Text.Json;
using CodeWorkspaceTool.Model;

namespace CodeWorkspaceTool.Tests.Unit.Model;

[TestFixture]
[TestOf(typeof(ExtensionsBlock))]
public class ExtensionsBlockSerializationTests
{
    // Regression test: IsEmpty is a computed convenience property and previously leaked into
    // the serialized JSON because it lacked [JsonIgnore], corrupting saved .code-workspace files.
    [Test]
    public void IsEmpty_is_not_serialized()
    {
        var block = new ExtensionsBlock { Recommendations = ["ms-dotnettools.csharp"] };

        var json = JsonSerializer.Serialize(block);

        Assert.That(json, Does.Not.Contain("IsEmpty"));
    }

    [Test]
    public void Null_recommendation_lists_are_omitted()
    {
        var block = new ExtensionsBlock();

        var json = JsonSerializer.Serialize(block);

        Assert.That(json, Is.EqualTo("{}"));
    }

    [Test]
    public void Both_lists_round_trip()
    {
        var block = new ExtensionsBlock
        {
            Recommendations = ["ms-dotnettools.csharp"],
            UnwantedRecommendations = ["ms-vscode.cpptools"],
        };

        var json = JsonSerializer.Serialize(block);
        var roundTripped = JsonSerializer.Deserialize<ExtensionsBlock>(json)!;

        Assert.Multiple(() =>
        {
            Assert.That(roundTripped.Recommendations, Is.EqualTo(block.Recommendations));
            Assert.That(roundTripped.UnwantedRecommendations, Is.EqualTo(block.UnwantedRecommendations));
        });
    }
}
