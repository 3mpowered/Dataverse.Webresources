using Empowered.Dataverse.Webresources.Init.Model;
using Empowered.Reactive.Extensions.Events;

namespace Empowered.Dataverse.Webresources.Init.Events;

public record InitInvokedEvent : IObservableEvent
{
    public required DirectoryInfo Directory { get; init; }
    public required string Project { get; init; }
    public required string GlobalNamespace { get; init; }

    internal static InitInvokedEvent From(InitOptions options) => new()
    {
        Directory = new DirectoryInfo(options.Directory),
        Project = options.Project,
        GlobalNamespace = options.GlobalNamespace
    };
}
