using System.Reflection;
using Empowered.Dataverse.Webresources.Init.Extensions;
using Empowered.Dataverse.Webresources.Init.Services;
using FluentAssertions;

namespace Empowered.Dataverse.Webresources.Init.Tests.Extensions;

public class AssemblyExtensionsTests
{
    [Fact]
    public void ShouldGetEmbeddedResourceString()
    {
        var webpackConfig = Assembly.GetAssembly(typeof(IInitService))?.GetEmbeddedResource("webpack.config.js");
        webpackConfig.Should().NotBeNull();
    }

    [Fact]
    public void ShouldThrowOnMissingEmbeddedResource()
    {
        const string resourceName = "webpack.config.ts";
        Action actor = () => Assembly.GetAssembly(typeof(IInitService))
            ?.GetEmbeddedResource(resourceName);

        actor.Should().ThrowExactly<ArgumentException>()
            .WithParameterName("resourceName")
            .And.Message
            .Should().StartWith($"Embedded resource '{resourceName}' was not found");
    }
}
