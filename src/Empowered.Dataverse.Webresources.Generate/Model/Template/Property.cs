namespace Empowered.Dataverse.Webresources.Generate.Model.Template;

internal record Property
{
    public required string Type { get; init; }
    public required string Name { get; init; }
    public required bool CanBeNullable { get; init; }
    public required bool IsOptional { get; init; }
}
