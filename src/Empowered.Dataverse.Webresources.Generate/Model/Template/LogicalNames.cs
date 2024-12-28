namespace Empowered.Dataverse.Webresources.Generate.Model.Template;

internal record LogicalNames
{
    public required ICollection<string> Entities { get; init; }
}
