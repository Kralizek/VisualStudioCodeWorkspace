using CodeWorkspaceTool.Commands.Folder;

namespace Tests.Commands.Folder;

[TestFixture]
[TestOf(typeof(FolderAddCommandSettings))]
public class FolderAddCommandSettingsTests
{
    [Test]
    public void Validate_rejects_a_name_when_adding_more_than_one_folder()
    {
        var settings = new FolderAddCommandSettings { Paths = ["a", "b"], Name = "Label" };

        var result = settings.Validate();

        Assert.That(result.Successful, Is.False);
    }

    [Test]
    public void Validate_allows_a_name_when_adding_a_single_folder()
    {
        var settings = new FolderAddCommandSettings { Paths = ["a"], Name = "Label" };

        var result = settings.Validate();

        Assert.That(result.Successful, Is.True);
    }

    [Test]
    public void Validate_allows_multiple_folders_without_a_name()
    {
        var settings = new FolderAddCommandSettings { Paths = ["a", "b"] };

        var result = settings.Validate();

        Assert.That(result.Successful, Is.True);
    }
}