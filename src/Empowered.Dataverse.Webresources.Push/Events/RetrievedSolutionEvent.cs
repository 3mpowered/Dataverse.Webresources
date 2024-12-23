using Empowered.Dataverse.Webresources.Model;
using Empowered.Reactive.Extensions.Events;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources.Push.Events;

public record RetrievedSolutionEvent : IObservableEvent
{
    public required Guid Id { get; init; }
    public required string UniqueName { get; init; }
    public required string FriendlyName { get; init; }
    public required string Version { get; init; }
    public required EntityReference PublisherId { get; init; }

    internal RetrievedSolutionEvent()
    {
    }

    internal static RetrievedSolutionEvent From(Solution solution)
    {
        return new RetrievedSolutionEvent
        {
            Id = solution.Id,
            UniqueName = solution.UniqueName,
            FriendlyName = solution.FriendlyName,
            Version = solution.Version,
            PublisherId = solution.PublisherId
        };
    }
}
