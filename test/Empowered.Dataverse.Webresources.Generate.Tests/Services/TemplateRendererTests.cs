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
            Properties =
            [
                new Property
                {
                    Name = "accountid",
                    Type = "string",
                    CanBeNullable = false,
                    IsOptional = false
                },
                new Property
                {
                    Name = "name",
                    Type = "string",
                    CanBeNullable = true,
                    IsOptional = true
                },
                new Property
                {
                    Name = "empwrd_revenue",
                    Type = "number",
                    CanBeNullable = false,
                    IsOptional = true
                },
                new Property
                {
                    Name = "empwrd_is_vip",
                    Type = "boolean",
                    CanBeNullable = true,
                    IsOptional = false
                }
            ]
        };

        var result = _templateRenderer.Render("entity.liquid", entity);
        _testOutputHelper.WriteLine(result);

        var propertyString = string.Join(s_separator,
            entity.Properties.Select(property =>
                $"{property.Name}{(property.IsOptional ? "?" : string.Empty)}: {property.Type}{(property.CanBeNullable ? " | null" : string.Empty)};"));
        var expected = $$"""
                         export interface {{entity.Name}} {
                             {{propertyString}}
                         }

                         """;
        result.Should().Be(expected);
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
        new Message
        {
            Name = "empwrd_do_something",
            IsBound = true,
            OperationType = 0,
            RequestParameters = [
                new RequestParameter
                {
                    Name = "entity",
                    Type = "string"
                }
            ]
        }
    }
}
