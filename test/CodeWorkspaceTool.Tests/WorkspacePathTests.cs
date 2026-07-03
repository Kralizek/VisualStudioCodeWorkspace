using CodeWorkspaceTool;
namespace Tests;

[TestOf(typeof(WorkspacePath))]
public class WorkspacePathTests
{
    [TestFixture]
    public class ToStored
    {
        [Test]
        public void Returns_forward_slashed_relative_path_for_nested_folder()
        {
            var result = WorkspacePath.ToStored("/repo", "/repo/src/api");

            Assert.That(result, Is.EqualTo("src/api"));
        }

        [Test]
        public void Returns_dot_for_the_workspace_directory_itself()
        {
            var result = WorkspacePath.ToStored("/repo", "/repo");

            Assert.That(result, Is.EqualTo("."));
        }

        [Test]
        public void Returns_relative_ascent_for_a_sibling_directory()
        {
            var result = WorkspacePath.ToStored("/repo/workspace", "/repo/other");

            Assert.That(result, Is.EqualTo("../other"));
        }
    }

    [TestFixture]
    public class ToFullPath
    {
        [Test]
        public void Resolves_a_stored_relative_path_against_the_workspace_directory()
        {
            var result = WorkspacePath.ToFullPath("/repo", "src/api");

            Assert.That(result, Is.EqualTo("/repo/src/api"));
        }

        [Test]
        public void Resolves_ascent_segments()
        {
            var result = WorkspacePath.ToFullPath("/repo/workspace", "../other");

            Assert.That(result, Is.EqualTo("/repo/other"));
        }
    }

    [TestFixture]
    public class PointsToSameFolder
    {
        [Test]
        public void Is_true_for_equivalent_paths()
        {
            var result = WorkspacePath.PointsToSameFolder("/repo", "src/api", "/repo/src/api");

            Assert.That(result, Is.True);
        }

        [Test]
        public void Is_true_for_the_dot_entry_against_the_workspace_directory()
        {
            var result = WorkspacePath.PointsToSameFolder("/repo", ".", "/repo");

            Assert.That(result, Is.True);
        }

        [Test]
        public void Is_false_for_different_folders()
        {
            var result = WorkspacePath.PointsToSameFolder("/repo", "src/api", "/repo/src/web");

            Assert.That(result, Is.False);
        }
    }
}