using Empowered.Dataverse.Webresources.Push.Extensions;
using Empowered.Dataverse.Webresources.Push.Model;
using FluentAssertions;

namespace Empowered.Dataverse.Webresources.Push.Tests.Extensions;

public class PushStateExtensionsTests
{
    [Theory]
    [InlineData(PushState.Uptodate, "Skipped")]
    [InlineData(PushState.Updated, "Updated")]
    [InlineData(PushState.Created, "Created")]
    public void ShouldFormatPushStateCorrectly(PushState pushState, string expectedFormat)
    {
        pushState.Format().Should().Be(expectedFormat);
    }

    [Fact]
    public void ShouldThrowOnInvalidPushStateForFormat()
    {
        const PushState pushState = (PushState)999;
        Action actor = () => { pushState.Format(); };

        actor.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName(nameof(pushState))
            .And.ActualValue.Should().Be(pushState);
    }
}
