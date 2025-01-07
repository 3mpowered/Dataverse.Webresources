namespace Empowered.Dataverse.Webresources.Generate.Model.Template;

internal record Property
{
    private static readonly DataType[] s_entityDataTypes =
    [
        DataType.Entity,
        DataType.EntityCollection,
        DataType.EntityReference
    ];

    // TODO: Check if there are additional types or subtypes.
    private static readonly DataType[] s_formattableDataTypes =
    [
        DataType.Boolean,
        DataType.Decimal,
        DataType.Float,
        DataType.Enumeration,
        DataType.Money,
        DataType.DateTime,
        DataType.EntityReference
    ];

    public required DataType DataType { get; init; }
    public required string Name { get; init; }
    public required bool IsNullable { get; init; }
    public required bool IsOptional { get; init; }
    public string? Entity { get; init; }
    public string? PrimaryIdName { get; init; }
    public bool IsFormattable => s_formattableDataTypes.Contains(DataType);
    public bool IsEntityType => s_entityDataTypes.Contains(DataType);
}
