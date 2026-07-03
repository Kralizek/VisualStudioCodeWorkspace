using CodeWorkspaceTool;
using CodeWorkspaceTool.Serialization;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<IWorkspaceFileLocator, FileSystemWorkspaceFileLocator>();
services.AddSingleton<IWorkspaceRepository, JsonWorkspaceRepository>();

var app = CodewsCommandApp.Create(new TypeRegistrar(services));
return app.Run(args);
