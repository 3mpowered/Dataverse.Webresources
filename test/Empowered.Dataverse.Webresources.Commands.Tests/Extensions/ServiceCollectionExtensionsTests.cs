using Empowered.Dataverse.Webresources.Commands.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Shouldly;

namespace Empowered.Dataverse.Webresources.Commands.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void ShouldResolveWebresourceCommand()
    {
        var organizationService = Substitute.For<IOrganizationService>();
        var serviceCollection = new ServiceCollection()
            .AddScoped<IOrganizationService>(_ => organizationService)
            .AddWebresourceCommand();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var webresourceCommand = serviceProvider.GetRequiredService<WebresourceCommand>();

        webresourceCommand.ShouldNotBeNull();
    }
}
