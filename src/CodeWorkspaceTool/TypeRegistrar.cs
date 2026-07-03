using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

namespace CodeWorkspaceTool;

/// <summary>
/// ITypeRegistrar/ITypeResolver pair backed by Microsoft.Extensions.DependencyInjection, so
/// command classes take their dependencies (IWorkspaceFileLocator, IWorkspaceRepository) via
/// constructor injection instead of calling static services directly. This is the pattern
/// documented by Spectre.Console.Cli itself for DI integration.
/// </summary>
public sealed class TypeRegistrar(IServiceCollection services) : ITypeRegistrar
{
    public void Register(Type service, Type implementation) =>
        services.AddSingleton(service, implementation);

    public void RegisterInstance(Type service, object implementation) =>
        services.AddSingleton(service, implementation);

    public void RegisterLazy(Type service, Func<object> factory) =>
        services.AddSingleton(service, _ => factory());

    public ITypeResolver Build() => new TypeResolver(services.BuildServiceProvider());
}

internal sealed class TypeResolver(IServiceProvider provider) : ITypeResolver, IDisposable
{
    public object? Resolve(Type? type) => type is null ? null : provider.GetService(type);

    public void Dispose()
    {
        if (provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}