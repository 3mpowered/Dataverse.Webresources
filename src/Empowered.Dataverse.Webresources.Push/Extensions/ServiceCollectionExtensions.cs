using System.IO.Abstractions;
using Empowered.Dataverse.Connection.Client.Extensions;
using Empowered.Dataverse.Webresources.Push.Services;
using Empowered.Reactive.Extensions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources.Push.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the push webresource services into the collection.
    /// </summary>
    /// <param name="serviceCollection">The used service collection.</param>
    /// <returns>The given service collection.</returns>
    public static IServiceCollection AddPushWebresources(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddScoped<IDataverseService, DataverseService>();
        serviceCollection.TryAddScoped<IPushService, PushService>();
        serviceCollection.TryAddSingleton<IFileService, FileService>();
        serviceCollection.TryAddSingleton<IFileSystem>(new FileSystem());
        return serviceCollection
            .AddLogging()
            .AddDataverseClient<IOrganizationService>()
            .AddEventObservable();
    }
}
