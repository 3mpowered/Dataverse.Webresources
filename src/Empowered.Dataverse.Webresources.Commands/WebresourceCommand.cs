using CommandDotNet;
using Empowered.CommandLine.Extensions.Extensions;
using Empowered.Dataverse.Webresources.Commands.Arguments;
using Empowered.Dataverse.Webresources.Commands.Services;
using Empowered.Dataverse.Webresources.Generate.Model;
using Empowered.Dataverse.Webresources.Generate.Services;
using Empowered.Dataverse.Webresources.Init.Model;
using Empowered.Dataverse.Webresources.Init.Services;
using Empowered.Dataverse.Webresources.Push.Model;
using Empowered.Dataverse.Webresources.Push.Services;
using Spectre.Console;

namespace Empowered.Dataverse.Webresources.Commands;

public class WebresourceCommand(
    IAnsiConsole console,
    IPushService pushService,
    IInitService initService,
    IGenerateService generateService,
    IOptionResolver optionResolver,
    IOptionWriter optionWriter)
{
    public async Task<int> Push(PushArguments arguments)
    {
        var options = optionResolver.Resolve<PushOptions, PushArguments>(arguments);
        var results = pushService.PushWebresources(options);
        console.Success(
            $"Pushed {results.Count.ToString().Italic()} webresources to solution {options.Solution.Italic()}");

        if (arguments.PersistConfiguration != null)
        {
            var targetPath = optionWriter.Write(options, arguments.PersistConfiguration);
            console.Info($"Wrote invoked arguments to the json configuration file {targetPath.FullName.Italic()}");
        }

        return await ExitCodes.Success;
    }

    public async Task<int> Init(InitArguments arguments)
    {
        var options = optionResolver.Resolve<InitOptions, InitArguments>(arguments);
        var projectDirectory = await initService.Init(options);
        console.Success(
            $"Initialized project {arguments.Project.Italic()} in directory {projectDirectory.FullName.Italic()}");
        return await ExitCodes.Success;
    }

    public async Task<int> Generate(GenerateArguments arguments)
    {
        var options = optionResolver.Resolve<GenerateOptions, GenerateArguments>(arguments);
        var directoryInfo = generateService.Generate(options);

        if (arguments.PersistConfiguration != null)
        {
            var targetPath = optionWriter.Write(options, arguments.PersistConfiguration);
            console.Info($"Wrote invoked arguments to the json configuration file {targetPath.FullName.Italic()}");
        }

        return await ExitCodes.Success;
    }
}
