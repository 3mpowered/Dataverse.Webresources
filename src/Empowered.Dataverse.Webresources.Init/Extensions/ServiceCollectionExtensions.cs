using System.IO.Abstractions;
using CliWrap;
using Empowered.Dataverse.Connection.Client.Extensions;
using Empowered.Dataverse.Webresources.Init.Services;
using Empowered.Reactive.Extensions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources.Init.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the init webresource services into the collection.
    /// </summary>
    /// <param name="services">The used service collection.</param>
    /// <returns>The given service collection.</returns>
    public static IServiceCollection AddInitWebresources(this IServiceCollection services)
    {
        services.TryAddScoped<IInitService, InitService>();
        services.TryAddSingleton<IFileSystem>(new FileSystem());
        services.TryAddScoped<ICliWrapper, CliWrapper>();
        return services
            .AddLogging()
            .AddDataverseClient<IOrganizationService>()
            .AddEventObservable();
    }
}
