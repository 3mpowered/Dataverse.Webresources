using System.IO.Abstractions.TestingHelpers;
using CliWrap;
using Empowered.Dataverse.Webresources.Init.Events;
using Empowered.Dataverse.Webresources.Init.Model;
using FluentAssertions;

namespace Empowered.Dataverse.Webresources.Init.Tests.Events;

public class EventInitialisationTests
{
    private readonly MockFileSystem _fileSystem = new();

    [Fact]
    public void CanInitialiseInitInvokedEventFromInitOptions()
    {
        var initOptions = new InitOptions
        {
            Directory = Path.GetTempPath(),
            Project = "TestProject",
            GlobalNamespace = "xrm"
        };
        var initInvokedEvent = InitInvokedEvent.From(initOptions);

        initInvokedEvent.Should().NotBeNull();
        initInvokedEvent.Directory.FullName.Should().Be(initOptions.Directory);
        initInvokedEvent.Project.Should().Be(initOptions.Project);
        initInvokedEvent.GlobalNamespace.Should().Be(initOptions.GlobalNamespace);
    }

    [Fact]
    public void CanInitialiseDirectoryCreatedEventFromIDirectoryInfo()
    {
        var directoryInfo = _fileSystem.DirectoryInfo.Wrap(new DirectoryInfo(Path.GetTempPath()));

        var directoryCreatedEvent = DirectoryCreatedEvent.From(directoryInfo);

        directoryCreatedEvent.Should().NotBeNull();
        directoryCreatedEvent.Directory.Should().BeEquivalentTo(directoryInfo);
    }

    [Fact]
    public void CanInitialiseFileCreatedEventFromIFileInfo()
    {
        var fileInfo = _fileSystem.FileInfo.Wrap(new FileInfo(Path.GetTempPath()));

        var fileCreatedEvent = FileCreatedEvent.From(fileInfo);

        fileCreatedEvent.Should().NotBeNull();
        fileCreatedEvent.File.Should().BeEquivalentTo(fileInfo);
    }

    [Fact]
    public void CanInitialiseNpmInstallSucceededEventFromCommandResult()
    {
        var commandResult = new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1));

        var npmInstallSucceededEvent = NpmInstallSucceededEvent.From(commandResult);

        npmInstallSucceededEvent.Should().NotBeNull();
        npmInstallSucceededEvent.Result.Should().BeEquivalentTo(commandResult);
    }

    [Fact]
    public void CanInitialiseNpmUpgradeSucceededEventFromCommandResult()
    {
        var commandResult = new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1));

        var npmUpgradeSucceededEvent = NpmUpgradeSucceededEvent.From(commandResult);

        npmUpgradeSucceededEvent.Should().NotBeNull();
        npmUpgradeSucceededEvent.Result.Should().BeEquivalentTo(commandResult);
    }
}
