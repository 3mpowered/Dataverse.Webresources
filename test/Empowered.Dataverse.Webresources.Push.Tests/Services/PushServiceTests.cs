using System.IO.Abstractions.TestingHelpers;
using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Events;
using Empowered.Dataverse.Webresources.Push.Model;
using Empowered.Dataverse.Webresources.Push.Services;
using Empowered.Reactive.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;

namespace Empowered.Dataverse.Webresources.Push.Tests.Services;

public class PushServiceTests
{
    private static readonly string s_temporaryPath = Path.GetTempPath();
    private readonly IDataverseService _dataverseService = Substitute.For<IDataverseService>();
    private readonly IEventObservable _observable = Substitute.For<IEventObservable>();
    private readonly MockFileSystem _fileSystem = new();
    private readonly PushService _pushService;

    public PushServiceTests()
    {
        IFileService fileService = new FileService(NullLogger<FileService>.Instance, _fileSystem);
        _pushService = new PushService(_dataverseService, fileService, NullLogger<PushService>.Instance, _observable);
    }

    [Fact]
    public void ShouldGetPublisherPrefixFromSolution()
    {
        var basePath = Path.Combine(s_temporaryPath, "dist");
        const string webresourcePrefix = "scripts";

        var options = new PushOptions
        {
            Directory = basePath,
            Solution = "customizations",
            PublisherPrefix = string.Empty,
            WebresourcePrefix = webresourcePrefix,
            FileExtensions = ["js"],
            IncludeSubDirectories = true,
            ForceUpdate = false
        };
        const string formFileName = "account.form.js";
        var formPath = Path.Combine(basePath, formFileName);
        _fileSystem.AddFile(formPath, new MockFileData("function onLoad(context) { }"));

        var publisher = new Publisher
        {
            Id = Guid.NewGuid(),
            CustomizationPrefix = "pub"
        };
        var solution = new Solution
        {
            Id = Guid.NewGuid(),
            UniqueName = "customizations",
            PublisherId = publisher.ToEntityReference()
        };
        var pushResult = new PushResult(
            new EntityReference(WebResource.EntityLogicalName,
                Guid.NewGuid()),
            new WebresourceFile(
                formFileName,
                formPath,
                ".js",
                "not relevant",
                string.Empty
            ),
            PushState.Created
        );

        _dataverseService
            .GetSolution(Arg.Any<string>())
            .ReturnsForAnyArgs(solution);
        _dataverseService
            .GetPublisher(Arg.Any<EntityReference>())
            .ReturnsForAnyArgs(publisher);
        _dataverseService
            .UpsertWebresource(Arg.Any<WebresourceFile>(), Arg.Any<PushOptions>())
            .Returns(pushResult);
        _dataverseService
            .AddToSolution(Arg.Any<EntityReference>(), Arg.Any<EntityReference>())
            .ReturnsForAnyArgs(_ => new AddToSolutionResult(
                new EntityReference(WebResource.EntityLogicalName, Guid.NewGuid()),
                new EntityReference(SolutionComponent.EntityLogicalName, Guid.NewGuid()),
                AddToSolutionState.Existing
            ));


        var results = _pushService.PushWebresources(options);

        results.ShouldNotBeNull().ShouldNotBeEmpty();
        results.Count.ShouldBe(1);
        results.ShouldContain(result =>
            result.PushState == pushResult.PushState &&
            result.WebresourceReference.Id == pushResult.WebresourceReference.Id);
        _dataverseService
            .Received(1)
            .GetPublisher(Arg.Any<EntityReference>());
        _observable
            .Received(1)
            .Publish(Arg.Any<RetrievedPublisherEvent>());
    }

