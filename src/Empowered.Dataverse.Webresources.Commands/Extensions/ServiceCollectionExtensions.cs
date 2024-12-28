using System.IO.Abstractions;
using System.Text.Json;
using Empowered.Dataverse.Connection.Client.Extensions;
using Empowered.Dataverse.Connection.Store.Extensions;
using Empowered.Dataverse.Webresources.Commands.Arguments;
using Empowered.Dataverse.Webresources.Commands.Observers;
using Empowered.Dataverse.Webresources.Commands.Services;
using Empowered.Dataverse.Webresources.Commands.Validation;
using Empowered.Dataverse.Webresources.Init.Extensions;
using Empowered.Dataverse.Webresources.Push.Extensions;
using Empowered.Reactive.Extensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xrm.Sdk;
using Spectre.Console;

namespace Empowered.Dataverse.Webresources.Commands.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebresourceCommand(this IServiceCollection services)
    {
        services.TryAddScoped<IEventObserver, ConsoleObserver>();
        services.TryAddScoped<IOptionResolver, OptionResolver>();
        services.TryAddScoped<IPushOptionWriter, PushOptionWriter>();
        services.TryAddScoped<PushArgumentsValidator>();
        services.TryAddScoped<WebresourceCommand>();
        services.TryAddScoped<IValidator<PushArguments>, PushArgumentsValidator>();
        services.TryAddSingleton<IFileSystem>(new FileSystem());
        services.TryAddSingleton(AnsiConsole.Console);
        return services
            .AddSingleton<JsonSerializerOptions>(_ => new JsonSerializerOptions
            {
                WriteIndented = true,
                AllowTrailingCommas = true
            })
            .AddDataverseClient<IOrganizationService>()
            .AddPushWebresources()
            .AddInitWebresources()
            .AddConnectionStore();
    }
}
