using System.IO.Abstractions;
using Empowered.Reactive.Extensions.Events;

namespace Empowered.Dataverse.Webresources.Init.Events;

public class DirectoryCreatedEvent : IObservableEvent
{
    public required IDirectoryInfo Directory { get; set; }

    internal static DirectoryCreatedEvent From(IDirectoryInfo directory) => new()
    {
        Directory = directory
    };
}
