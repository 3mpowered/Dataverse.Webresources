using Empowered.Dataverse.Webresources.Generate.Model.Template;

namespace Empowered.Dataverse.Webresources.Generate.Extensions;

public static class EnumExtensions
{
    internal static StructuralProperty ToStructuralProperty(this ParameterDataType parameterDataType) => parameterDataType switch
    {
        ParameterDataType.Boolean => StructuralProperty.PrimitiveType,
        ParameterDataType.DateTime => StructuralProperty.PrimitiveType,
        ParameterDataType.Decimal => StructuralProperty.PrimitiveType,
        ParameterDataType.Entity => StructuralProperty.EntityType,
        ParameterDataType.EntityReference => StructuralProperty.EntityType,
        ParameterDataType.EntityCollection => StructuralProperty.Collection,
        ParameterDataType.Enumeration => StructuralProperty.EnumerationType,
        ParameterDataType.Float => StructuralProperty.PrimitiveType,
        ParameterDataType.Guid => StructuralProperty.PrimitiveType,
        ParameterDataType.Integer => StructuralProperty.PrimitiveType,
        ParameterDataType.Money => StructuralProperty.PrimitiveType,
        ParameterDataType.Picklist => StructuralProperty.PrimitiveType,
        ParameterDataType.StringArray => StructuralProperty.Collection,
        ParameterDataType.String => StructuralProperty.PrimitiveType,
        _ => throw new ArgumentOutOfRangeException(nameof(parameterDataType), parameterDataType, $"Unknown data type {parameterDataType}!")
    };

    internal static string ToEdmTypeName(this ParameterDataType parameterDataType) => parameterDataType switch
    {
        ParameterDataType.Boolean => "Edm.Boolean",
        ParameterDataType.DateTime => "Edm.DateTimeOffset",
        ParameterDataType.Decimal => "Edm.Decimal",
        ParameterDataType.Float => "Edm.Double",
        ParameterDataType.Guid => "Edm.Guid",
        ParameterDataType.Integer => "Edm.Int32",
        ParameterDataType.Money => "Edm.Decimal",
        ParameterDataType.Picklist => "Edm.Int32",
        ParameterDataType.StringArray => "Collection(Edm.String)",
        ParameterDataType.String => "Edm.String",
        _ => throw new ArgumentOutOfRangeException(nameof(parameterDataType), parameterDataType,
            $"Can't resolve EDM type for data type {parameterDataType} without additional information!")
    };
}
