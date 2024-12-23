using CliWrap;
using Empowered.Reactive.Extensions.Events;

namespace Empowered.Dataverse.Webresources.Init.Events;

public class NpmUpgradeSucceededEvent : IObservableEvent
{
    public required CommandResult Result { get; set; }

    internal static NpmUpgradeSucceededEvent From(CommandResult result) => new() { Result = result };
}
