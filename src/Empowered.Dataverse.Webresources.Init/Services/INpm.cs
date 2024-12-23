using CliWrap;

namespace Empowered.Dataverse.Webresources.Init.Services;

public interface INpm
{
    Task<CommandResult> Install(string workingDirectory);
    Task<CommandResult> UpgradeDependencies(string workingDirectory);
}
