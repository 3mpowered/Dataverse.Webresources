using System.IO.Abstractions.TestingHelpers;
using CliWrap;
using CommandDotNet;
using Empowered.Dataverse.Webresources.Commands.Observers;
using Empowered.Dataverse.Webresources.Init.Events;
using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Events;
using Empowered.Dataverse.Webresources.Push.Model;
using FluentAssertions;
using Microsoft.Xrm.Sdk;
using Spectre.Console.Testing;

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
            $"Pushing webresources from directory {@event.Directory.FullName}";
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

    [Fact]
    public void ShouldPrintInitInvokedOnNext()
    {
        var @event = new InitInvokedEvent
        {
            Directory = new DirectoryInfo(Path.GetTempPath()),
            Project = "webresources",
            GlobalNamespace = "any"
        };

        _consoleObserver.OnNext(@event);

        _console.Output.Replace(Environment.NewLine, string.Empty).Should()
            .StartWith($"Initializing webresource project {@event.Project} in directory");
    }

    [Fact]
    public void ShouldPrintDirectoryCreatedOnNext()
    {
        var mockFileSystem = new MockFileSystem();
        var @event = new DirectoryCreatedEvent
        {
            Directory = mockFileSystem.DirectoryInfo.New(Path.GetTempPath())
        };

        _consoleObserver.OnNext(@event);

        _console.Output.Replace(Environment.NewLine, string.Empty)
            .Should()
            .StartWith($"Created directory {@event.Directory.Name}");
    }

    [Fact]
    public void ShouldPrintFileCreatedOnNext()
    {
        var mockFileSystem = new MockFileSystem();
        var @event = new FileCreatedEvent
        {
            File = mockFileSystem.FileInfo.New(Path.Combine(Path.GetTempPath(), "package.json"))
        };

        _consoleObserver.OnNext(@event);

        _console.Output.Replace(Environment.NewLine, string.Empty)
            .Should()
            .StartWith($"Created file {@event.File.Name}");
    }

    [Fact]
    public async Task ShouldPrintNpmInstallSucceededOnNext()
    {
        var @event = new NpmInstallSucceededEvent
        {
            Result = new CommandResult(await ExitCodes.Success, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1))
        };

        _consoleObserver.OnNext(@event);

        _console.Output.Replace(Environment.NewLine, string.Empty)
            .Should()
            .StartWith($"npm install succeeded in {@event.Result.RunTime.Milliseconds} milliseconds");
    }

    [Fact]
    public async Task ShouldPrintNpmUpgradeSucceededOnNext()
    {
        var @event = new NpmUpgradeSucceededEvent
        {
            Result = new CommandResult(await ExitCodes.Success, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1))
        };

        _consoleObserver.OnNext(@event);

        _console.Output.Replace(Environment.NewLine, string.Empty)
            .Should()
            .StartWith($"npm run dependencies:upgrade succeeded in {@event.Result.RunTime.Milliseconds} milliseconds");
    }
}
