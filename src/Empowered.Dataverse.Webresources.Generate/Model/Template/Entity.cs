namespace Empowered.Dataverse.Webresources.Generate.Model.Template;

internal record Entity
{
    public required string Name { get; init; }
    public required ICollection<Property> AttributeProperties { get; init; }
    public required ICollection<Property> NavigationProperties { get; init; }
}
