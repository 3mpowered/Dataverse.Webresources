using Empowered.Dataverse.Webresources.Model;
using Empowered.Reactive.Extensions.Events;

namespace Empowered.Dataverse.Webresources.Push.Events;

public record RetrievedPublisherEvent : IObservableEvent
{
    public required Guid Id { get; init; }
    public required string UniqueName { get; init; }
    public required string FriendlyName { get; init; }
    public required string CustomizationPrefix { get; init; }
    public required PublisherSource Source { get; init; }

    internal RetrievedPublisherEvent()
    {
    }

    internal static RetrievedPublisherEvent From(Publisher publisher, bool retrievedFromConfig)
    {
        return new RetrievedPublisherEvent
        {
            Id = publisher.Id,
            UniqueName = publisher.UniqueName,
            FriendlyName = publisher.FriendlyName,
            CustomizationPrefix = publisher.CustomizationPrefix,
            Source = retrievedFromConfig
                ? PublisherSource.Configuration
                : PublisherSource.Solution
        };
    }

    public enum PublisherSource
    {
        Configuration,
        Solution
    }
}
