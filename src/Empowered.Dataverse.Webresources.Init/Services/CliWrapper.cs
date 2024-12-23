using CliWrap;

namespace Empowered.Dataverse.Webresources.Init.Services;

internal class CliWrapper : ICliWrapper
{
    private readonly Command _npm = Cli.Wrap("npm");

    public async Task<CommandResult> NpmInstall(string workingDirectory) => await _npm
        .WithArguments(args => args
            .Add("install")
            .Add("--force")
        )
        .WithWorkingDirectory(workingDirectory)
        .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
        .WithStandardErrorPipe(PipeTarget.ToDelegate(Console.WriteLine))
        .ExecuteAsync();

    public async Task<CommandResult> NpmUpgradeDependencies(string workingDirectory) => await _npm.WithArguments(
            args => args
                .Add("run")
                .Add("dependencies:upgrade")
        )
        .WithWorkingDirectory(workingDirectory)
        .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
        .WithStandardErrorPipe(PipeTarget.ToDelegate(Console.WriteLine))
        .ExecuteAsync();

    public async Task<CommandResult> VsCodeOpen(string workingDirectory) => await Cli.Wrap("code")
        .WithArguments(workingDirectory)
        .WithWorkingDirectory(workingDirectory)
        .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
        .WithStandardErrorPipe(PipeTarget.ToDelegate(Console.WriteLine))
        .ExecuteAsync();
}
