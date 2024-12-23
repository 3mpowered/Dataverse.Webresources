using CommandDotNet;

namespace Empowered.Dataverse.Webresources.Commands.Services;

public interface IOptionResolver
{
    TOptions Resolve<TOptions, TArguments>(TArguments arguments)
        where TOptions : class
        where TArguments : IArgumentModel;
}
