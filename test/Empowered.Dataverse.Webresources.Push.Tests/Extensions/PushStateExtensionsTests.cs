using Empowered.Dataverse.Webresources.Push.Extensions;
using Empowered.Dataverse.Webresources.Push.Model;
using Shouldly;

namespace Empowered.Dataverse.Webresources.Push.Tests.Extensions;

public class PushStateExtensionsTests
{
    [Theory]
    [InlineData(PushState.Uptodate, "Skipped")]
    [InlineData(PushState.Updated, "Updated")]
    [InlineData(PushState.Created, "Created")]
    public void ShouldFormatPushStateCorrectly(PushState pushState, string expectedFormat)
    {
        pushState.Format().ShouldBe(expectedFormat);
    }

    [Fact]
    public void ShouldThrowOnInvalidPushStateForFormat()
    {
        const PushState pushState = (PushState)999;
        var exception = Should.Throw<ArgumentOutOfRangeException>(() => { pushState.Format(); });

        exception.ParamName.ShouldBe(nameof(pushState));
        exception.ActualValue.ShouldBe(pushState);
    }
}
