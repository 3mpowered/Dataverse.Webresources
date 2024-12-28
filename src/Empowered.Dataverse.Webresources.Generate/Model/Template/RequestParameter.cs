namespace Empowered.Dataverse.Webresources.Generate.Model.Template;

internal record RequestParameter
{
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required bool IsComplexType { get; init; }
    public required bool IsOptional { get; init; }
    public required string EdmType { get; init; }
    public required int StructuralProperty { get; init; }
}
