using Empowered.Dataverse.Webresources.Push.Extensions;
using Empowered.Dataverse.Webresources.Push.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using NSubstitute;

namespace Empowered.Dataverse.Webresources.Push.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void ShouldAddWebresources()
    {
        var collection = new ServiceCollection();
        var serviceProvider = collection
            .AddScoped<IOrganizationService>(_ => Substitute.For<IOrganizationService>())
            .AddWebresources()
            .BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IPushService>();
    }
}
