namespace Empowered.Dataverse.Webresources.Init.Model;

public record InitOptions
{
    public required string Directory { get; init; }
    public DirectoryInfo DirectoryInfo => new(Directory);
    public required string Project { get; init; }
    public required string GlobalNamespace { get; init; }
    public bool Force { get; init; } = false;
    public bool UpgradeDependencies { get; init; } = true;
    public string? Author { get; init; }
    public string? Repository { get; init; }

    public override string ToString() => $"{nameof(Directory)}: {Directory}, {nameof(Project)}: {Project}, {nameof(GlobalNamespace)}: {GlobalNamespace}, {nameof(Force)}: {Force}, {nameof(UpgradeDependencies)}: {UpgradeDependencies}, {nameof(Author)}: {Author}, {nameof(Repository)}: {Repository}";
}