    [Fact]
    public void ShouldGetPublisherPrefixFromOptions()
    {
        var basePath = Path.Combine(s_temporaryPath, "dist");
        const string webresourcePrefix = "scripts";

        var options = new PushOptions
        {
            Directory = basePath,
            Solution = "customizations",
            PublisherPrefix = "cust_",
            WebresourcePrefix = webresourcePrefix,
            FileExtensions = ["js"],
            IncludeSubDirectories = true,
            ForceUpdate = false,
        };
        const string formFileName = "account.form.js";
        var formPath = Path.Combine(basePath, formFileName);
        const string formContent = "function onLoad(context) { }";
        _fileSystem.AddFile(formPath, new MockFileData(formContent));

        var publisher = new Publisher
        {
            Id = Guid.NewGuid(),
            CustomizationPrefix = "pub"
        };
        var solution = new Solution
        {
            Id = Guid.NewGuid(),
            UniqueName = "customizations",
            PublisherId = publisher.ToEntityReference()
        };
        var pushResult = new PushResult(
            new EntityReference(WebResource.EntityLogicalName, Guid.NewGuid()),
            new WebresourceFile(
                formFileName,
                formPath,
                ".js",
                "not relevant",
                formContent
            ),
            PushState.Created
        );

        _dataverseService
            .GetSolution(Arg.Any<string>())
            .ReturnsForAnyArgs(solution);
        _dataverseService
            .UpsertWebresource(Arg.Any<WebresourceFile>(), Arg.Any<PushOptions>())
            .ReturnsForAnyArgs(pushResult);
        _dataverseService.GetPublisher(options.PublisherPrefix)
            .Returns(publisher);
        _dataverseService
            .AddToSolution(Arg.Any<EntityReference>(), Arg.Any<EntityReference>())
            .ReturnsForAnyArgs(_ => new AddToSolutionResult(
                new EntityReference(WebResource.EntityLogicalName, Guid.NewGuid()),
                new EntityReference(SolutionComponent.EntityLogicalName, Guid.NewGuid()),
                AddToSolutionState.Existing
            ));

        var results = _pushService.PushWebresources(options);

        results.ShouldNotBeNull().ShouldNotBeEmpty();
        results.Count.ShouldBe(1);
        results.ShouldContain(result =>
            result.PushState == pushResult.PushState &&
            result.WebresourceReference.Id == pushResult.WebresourceReference.Id);
        _dataverseService
            .DidNotReceive()
            .GetPublisher(Arg.Any<EntityReference>());
        _observable
            .Received(1)
            .Publish(Arg.Any<RetrievedPublisherEvent>());
    }

    [Fact]
    public void ShouldEmitObservableEvents()
    {
        var basePath = Path.Combine(s_temporaryPath, "dist");

        const string formFileName = "account.form.js";
        var formPath = Path.Combine(basePath, formFileName);
        const string formContent = "function onLoad(context) { }";
        _fileSystem.AddFile(formPath, new MockFileData(formContent));

        var publisher = new Publisher
        {
            Id = Guid.NewGuid(),
            CustomizationPrefix = "pub"
        };
        var solution = new Solution
        {
            Id = Guid.NewGuid(),
            UniqueName = "customizations",
            PublisherId = publisher.ToEntityReference()
        };
        var pushResult = new PushResult(
            new EntityReference(WebResource.EntityLogicalName, Guid.NewGuid()),
            new WebresourceFile(
                formFileName,
                formPath,
                ".js",
                "not relevant",
                formContent
            ),
            PushState.Created
        );

        var options = new PushOptions
        {
            Directory = basePath,
            Solution = solution.UniqueName,
            Publish = true,
        };

        _dataverseService
            .GetSolution(Arg.Any<string>())
            .ReturnsForAnyArgs(solution);
        _dataverseService
            .GetPublisher(Arg.Any<EntityReference>())
            .Returns(publisher);
        _dataverseService
            .UpsertWebresource(Arg.Any<WebresourceFile>(), Arg.Any<PushOptions>())
            .ReturnsForAnyArgs(pushResult);
        _dataverseService
            .AddToSolution(Arg.Any<EntityReference>(), Arg.Any<EntityReference>())
            .ReturnsForAnyArgs(_ => new AddToSolutionResult(
                new EntityReference(WebResource.EntityLogicalName, Guid.NewGuid()),
                new EntityReference(SolutionComponent.EntityLogicalName, Guid.NewGuid()),
                AddToSolutionState.Added
            ));


        var results = _pushService.PushWebresources(options);

        results.ShouldNotBeNull().ShouldNotBeEmpty();
        results.Count.ShouldBe(1);
        results.ShouldContain(result =>
            result.PushState == pushResult.PushState &&
            result.WebresourceReference.Id == pushResult.WebresourceReference.Id);

        _observable
            .Received(1)
            .Publish(Arg.Any<PublishedWebresourcesEvent>());
        _observable
            .Received(1)
            .Publish(Arg.Any<RetrievedSolutionEvent>());
        _observable
            .Received(1)
            .Publish(Arg.Any<PushInvokedEvent>());
        _observable
            .Received(1)
            .PublishRange(Arg.Any<ICollection<RetrievedFileEvent>>());
        _observable
            .Received(1)
            .PublishRange(Arg.Any<ICollection<PushedWebresourceEvent>>());
    }

