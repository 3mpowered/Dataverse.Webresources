using Empowered.Dataverse.Webresources.Commands.Validation;
using FluentAssertions;
using Xunit;

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
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ShouldBeInvalidWhenTypeIsNoCollection()
    {
        new RegularExpressionCollectionAttribute(Pattern)
            .IsValid(1)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ShouldBeInvalidWhenPatternDoesntMatchToWholeCollection()
    {
        string[] collection = ["js", ".json", "html"];
        new RegularExpressionCollectionAttribute(Pattern)
            .IsValid(collection)
            .Should()
            .BeFalse();
    }
}
