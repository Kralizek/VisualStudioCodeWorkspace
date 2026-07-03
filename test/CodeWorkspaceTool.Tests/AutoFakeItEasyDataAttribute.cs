using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.NUnit3;

namespace CodeWorkspaceTool.Tests;

/// <summary>
/// [AutoData] configured so that any parameter typed as an interface (e.g.
/// IWorkspaceFileLocator, IWorkspaceRepository) is auto-generated as a FakeItEasy fake,
/// while plain data (strings, records, settings objects) is generated normally by AutoFixture.
/// </summary>
public sealed class AutoFakeItEasyDataAttribute() : AutoDataAttribute(
    () => new Fixture().Customize(new AutoFakeItEasyCustomization()));
