using CommandDotNet;
using Empowered.CommandLine.Extensions.Extensions;
using Empowered.Dataverse.Webresources.Commands.Arguments;
using Empowered.Dataverse.Webresources.Commands.Services;
using Empowered.Dataverse.Webresources.Push.Model;
using Empowered.Dataverse.Webresources.Push.Services;
using Spectre.Console;

namespace Empowered.Dataverse.Webresources.Commands;

public class WebresourceCommand(
    IAnsiConsole console,
    IPushService pushService,
    IPushOptionResolver optionResolver,
    IPushOptionWriter optionWriter)
{
    public async Task<int> Push(PushArguments arguments)
    {
        var options = optionResolver.Resolve(arguments);
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
}
