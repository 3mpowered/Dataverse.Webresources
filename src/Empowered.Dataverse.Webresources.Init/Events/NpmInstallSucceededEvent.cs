using CliWrap;
using Empowered.Reactive.Extensions.Events;

namespace Empowered.Dataverse.Webresources.Init.Events;

public class NpmInstallSucceededEvent : IObservableEvent
{
    public required CommandResult Result { get; init; }

    internal static NpmInstallSucceededEvent From(CommandResult result) => new()
    {
        Result = result
    };
}
