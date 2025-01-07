using Empowered.Dataverse.Webresources.Generate.Extensions;

namespace Empowered.Dataverse.Webresources.Generate.Model.Template;

internal record RequestParameter
{
    private static readonly DataType[] s_entityDataTypes =
    [
        Template.DataType.Entity,
        Template.DataType.EntityCollection,
        Template.DataType.EntityReference
    ];

    public required string Name { get; init; }
    public required DataType DataType { get; init; }
    public required bool IsOptional { get; init; }
    public string? Entity { get; init; }
    public string? Enumeration { get; init; }
    public ICollection<Option>? Options { get; init; }
    public StructuralProperty StructuralProperty => DataType.ToStructuralProperty();

    public string EdmType
    {
        get
        {
            // TODO: Has enumeration a default EDM type to fall back to?
            if (DataType == DataType.Enumeration)
            {
                return $"Microsoft.Dynamics.CRM.{Enumeration}";
            }

            if (s_entityDataTypes.Contains(DataType))
            {
                return string.IsNullOrWhiteSpace(Entity) ? "mscrm.crmbaseentity" : $"mscrm.{Entity}";
            }

            return DataType.ToEdmTypeName();
        }
    }
}