    [Fact]
    public void ShouldSkipPublishIfArgumentFalse()
    {
        var basePath = Path.Combine(s_temporaryPath, "dist");

        const string formFileName = "account.form.js";
        var formPath = Path.Combine(basePath, formFileName);
        const string formContent = "function onLoad(context) { }";
        _fileSystem.AddFile(formPath, new MockFileData(formContent));

        var publisher = new Publisher
        {
            Id = Guid.NewGuid(),
            CustomizationPrefix = "pub"
        };
        var solution = new Solution
        {
            Id = Guid.NewGuid(),
            UniqueName = "customizations",
            PublisherId = publisher.ToEntityReference()
        };
        var pushResult = new PushResult(
            new EntityReference(WebResource.EntityLogicalName, Guid.NewGuid()),
            new WebresourceFile(
                formFileName,
                formPath,
                ".js",
                "not relevant",
                formContent
            ),
            PushState.Created
        );

        var options = new PushOptions
        {
            Directory = basePath,
            Solution = solution.UniqueName,
            Publish = false,
        };

        _dataverseService
            .GetSolution(Arg.Any<string>())
            .ReturnsForAnyArgs(solution);
        _dataverseService
            .GetPublisher(Arg.Any<EntityReference>())
            .Returns(publisher);
        _dataverseService
            .UpsertWebresource(Arg.Any<WebresourceFile>(), Arg.Any<PushOptions>())
            .ReturnsForAnyArgs(pushResult);
        _dataverseService
            .AddToSolution(Arg.Any<EntityReference>(), Arg.Any<EntityReference>())
            .ReturnsForAnyArgs(_ => new AddToSolutionResult(
                new EntityReference(WebResource.EntityLogicalName, Guid.NewGuid()),
                new EntityReference(SolutionComponent.EntityLogicalName, Guid.NewGuid()),
                AddToSolutionState.Added
            ));


        var results = _pushService.PushWebresources(options);

        results.ShouldNotBeNull().ShouldNotBeEmpty();
        results.Count.ShouldBe(1);
        results.ShouldContain(result =>
            result.PushState == pushResult.PushState &&
            result.WebresourceReference.Id == pushResult.WebresourceReference.Id);
        _dataverseService
            .DidNotReceive()
            .Publish(Arg.Any<ICollection<EntityReference>>());
    }

