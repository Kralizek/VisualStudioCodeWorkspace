namespace CodeWorkspaceTool.Tests.Unit;

[TestFixture]
public class WorkspacePathTests
{
    [Test]
    public void ToStored_returns_forward_slashed_relative_path_for_nested_folder()
    {
        var result = WorkspacePath.ToStored("/repo", "/repo/src/api");

        Assert.That(result, Is.EqualTo("src/api"));
    }

    [Test]
    public void ToStored_returns_dot_for_the_workspace_directory_itself()
    {
        var result = WorkspacePath.ToStored("/repo", "/repo");

        Assert.That(result, Is.EqualTo("."));
    }

    [Test]
    public void ToStored_returns_relative_ascent_for_a_sibling_directory()
    {
        var result = WorkspacePath.ToStored("/repo/workspace", "/repo/other");

        Assert.That(result, Is.EqualTo("../other"));
    }

    [Test]
    public void ToFullPath_resolves_a_stored_relative_path_against_the_workspace_directory()
    {
        var result = WorkspacePath.ToFullPath("/repo", "src/api");

        Assert.That(result, Is.EqualTo("/repo/src/api"));
    }

    [Test]
    public void ToFullPath_resolves_ascent_segments()
    {
        var result = WorkspacePath.ToFullPath("/repo/workspace", "../other");

        Assert.That(result, Is.EqualTo("/repo/other"));
    }

    [Test]
    public void PointsToSameFolder_is_true_for_equivalent_paths()
    {
        var result = WorkspacePath.PointsToSameFolder("/repo", "src/api", "/repo/src/api");

        Assert.That(result, Is.True);
    }

    [Test]
    public void PointsToSameFolder_is_true_for_the_dot_entry_against_the_workspace_directory()
    {
        var result = WorkspacePath.PointsToSameFolder("/repo", ".", "/repo");

        Assert.That(result, Is.True);
    }

    [Test]
    public void PointsToSameFolder_is_false_for_different_folders()
    {
        var result = WorkspacePath.PointsToSameFolder("/repo", "src/api", "/repo/src/web");

        Assert.That(result, Is.False);
    }
}
