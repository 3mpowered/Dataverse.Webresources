using Empowered.Reactive.Extensions.Events;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources.Push.Events;

public record PublishedWebresourcesEvent : IObservableEvent
{
    public required ICollection<EntityReference> Webresources { get; init; }

    internal PublishedWebresourcesEvent()
    {
    }

    internal static PublishedWebresourcesEvent From(ICollection<EntityReference> webresources) =>
        new()
        {
            Webresources = webresources
        };
}
