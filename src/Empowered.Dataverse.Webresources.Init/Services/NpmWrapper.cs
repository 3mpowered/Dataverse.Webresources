using CliWrap;
using CliWrap.Buffered;

namespace Empowered.Dataverse.Webresources.Init.Services;


    internal class NpmWrapper : INpm
    {
        private readonly Command _npm = Cli.Wrap("npm");

        public async Task<CommandResult> Install(string workingDirectory) => await _npm
            .WithArguments(args => args
                .Add("install")
                .Add("--force")
            )
            .WithWorkingDirectory(workingDirectory)
            .ExecuteBufferedAsync();

        public async Task<CommandResult> UpgradeDependencies(string workingDirectory) => await _npm.WithArguments(
                args => args
                    .Add("run")
                    .Add("dependencies:upgrade")
            )
            .WithWorkingDirectory(workingDirectory)
            .ExecuteBufferedAsync();
    }

