using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Events;
using Empowered.Dataverse.Webresources.Push.Model;
using FluentAssertions;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources.Push.Tests.Events;

public class EventInitializationTests
{
    [Fact]
    public void CanInitializePublishedWebresourceEventFromEntityReferenceCollection()
    {
        ICollection<EntityReference> webresources =
        [
            new(WebResource.EntityLogicalName, Guid.NewGuid()),
            new(WebResource.EntityLogicalName, Guid.NewGuid()),
            new(WebResource.EntityLogicalName, Guid.NewGuid()),
            new(WebResource.EntityLogicalName, Guid.NewGuid()),
            new(WebResource.EntityLogicalName, Guid.NewGuid()),
        ];

        var publishedWebresourcesEvent = PublishedWebresourcesEvent.From(webresources);

        publishedWebresourcesEvent.Should().NotBeNull();
        publishedWebresourcesEvent.Webresources.Should().NotBeNullOrEmpty();
        publishedWebresourcesEvent.Webresources.Should().HaveCount(webresources.Count);
        publishedWebresourcesEvent.Webresources.Should().BeEquivalentTo(webresources);
    }

    [Fact]
    public void CanInitializePushInvokedEventFromPushOptions()
    {
        var pushOptions = new PushOptions
        {
            Solution = "solution",
            Directory = Path.GetTempPath(),
        };

        var pushInvokedEvent = PushInvokedEvent.From(pushOptions);

        pushInvokedEvent.Should().NotBeNull();
        pushInvokedEvent.Options.Should().BeEquivalentTo(pushOptions);
        pushInvokedEvent.Directory.FullName.Should().Be(pushOptions.Directory);
        pushInvokedEvent.SolutionName.Should().Be(pushOptions.Solution);
    }

    [Theory]
    [InlineData(RetrievedPublisherEvent.PublisherSource.Solution, false)]
    [InlineData(RetrievedPublisherEvent.PublisherSource.Configuration, true)]
    public void CanInitializeRetrievePublisherEventFromPublisherRetrievedFromConfig(
        RetrievedPublisherEvent.PublisherSource expectedSource, bool retrievedFromConfiguration)
    {
        var publisher = new Publisher
        {
            Id = Guid.NewGuid(),
            UniqueName = "publisher",
            FriendlyName = "Publisher",
            CustomizationPrefix = "pub",
        };

        var retrievedPublisherEvent = RetrievedPublisherEvent.From(publisher, retrievedFromConfiguration);

        retrievedPublisherEvent.Should().NotBeNull();

        retrievedPublisherEvent.Id.Should().Be(publisher.Id);
        retrievedPublisherEvent.UniqueName.Should().Be(publisher.UniqueName);
        retrievedPublisherEvent.FriendlyName.Should().Be(publisher.FriendlyName);
        retrievedPublisherEvent.CustomizationPrefix.Should().Be(publisher.CustomizationPrefix);
        retrievedPublisherEvent.Source.Should().Be(expectedSource);
    }

    [Fact]
    public void CanInitializePushedWebresourceEventFromPushResultAndPushOptions()
    {
        var pushOptions = new PushOptions
        {
            Solution = "solution",
            Directory = Path.GetTempPath()
        };
        var webresourceFile = new WebresourceFile(
            "account.form.js",
            Path.Combine(Path.GetTempPath(), "account.form.js"),
            ".js",
            "pub_account.form.js",
            string.Empty
        );
        var webresourceReference = new EntityReference(WebResource.EntityLogicalName, Guid.NewGuid());
        var pushResult = new PushResult(webresourceReference, webresourceFile, PushState.Created);

        var pushedWebresourceEvent = PushedWebresourceEvent
            .FromRange([pushResult], pushOptions)
            .Single();

        pushedWebresourceEvent.Should().NotBeNull();
        pushedWebresourceEvent.PushResult.Should().BeEquivalentTo(pushResult);
        pushedWebresourceEvent.Options.Should().BeEquivalentTo(pushOptions);
    }

    [Fact]
    public void CanInitializeRetrievedFileEventFromPushOptionsAndWebresourceFile()
    {
        var pushOptions = new PushOptions
        {
            Solution = "solution",
            Directory = Path.GetTempPath()
        };
        var webresourceFile = new WebresourceFile(
            "account.form.js",
            Path.Combine(Path.GetTempPath(), "account.form.js"),
            ".js",
            "pub_account.form.js",
            string.Empty
        );

        var retrievedFileEvent = RetrievedFileEvent
            .FromRange(pushOptions, [webresourceFile])
            .Single();

        retrievedFileEvent.Should().NotBeNull();
        retrievedFileEvent.File.Should().BeEquivalentTo(webresourceFile);
        retrievedFileEvent.Options.Should().BeEquivalentTo(pushOptions);
        retrievedFileEvent.Directory.FullName.Should().Be(pushOptions.Directory);
    }

    [Fact]
    public void CanInitializeRetrievedSolutionEventFromSolution()
    {
        var solution = new Solution
        {
            Id = Guid.NewGuid(),
            UniqueName = "solution",
            FriendlyName = "Solution",
            Version = "1.0.0.0",
            PublisherId = new EntityReference(Publisher.EntityLogicalName, Guid.NewGuid())
        };

        var retrievedSolutionEvent = RetrievedSolutionEvent.From(solution);

        retrievedSolutionEvent.Should().NotBeNull();
        retrievedSolutionEvent.Id.Should().Be(solution.Id);
        retrievedSolutionEvent.UniqueName.Should().Be(solution.UniqueName);
        retrievedSolutionEvent.FriendlyName.Should().Be(solution.FriendlyName);
        retrievedSolutionEvent.Version.Should().Be(solution.Version);
        retrievedSolutionEvent.PublisherId.Should().BeEquivalentTo(solution.PublisherId);
        retrievedSolutionEvent.PublisherId.Id.Should().Be(solution.PublisherId.Id);
    }
}
