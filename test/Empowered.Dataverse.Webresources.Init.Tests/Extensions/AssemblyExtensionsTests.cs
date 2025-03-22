using System.Reflection;
using Empowered.Dataverse.Webresources.Init.Extensions;
using Empowered.Dataverse.Webresources.Init.Services;
using Shouldly;

namespace Empowered.Dataverse.Webresources.Init.Tests.Extensions;

public class AssemblyExtensionsTests
{
    [Fact]
    public void ShouldGetEmbeddedResourceString()
    {
        var webpackConfig = Assembly.GetAssembly(typeof(IInitService))?.GetEmbeddedResource("webpack.config.js");
        webpackConfig.ShouldNotBeNull();
    }

    [Fact]
    public void ShouldThrowOnMissingEmbeddedResource()
    {
        const string resourceName = "webpack.config.ts";
        var exception = Should.Throw<ArgumentException>(() =>
            Assembly.GetAssembly(typeof(IInitService))!.GetEmbeddedResource(resourceName)
        );

        exception.ParamName.ShouldBe("resourceName");
        exception.Message.ShouldStartWith($"Embedded resource '{resourceName}' was not found");
    }
}
