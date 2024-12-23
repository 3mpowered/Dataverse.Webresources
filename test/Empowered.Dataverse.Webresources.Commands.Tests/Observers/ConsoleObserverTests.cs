using Empowered.Dataverse.Webresources.Commands.Observers;
using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Events;
using Empowered.Dataverse.Webresources.Push.Model;
using FluentAssertions;
using Microsoft.Xrm.Sdk;
using Spectre.Console.Testing;
using Xunit;

namespace Empowered.Dataverse.Webresources.Commands.Tests.Observers;

public class ConsoleObserverTests
{
    private readonly TestConsole _console = new();
    private readonly ConsoleObserver _consoleObserver;

    public ConsoleObserverTests()
    {
        _consoleObserver = new ConsoleObserver(_console);
    }

    [Fact]
    public void ShouldPrintOnCompletion()
    {
        _consoleObserver.OnCompleted();
        _console.Output.Should().StartWith("Finished command");
    }

    [Fact]
    public void ShouldPrintExceptionMessageOnError()
    {
        const string errorMessage = "Something went wrong";
        _consoleObserver.OnError(new InvalidOperationException(errorMessage));
        _console.Output.Should().StartWith($"Command failed with error: {errorMessage}");
    }

    [Fact]
    public void ShouldPrintPublishedWebresourceOnNext()
    {
        var webresources = new List<EntityReference>
        {
            new(WebResource.EntityLogicalName, Guid.NewGuid())
        };
        var @event = PublishedWebresourcesEvent.From(webresources);

        _consoleObserver.OnNext(@event);
        _console.Output.Should().StartWith($"Published {@event.Webresources.Count} webresources");
    }

    [Fact]
    public void ShouldPrintPushedWebresourcesOnNext()
    {
        var webresourceFile = new WebresourceFile(
            "foo.js",
            Path.Combine(Path.GetTempPath(), "foo.js"),
            ".js",
            "pub_/foo.js",
            "console.log('hello world');"
        );
        var webresourceReference = new EntityReference(WebResource.EntityLogicalName, Guid.NewGuid());
        var pushResult = new PushResult(webresourceReference, webresourceFile, PushState.Created);
        var pushOptions = new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "customizations"
        };
        var @event = PushedWebresourceEvent.From(pushResult, pushOptions);

        _consoleObserver.OnNext(@event);

        // TODO: where does the \n in the output come from? Seems like an issue with the test console.
        var expected =
            $"{@event.PushResult.PushState.Format()} file {@event.PushResult.File.FilePath}";
        _console.Output.Replace(Environment.NewLine, string.Empty).Should().StartWith(expected);
    }

    [Fact]
    public void ShouldPrintOnPushInvokedOnNext()
    {
        var pushOptions = new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "customizations"
        };
        var @event = PushInvokedEvent.From(pushOptions);

        _consoleObserver.OnNext(@event);
        var expected =
            $"Pushing webresources from directory {@event.Directory.FullName} into";
        _console.Output.Replace(Environment.NewLine, string.Empty).Should().StartWith(expected);
    }

    [Fact]
    public void ShouldPrintRetrievedFileOnNext()
    {
        var pushOptions = new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "customizations"
        };
        var webresourceFile = new WebresourceFile(
            "foo.js",
            Path.Combine(Path.GetTempPath(), "foo.js"),
            ".js",
            "pub_/foo.js",
            "console.log('hello world');"
        );
        var @event = RetrievedFileEvent.From(pushOptions, webresourceFile);

        _consoleObserver.OnNext(@event);

        var expected = $"Found file {@event.File.FileName} from file path {@event.File.FilePath}";
        _console.Output.Replace(Environment.NewLine, string.Empty).Should().StartWith(expected);
    }

    [Fact]
    public void ShouldPrintRetrievePublisherOnNext()
    {
        var @event = new RetrievedPublisherEvent
        {
            Id = Guid.NewGuid(),
            Source = RetrievedPublisherEvent.PublisherSource.Configuration,
            CustomizationPrefix = "pub_",
            FriendlyName = "Publisher",
            UniqueName = "publisher"
        };

        _consoleObserver.OnNext(@event);

        var expected =
            $"Retrieved publisher {@event.UniqueName} with friendly name {@event.FriendlyName}";
        _console.Output.Replace(Environment.NewLine, string.Empty).Should().StartWith(expected);
    }

    [Fact]
    public void ShouldPrintRetrievedSolutionOnNext()
    {
        var @event = new RetrievedSolutionEvent
        {
            Id = Guid.NewGuid(),
            Version = "1.0.0.0",
            FriendlyName = "Customizations",
            PublisherId = new EntityReference(Publisher.EntityLogicalName, Guid.NewGuid()),
            UniqueName = "customizations"
        };

        _consoleObserver.OnNext(@event);

        var expected = $"Retrieved solution {@event.UniqueName} with friendly name {@event.FriendlyName}";
        _console.Output.Replace(Environment.NewLine, string.Empty).Should().StartWith(expected);
    }
}
