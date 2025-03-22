using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Events;
using Empowered.Dataverse.Webresources.Push.Model;
using Microsoft.Xrm.Sdk;
using Shouldly;

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

        publishedWebresourcesEvent.ShouldNotBeNull();
        publishedWebresourcesEvent.Webresources.ShouldNotBeNull().ShouldNotBeEmpty();
        publishedWebresourcesEvent.Webresources.Count.ShouldBe(webresources.Count);
        publishedWebresourcesEvent.Webresources.ShouldBeEquivalentTo(webresources);
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

        pushInvokedEvent.ShouldNotBeNull();
        pushInvokedEvent.Options.ShouldBeEquivalentTo(pushOptions);
        pushInvokedEvent.Directory.FullName.ShouldBe(pushOptions.Directory);
        pushInvokedEvent.SolutionName.ShouldBe(pushOptions.Solution);
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

        retrievedPublisherEvent.ShouldNotBeNull();

        retrievedPublisherEvent.Id.ShouldBe(publisher.Id);
        retrievedPublisherEvent.UniqueName.ShouldBe(publisher.UniqueName);
        retrievedPublisherEvent.FriendlyName.ShouldBe(publisher.FriendlyName);
        retrievedPublisherEvent.CustomizationPrefix.ShouldBe(publisher.CustomizationPrefix);
        retrievedPublisherEvent.Source.ShouldBe(expectedSource);
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

        pushedWebresourceEvent.ShouldNotBeNull();
        pushedWebresourceEvent.PushResult.ShouldBeEquivalentTo(pushResult);
        pushedWebresourceEvent.Options.ShouldBeEquivalentTo(pushOptions);
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

        retrievedFileEvent.ShouldNotBeNull();
        retrievedFileEvent.File.ShouldBeEquivalentTo(webresourceFile);
        retrievedFileEvent.Options.ShouldBeEquivalentTo(pushOptions);
        retrievedFileEvent.Directory.FullName.ShouldBe(pushOptions.Directory);
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

        retrievedSolutionEvent.ShouldNotBeNull();
        retrievedSolutionEvent.Id.ShouldBe(solution.Id);
        retrievedSolutionEvent.UniqueName.ShouldBe(solution.UniqueName);
        retrievedSolutionEvent.FriendlyName.ShouldBe(solution.FriendlyName);
        retrievedSolutionEvent.Version.ShouldBe(solution.Version);
        retrievedSolutionEvent.PublisherId.ShouldBeEquivalentTo(solution.PublisherId);
        retrievedSolutionEvent.PublisherId.Id.ShouldBe(solution.PublisherId.Id);
    }
}
