using CommandDotNet;
using Empowered.CommandLine.Extensions.Extensions;
using Empowered.Dataverse.Webresources.Commands.Arguments;
using Empowered.Dataverse.Webresources.Core.Services;
using Spectre.Console;

namespace Empowered.Dataverse.Webresources.Commands;

public class WebresourceCommand(IAnsiConsole console, IPushService pushService)
{
    public async Task<int> Push(PushArguments arguments)
    {
        console.Info($"Pushing webresources to solution {arguments.Solution.Italic()}");
        pushService.PushWebresources(arguments.Solution, arguments.Directory, arguments.Recursive,
            arguments.FileExtensions, arguments.PublisherPrefix);
        return await ExitCodes.Success;
    }
}
