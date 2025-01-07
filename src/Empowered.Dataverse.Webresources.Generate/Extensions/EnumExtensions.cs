using Empowered.Dataverse.Webresources.Generate.Model.Template;

namespace Empowered.Dataverse.Webresources.Generate.Extensions;

public static class EnumExtensions
{
    internal static StructuralProperty ToStructuralProperty(this DataType dataType) => dataType switch
    {
        DataType.Boolean => StructuralProperty.PrimitiveType,
        DataType.DateTime => StructuralProperty.PrimitiveType,
        DataType.Decimal => StructuralProperty.PrimitiveType,
        DataType.Entity => StructuralProperty.EntityType,
        DataType.EntityReference => StructuralProperty.EntityType,
        DataType.EntityCollection => StructuralProperty.Collection,
        DataType.Enumeration => StructuralProperty.EnumerationType,
        DataType.Float => StructuralProperty.PrimitiveType,
        DataType.Guid => StructuralProperty.PrimitiveType,
        DataType.Integer => StructuralProperty.PrimitiveType,
        DataType.Money => StructuralProperty.PrimitiveType,
        DataType.Picklist => StructuralProperty.PrimitiveType,
        DataType.StringArray => StructuralProperty.Collection,
        DataType.String => StructuralProperty.PrimitiveType,
        _ => throw new ArgumentOutOfRangeException(nameof(dataType), dataType, $"Unknown data type {dataType}!")
    };

    internal static string ToEdmTypeName(this DataType dataType) => dataType switch
    {
        DataType.Boolean => "Edm.Boolean",
        DataType.DateTime => "Edm.DateTimeOffset",
        DataType.Decimal => "Edm.Decimal",
        DataType.Float => "Edm.Double",
        DataType.Guid => "Edm.Guid",
        DataType.Integer => "Edm.Int32",
        DataType.Money => "Edm.Decimal",
        DataType.Picklist => "Edm.Int32",
        DataType.StringArray => "Collection(Edm.String)",
        DataType.String => "Edm.String",
        _ => throw new ArgumentOutOfRangeException(nameof(dataType), dataType,
            $"Can't resolve EDM type for data type {dataType} without additional information!")
    };
}
