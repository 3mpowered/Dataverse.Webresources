using Empowered.Dataverse.Webresources.Init.Extensions;
using Empowered.Dataverse.Webresources.Init.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using NSubstitute;

namespace Empowered.Dataverse.Webresources.Init.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void ShouldAddInitWebresources()
    {
        var collection = new ServiceCollection();
        var serviceProvider = collection
            .AddScoped<IOrganizationService>(_ => Substitute.For<IOrganizationService>())
            .AddInitWebresources()
            .BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IInitService>();
        service.Should().NotBeNull();
    }
}
