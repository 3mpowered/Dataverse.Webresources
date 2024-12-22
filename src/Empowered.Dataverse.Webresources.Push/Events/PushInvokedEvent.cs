using Empowered.Dataverse.Webresources.Push.Model;
using Empowered.Reactive.Extensions.Events;

namespace Empowered.Dataverse.Webresources.Push.Events;

public record PushInvokedEvent : IObservableEvent
{
    public required string SolutionName { get; init; }
    public required DirectoryInfo Directory { get; set; }
    public required PushOptions Options { get; init; }

    internal PushInvokedEvent()
    {
    }

    internal static PushInvokedEvent From(PushOptions options) =>
        new()
        {
            SolutionName = options.Solution,
            Directory = options.DirectoryInfo,
            Options = options
        };
}
