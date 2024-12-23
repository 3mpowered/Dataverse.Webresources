using System.Text.Json.Serialization;
using Empowered.Dataverse.Webresources.Model;

namespace Empowered.Dataverse.Webresources.Push.Model;

public record PushOptions
{
    public required string Solution { get; init; }
    public required string Directory { get; init; }
    [JsonIgnore] public DirectoryInfo DirectoryInfo => new(Directory);
    public bool ForceUpdate { get; init; } = false;
    public string PublisherPrefix { get; set; } = string.Empty;
    public string[] FileExtensions { get; init; } = [];
    internal string FileExtensionsString => string.Join(", ", FileExtensions);
    public bool IncludeSubDirectories { get; init; } = true;
    public string WebresourcePrefix { get; set; } = string.Empty;

    public webresource_webresourcetype DefaultWebresourceType { get; init; } =
        webresource_webresourcetype.Script_JScript;

    public bool AllowManagedUpdates { get; init; } = false;
    public bool Publish { get; init; } = true;
}
