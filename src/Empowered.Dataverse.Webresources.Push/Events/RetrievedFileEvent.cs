using Empowered.Dataverse.Webresources.Push.Model;
using Empowered.Reactive.Extensions.Events;

namespace Empowered.Dataverse.Webresources.Push.Events;

public record RetrievedFileEvent : IObservableEvent
{
    public required DirectoryInfo Directory { get; init; }
    public required PushOptions Options { get; init; }
    public required WebresourceFile File { get; init; }

    internal RetrievedFileEvent()
    {
    }

    internal static RetrievedFileEvent From(PushOptions options, WebresourceFile file) =>
        new()
        {
            Directory = options.DirectoryInfo,
            Options = options,
            File = file
        };
    internal static ICollection<RetrievedFileEvent> FromRange(PushOptions options, ICollection<WebresourceFile> files) =>
    files.Select(file => From(options, file)).ToList();
}
