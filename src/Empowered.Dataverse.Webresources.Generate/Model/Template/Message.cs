namespace Empowered.Dataverse.Webresources.Generate.Model.Template;

internal record Message
{
    public required string Name { get; init; }
    public required BoundParameter BoundParameter { get; init; }
    public string? Entity { get; init; }
    public string? PrimaryIdName { get; init; }
    public OperationType OperationType { get; init; }
    public required ICollection<RequestParameter> RequestParameters { get; init; }
    public required ICollection<Property> ResponseProperties { get; init; }
}
