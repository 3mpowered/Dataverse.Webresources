using System.IO.Abstractions;
using Empowered.Reactive.Extensions.Events;

namespace Empowered.Dataverse.Webresources.Init.Events;

public record FileCreatedEvent : IObservableEvent
{
    public required IFileInfo File { get; init; }

    internal static FileCreatedEvent From(IFileInfo file) => new()
    {
        File = file,
    };
}
