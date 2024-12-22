using CommandDotNet.DataAnnotations;
using CommandDotNet.FluentValidation;
using Empowered.CommandLine.Extensions;
using Empowered.CommandLine.Extensions.Dataverse;
using Empowered.Dataverse.Connection.Client.Extensions;
using Empowered.Dataverse.Webresources.Commands;
using Empowered.Dataverse.Webresources.Commands.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources;

public static class Program
{
    public static void Main(string[] args) =>
        new EmpoweredAppRunner<WebresourceCommand>("3mpwrd-webresources", Configure)
            .UseDataAnnotationValidations()
            .UseFluentValidation()
            .UseDataverseConnectionTest<IOrganizationService>()
            .Run(args);

    private static void Configure(IServiceCollection collection, IConfigurationBuilder builder)
    {
        collection.AddWebresourceCommand();
        builder.AddDataverseConnectionSource();
    }
}
