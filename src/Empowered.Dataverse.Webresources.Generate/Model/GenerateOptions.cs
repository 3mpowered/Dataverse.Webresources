using System.Text.Json.Serialization;

namespace Empowered.Dataverse.Webresources.Generate.Model;

public record GenerateOptions
{
    public required string Directory { get; init; }
    [JsonIgnore]
    public DirectoryInfo DirectoryInfo => new(Directory);
    public ICollection<string> Entities { get; init; } = new List<string>();
    public ICollection<string> Actions { get; init; } = new List<string>();
    public ICollection<Guid> Forms { get; init; } = new List<Guid>();
}
