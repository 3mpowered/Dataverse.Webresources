using Empowered.Dataverse.Webresources.Generate.Model.Template;
using Empowered.Dataverse.Webresources.Generate.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit.Abstractions;

namespace Empowered.Dataverse.Webresources.Generate.Tests.Services;

public class TemplateRendererTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private static readonly string? s_separator = $"{Environment.NewLine}    ";
    private readonly TemplateRenderer _templateRenderer = new(NullLogger<TemplateRenderer>.Instance);

    public TemplateRendererTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void ShouldRenderEnumTemplate()
    {
        var choice = new Choice
        {
            Name = "caseorigin",
            Options =
            [
                new Option
                {
                    Name = "Chat",
                    Value = 10000
                },
                new Option
                {
                    Name = "Email",
                    Value = 10001
                },
                new Option
                {
                    Name = "Phone",
                    Value = 10002
                }
            ]
        };
        var result = _templateRenderer.Render("choice.liquid", choice);
        _testOutputHelper.WriteLine(result);

        var optionString = string.Join(s_separator,
            choice.Options.Select(option => $"{option.Name} = {option.Value},"));
        var expected = $$"""
                         export enum {{choice.Name}} {
                             {{optionString}}
                         }

                         """;
        result.Should().Be(expected);
    }

    [Fact]
    public void ShouldRenderEntityTemplate()
    {
        var entity = new Entity
        {
            Name = "account",
            AttributeProperties =
            [
                new Property
                {
                    Name = "accountid",
                    DataType = DataType.Guid,
                    IsNullable = false,
                    IsOptional = false
                },
                new Property
                {
                    Name = "name",
                    DataType = DataType.String,
                    IsNullable = true,
                    IsOptional = true
                },
                new Property
                {
                    Name = "_primarycontactid_value",
                    DataType = DataType.EntityReference,
                    IsOptional = true,
                    IsNullable = true,
                    Entity = "contact",
                    PrimaryIdName = "contactid"
                },
                new Property
                {
                    Name = "empwrd_revenue",
                    DataType = DataType.Decimal,
                    IsNullable = false,
                    IsOptional = true
                },
                new Property
                {
                    Name = "empwrd_is_vip",
                    DataType = DataType.Boolean,
                    IsNullable = true,
                    IsOptional = false
                }
            ],
            NavigationProperties =
            [
                new Property
                {
                    Name = "primarycontactid",
                    DataType = DataType.Entity,
                    IsOptional = true,
                    IsNullable = true,
                    Entity = "contact",
                    PrimaryIdName = "contactid"
                }
            ]
        };

        var result = _templateRenderer.Render("entity.liquid", entity);
        _testOutputHelper.WriteLine(result);

//         var propertyString = string.Join(s_separator,
//             entity.Properties.Select(property =>
//                 $"{property.Name}{(property.IsOptional ? "?" : string.Empty)}: {property.DataType}{(property.CanBeNullable ? " | null" : string.Empty)};"));
//         var expected = $$"""
//                          export interface {{entity.Name}} {
//                              {{propertyString}}
//                          }
//
//                          """;
//         result.Should().Be(expected);
    }

    [Fact]
    public void ShouldRenderLogicalNamesTemplate()
    {
        var logicalNames = new LogicalNames
        {
            Entities =
            [
                "account",
                "contact",
                "incident"
            ]
        };

        var result = _templateRenderer.Render("logicalNames.liquid", logicalNames);
        _testOutputHelper.WriteLine(result);

        var entitiesString = string.Join(s_separator,
            logicalNames.Entities.Select(entity => $"public static readonly {entity} = \"{entity}\";"));
        var expected = $$"""
                         export abstract class LogicalNames {
                             {{entitiesString}}
                         }

                         """;
        result.Should().Be(expected);
    }

    [Fact]
    public void ShouldRenderMessageNamesTemplate()
    {
        var messages = new MessageNames
        {
            Messages =
            [
                "WhoAmI",
                "AddToQueue",
                "msdyn_something"
            ]
        };

        var result = _templateRenderer.Render("messageNames.liquid", messages);
        _testOutputHelper.WriteLine(result);
        var messagesString = string.Join(s_separator,
            messages.Messages.Select(message => $"public static readonly {message} = \"{message}\";"));
        var expected = $$"""
                         export abstract class Messages {
                             {{messagesString}}
                         }

                         """;
        result.Should().Be(expected);
    }

    [Fact]
    public void ShouldRenderMessageTemplate()
    {
        var message = new Message
        {
            Name = "empwrd_test",
            BoundParameter = BoundParameter.Entity,
            OperationType = OperationType.Action,
            Entity = "account",
            PrimaryIdName = "accountid",
            RequestParameters =
            [
                new RequestParameter
                {
                    Name = "BooleanParameter",
                    DataType = DataType.Boolean,
                    IsOptional = true,
                },
                new RequestParameter
                {
                    Name = "DateTimeParameter",
                    DataType = DataType.DateTime,
                    IsOptional = true,
                },
                new RequestParameter
                {
                    Name = "DecimalParameter",
                    DataType = DataType.Decimal,
                    IsOptional = true,
                },
                new RequestParameter
                {
                    Name = "BaseEntityParameter",
                    DataType = DataType.Entity,
                    IsOptional = true,
                },
                new RequestParameter
                {
                    Name = "EntityCollectionParameter",
                    DataType = DataType.EntityCollection,
                    IsOptional = true,
                    Entity = "solution"
                },
                new RequestParameter
                {
                    Name = "EntityReferenceParameter",
                    DataType = DataType.EntityReference,
                    IsOptional = true
                },
                new RequestParameter
                {
                    Name = "FloatParameter",
                    DataType = DataType.Float,
                    IsOptional = true,
                },
                new RequestParameter
                {
                    Name = "GuidParameter",
                    DataType = DataType.Guid,
                    IsOptional = true
                },
                new RequestParameter
                {
                    Name = "IntegerParameter",
                    DataType = DataType.Integer,
                    IsOptional = true
                },
                new RequestParameter
                {
                    Name = "MoneyParameter",
                    DataType = DataType.Money,
                    IsOptional = true,
                },
                new RequestParameter
                {
                    Name = "PicklistParameter",
                    DataType = DataType.Picklist,
                    IsOptional = true
                },
                new RequestParameter
                {
                    Name = "StringArrayParameter",
                    DataType = DataType.StringArray,
                    IsOptional = true,
                },
                new RequestParameter
                {
                    Name = "StringParameter",
                    DataType = DataType.String,
                    IsOptional = true
                },
                new RequestParameter
                {
                    Name = "EnumParameter",
                    DataType = DataType.Enumeration,
                    IsOptional = true,
                    Enumeration = "OptionType",
                    Options =
                    [
                        new Option
                        {
                            Name = "Option A",
                            Value = 1000,
                        },
                        new Option
                        {
                            Name = "Option B",
                            Value = 1001
                        }
                    ]
                }
            ],
            ResponseProperties =
            [
                new Property
                {
                    Name = "BooleanProperty",
                    DataType = DataType.Boolean,
                    IsOptional = false,
                    IsNullable = true,
                },
                new Property
                {
                    Name = "DateTimeProperty",
                    DataType = DataType.DateTime,
                    IsOptional = false,
                    IsNullable = true
                },
                new Property
                {
                    Name = "DecimalProperty",
                    DataType = DataType.Decimal,
                    IsOptional = false,
                    IsNullable = true
                },
                new Property
                {
                    Name = "BaseEntityProperty",
                    DataType = DataType.Entity,
                    IsOptional = false,
                    IsNullable = true,
                },
                new Property
                {
                    Name = "AccountEntityProperty",
                    DataType = DataType.Entity,
                    IsOptional = false,
                    IsNullable = true,
                    Entity = "account",
                    PrimaryIdName = "accountid"
                },
                new Property
                {
                    Name = "EntityCollectionProperty",
                    DataType = DataType.EntityCollection,
                    IsOptional = false,
                    IsNullable = true,
                    Entity = "contact",
                    PrimaryIdName = "contactid"
                },
                new Property
                {
                    Name = "EntityReferenceProperty",
                    DataType = DataType.EntityReference,
                    IsOptional = false,
                    IsNullable = true,
                    Entity = "solution",
                    PrimaryIdName = "solutionid"
                },
                new Property
                {
                    Name = "FloatProperty",
                    DataType = DataType.Float,
                    IsOptional = false,
                    IsNullable = true
                },
                new Property
                {
                    Name = "GuidProperty",
                    DataType = DataType.Guid,
                    IsOptional = false,
                    IsNullable = true
                },
                new Property
                {
                    Name = "IntegerProperty",
                    DataType = DataType.Integer,
                    IsOptional = false,
                    IsNullable = true
                },
                new Property
                {
                    Name = "MoneyProperty",
                    DataType = DataType.Money,
                    IsOptional = false,
                    IsNullable = true
                },
                new Property
                {
                    Name = "PicklistProperty",
                    DataType = DataType.Picklist,
                    IsOptional = false,
                    IsNullable = true
                },
                new Property
                {
                    Name = "StringArrayProperty",
                    DataType = DataType.StringArray,
                    IsOptional = false,
                    IsNullable = true
                },
                new Property
                {
                    Name = "StringParameter",
                    DataType = DataType.String,
                    IsOptional = false,
                    IsNullable = true
                },
                new Property
                {
                    Name = "EnumParameter",
                    DataType = DataType.Enumeration,
                    IsOptional = false,
                    IsNullable = true
                }
            ]
        };

        var result = _templateRenderer.Render("message.liquid", message);
        _testOutputHelper.WriteLine(result);
    }
}
