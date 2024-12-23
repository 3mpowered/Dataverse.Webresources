using CliWrap;

namespace Empowered.Dataverse.Webresources.Init.Services;

public interface ICliWrapper
{
    Task<CommandResult> NpmInstall(string workingDirectory);
    Task<CommandResult> NpmUpgradeDependencies(string workingDirectory);
    Task<CommandResult> VsCodeOpen(string workingDirectory);
}
