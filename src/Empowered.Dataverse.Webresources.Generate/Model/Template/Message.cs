using Empowered.Dataverse.Webresources.Generate.Extensions;

namespace Empowered.Dataverse.Webresources.Generate.Model.Template;

internal record Message(
)
{
    public required string Name { get; init; }
    public required BoundParameter BoundParameter { get; init; }
    public OperationType OperationType { get; init; }
    public required ICollection<RequestParameter> RequestParameters { get; init; }
    public required ICollection<Property> ResponseProperties { get; init; }
}

internal enum OperationType
{
    Action = 0,
    Function = 1,
    CRUD = 2
}

internal enum BoundParameter
{
    Global,
    Entity,
    EntityCollection
}
