using Empowered.Dataverse.Connection.Client.Extensions;
using Empowered.Dataverse.Connection.Store.Extensions;
using Empowered.Dataverse.Webresources.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources.Commands.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebresourceCommand(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddDataverseClient<IOrganizationService>()
            .AddWebresources()
            .AddConnectionStore();
        return serviceCollection;
    }
}
