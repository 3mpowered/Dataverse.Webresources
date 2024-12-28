using Empowered.Dataverse.Webresources.Generate.Extensions;
using Empowered.Dataverse.Webresources.Generate.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using NSubstitute;

namespace Empowered.Dataverse.Webresources.Generate.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void ShouldInvokeGenerateService()
    {
        var services = new ServiceCollection();
        services.AddScoped<IOrganizationService>(_ => Substitute.For<IOrganizationService>());
        services.AddGenerateWebresources();
        var serviceProvider = services.BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IGenerateService>();

        service.Should().NotBeNull();
    }
}
