using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Client.Extensions;
using Empowered.Dataverse.Connection.Store.Extensions;
using Empowered.Dataverse.Webresources.Commands.Observers;
using Empowered.Dataverse.Webresources.Commands.Services;
using Empowered.Dataverse.Webresources.Commands.Validation;
using Empowered.Dataverse.Webresources.Push.Extensions;
using Empowered.Reactive.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xrm.Sdk;
using Spectre.Console;

namespace Empowered.Dataverse.Webresources.Commands.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebresourceCommand(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddScoped<IEventObserver, ConsoleObserver>();
        serviceCollection.TryAddScoped<IPushOptionResolver, PushOptionResolver>();
        serviceCollection.TryAddScoped<IPushOptionWriter, PushOptionWriter>();
        serviceCollection.TryAddScoped<PushArgumentsValidator>();
        serviceCollection.TryAddScoped<WebresourceCommand>();
        serviceCollection.TryAddSingleton<IFileSystem>(new FileSystem());
        serviceCollection.TryAddSingleton(AnsiConsole.Console);
        return serviceCollection
            .AddSingleton<JsonSerializerOptions>(_ => new JsonSerializerOptions
            {
                WriteIndented = true,
                AllowTrailingCommas = true
            })
            .AddDataverseClient<IOrganizationService>()
            .AddWebresources()
            .AddConnectionStore();
    }
}
