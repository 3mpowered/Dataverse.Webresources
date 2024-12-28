namespace Empowered.Dataverse.Webresources.Generate.Model.Template;

internal record Choice
{
    public required string Name { get; init; }
    public required ICollection<Option> Options { get; init; } = [];
}
