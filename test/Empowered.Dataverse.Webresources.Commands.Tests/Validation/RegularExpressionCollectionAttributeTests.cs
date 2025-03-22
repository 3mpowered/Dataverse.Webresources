using Empowered.Dataverse.Webresources.Commands.Validation;
using Shouldly;

namespace Empowered.Dataverse.Webresources.Commands.Tests.Validation;

public class RegularExpressionCollectionAttributeTests
{
    private const string Pattern = @"^\.[^.]+$";

    [Fact]
    public void ShouldBeValidIfPatternMatchesCollection()
    {
        string[] collection = [".js", ".json", ".html"];
        new RegularExpressionCollectionAttribute(Pattern)
            .IsValid(collection)
            .ShouldBeTrue();
    }

    [Fact]
    public void ShouldBeInvalidWhenTypeIsNoCollection()
    {
        new RegularExpressionCollectionAttribute(Pattern)
            .IsValid(1)
            .ShouldBeFalse();
    }

    [Fact]
    public void ShouldBeInvalidWhenPatternDoesntMatchToWholeCollection()
    {
        string[] collection = ["js", ".json", "html"];
        new RegularExpressionCollectionAttribute(Pattern)
            .IsValid(collection)
            .ShouldBeFalse();
    }

    [Fact]
    public void ShouldBeValidForEmptyCollections()
    {
        string[] collection = [];
        new RegularExpressionCollectionAttribute(Pattern)
            .IsValid(collection)
            .ShouldBeTrue();
    }

    [Fact]
    public void ShouldBeValidForNullValues()
    {
        new RegularExpressionCollectionAttribute(Pattern)
            .IsValid(null)
            .ShouldBeTrue();
    }
}
