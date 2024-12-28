namespace Empowered.Dataverse.Webresources.Generate.Model.Template;

internal record Message
{
    public required string Name { get; init; }
    public required bool IsBound { get; init; }
    public required int OperationType { get; init; }
    public required ICollection<RequestParameter> RequestParameters { get; init; }
    public required ICollection<Property> ResponseProperties { get; init; }
}
