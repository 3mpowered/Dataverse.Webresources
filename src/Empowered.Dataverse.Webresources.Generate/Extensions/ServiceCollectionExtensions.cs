using System.IO.Abstractions;
using Empowered.Dataverse.Connection.Client.Extensions;
using Empowered.Dataverse.Webresources.Generate.Services;
using Empowered.Reactive.Extensions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources.Generate.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGenerateWebresources(this IServiceCollection services)
    {
        services.TryAddSingleton<IFileSystem>(new FileSystem());
        services.TryAddScoped<IGenerateService, GenerateService>();
        services.TryAddTransient<ITemplateRenderer, TemplateRenderer>();
        return services
            .AddLogging()
            .AddDataverseClient<IOrganizationService>()
            .AddEventObservable();
    }
}
