using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.NUnit3;

namespace CodeWorkspaceTool.Tests;

public interface IFixtureCustomization
{
    void Customize(IFixture fixture);
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class AutoDataProviderAttribute() : AutoDataAttribute(() => FixtureFactory.CreateFixture());

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class AutoDataProviderAttribute<TCustomization>() : AutoDataAttribute(() => FixtureFactory.CreateFixture(new TCustomization()))
    where TCustomization : IFixtureCustomization, new();

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class InlineAutoDataProviderAttribute(params object[] args) : InlineAutoDataAttribute(() => FixtureFactory.CreateFixture(), args);

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class InlineAutoDataProviderAttribute<TCustomization>(params object[] args)
    : InlineAutoDataAttribute(() => FixtureFactory.CreateFixture(new TCustomization()), args)
    where TCustomization : IFixtureCustomization, new();

public static partial class FixtureFactory
{
    public static IFixture CreateFixture(params IFixtureCustomization[] customizations)
    {
        var fixture = new Fixture();

        fixture.Customize(new AutoFakeItEasyCustomization
        {
            ConfigureMembers = true,
            GenerateDelegates = true,
        });

        ConfigureProjectSpecific(fixture);

        foreach (var customization in customizations)
        {
            customization.Customize(fixture);
        }

        return fixture;
    }

    static partial void ConfigureProjectSpecific(IFixture fixture);
}