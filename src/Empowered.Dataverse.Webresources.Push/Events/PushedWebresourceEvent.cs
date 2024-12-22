using Empowered.Dataverse.Webresources.Push.Model;
using Empowered.Reactive.Extensions.Events;

namespace Empowered.Dataverse.Webresources.Push.Events;

public record PushedWebresourceEvent : IObservableEvent
{
    public required PushOptions Options { get; init; }
    public required PushResult PushResult { get; init; }

    internal PushedWebresourceEvent()
    {
    }

    internal static PushedWebresourceEvent From(PushResult pushResult, PushOptions options) =>
        new PushedWebresourceEvent
        {
            Options = options,
            PushResult = pushResult
        };

    internal static ICollection<PushedWebresourceEvent> FromRange(IEnumerable<PushResult> pushResults,
        PushOptions options) =>
        pushResults
            .Select(result => From(result, options))
            .ToList();
}