    [Fact]
    public void ShouldPublishIfArgumentIsTrue()
    {
        var basePath = Path.Combine(s_temporaryPath, "dist");

        const string formFileName = "account.form.js";
        var formPath = Path.Combine(basePath, formFileName);
        const string formContent = "function onLoad(context) { }";
        _fileSystem.AddFile(formPath, new MockFileData(formContent));

        var publisher = new Publisher
        {
            Id = Guid.NewGuid(),
            CustomizationPrefix = "pub"
        };
        var solution = new Solution
        {
            Id = Guid.NewGuid(),
            UniqueName = "customizations",
            PublisherId = publisher.ToEntityReference()
        };
        var pushResult = new PushResult(
            new EntityReference(WebResource.EntityLogicalName, Guid.NewGuid()),
            new WebresourceFile(
                formFileName,
                formPath,
                ".js",
                "not relevant",
                formContent
            ),
            PushState.Created
        );

        var options = new PushOptions
        {
            Directory = basePath,
            Solution = solution.UniqueName,
            Publish = true
        };

        _dataverseService
            .GetSolution(Arg.Any<string>())
            .ReturnsForAnyArgs(solution);
        _dataverseService
            .GetPublisher(Arg.Any<EntityReference>())
            .Returns(publisher);
        _dataverseService
            .UpsertWebresource(Arg.Any<WebresourceFile>(), Arg.Any<PushOptions>())
            .ReturnsForAnyArgs(pushResult);
        _dataverseService
            .AddToSolution(Arg.Any<EntityReference>(), Arg.Any<EntityReference>())
            .ReturnsForAnyArgs(_ => new AddToSolutionResult(
                new EntityReference(WebResource.EntityLogicalName, Guid.NewGuid()),
                new EntityReference(SolutionComponent.EntityLogicalName, Guid.NewGuid()),
                AddToSolutionState.Existing
            ));


        var results = _pushService.PushWebresources(options);

        results.ShouldNotBeNull().ShouldNotBeEmpty();
        results.Count.ShouldBe(1);
        results.ShouldContain(result =>
            result.PushState == pushResult.PushState &&
            result.WebresourceReference.Id == pushResult.WebresourceReference.Id);
        _dataverseService
            .Received(1)
            .Publish(Arg.Any<ICollection<EntityReference>>());
        _observable
            .Received(1)
            .Publish(Arg.Any<PublishedWebresourcesEvent>());
    }

    [Fact]
    public void ShouldThrowOnRetrievalOfPublisherPrefix()
    {
        var publisher = new Publisher
        {
            Id = Guid.NewGuid(),
            CustomizationPrefix = "pub"
        };
        var solution = new Solution
        {
            Id = Guid.NewGuid(),
            UniqueName = "customizations",
            PublisherId = publisher.ToEntityReference()
        };

        var options = new PushOptions
        {
            Directory = s_temporaryPath,
            Solution = solution.UniqueName,
            PublisherPrefix = string.Empty,
        };

        _dataverseService
            .GetSolution(Arg.Any<string>())
            .ReturnsForAnyArgs(solution);
        var thrownException = new InvalidOperationException("this is a fake");
        _dataverseService
            .GetPublisher(Arg.Any<EntityReference>())
            .ThrowsForAnyArgs(thrownException);

        var exception = Should.Throw<InvalidOperationException>(() =>
            _pushService.PushWebresources(options));

        exception.Message.ShouldBe(thrownException.Message);
        _observable
            .Received(1)
            .PublishError(thrownException);
    }

    [Fact]
    public void ShouldThrowOnRetrievalOfSolution()
    {
        var options = new PushOptions
        {
            Directory = s_temporaryPath,
            Solution = "customizations",
            PublisherPrefix = string.Empty,
            FileExtensions = ["js"],
            IncludeSubDirectories = true,
            ForceUpdate = false
        };


        var thrownException = new InvalidOperationException("this is a fake");
        _dataverseService
            .GetSolution(Arg.Any<string>())
            .ThrowsForAnyArgs(thrownException);

        var exception = Should.Throw<InvalidOperationException>(() =>
            _pushService.PushWebresources(options));

        exception.Message.ShouldBe(thrownException.Message);
        _observable
            .Received(1)
            .PublishError(thrownException);
    }
}
