using System.IO.Abstractions;
using Empowered.Dataverse.Webresources.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Empowered.Dataverse.Webresources.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebresources(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddLogging()
            .AddScoped<IDataverseService, DataverseService>()
            .AddScoped<IPushService, PushService>()
            .AddSingleton<IFileService, FileService>()
            .AddSingleton<IFileSystem>(new FileSystem());

        return serviceCollection;
    }
}
