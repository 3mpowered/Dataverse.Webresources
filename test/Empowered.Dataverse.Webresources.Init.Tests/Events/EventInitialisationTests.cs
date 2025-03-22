using System.IO.Abstractions.TestingHelpers;
using CliWrap;
using Empowered.Dataverse.Webresources.Init.Events;
using Empowered.Dataverse.Webresources.Init.Model;
using Shouldly;

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

        initInvokedEvent.ShouldNotBeNull();
        initInvokedEvent.Directory.FullName.ShouldBe(initOptions.Directory);
        initInvokedEvent.Project.ShouldBe(initOptions.Project);
        initInvokedEvent.GlobalNamespace.ShouldBe(initOptions.GlobalNamespace);
    }

    [Fact]
    public void CanInitialiseDirectoryCreatedEventFromIDirectoryInfo()
    {
        var directoryInfo = _fileSystem.DirectoryInfo.Wrap(new DirectoryInfo(Path.GetTempPath()));

        var directoryCreatedEvent = DirectoryCreatedEvent.From(directoryInfo);

        directoryCreatedEvent.ShouldNotBeNull();
        directoryCreatedEvent.Directory.ShouldBeEquivalentTo(directoryInfo);
    }

    [Fact]
    public void CanInitialiseFileCreatedEventFromIFileInfo()
    {
        var fileInfo = _fileSystem.FileInfo.Wrap(new FileInfo(Path.GetTempPath()));

        var fileCreatedEvent = FileCreatedEvent.From(fileInfo);

        fileCreatedEvent.ShouldNotBeNull();
        fileCreatedEvent.File.ShouldBeEquivalentTo(fileInfo);
    }

    [Fact]
    public void CanInitialiseNpmInstallSucceededEventFromCommandResult()
    {
        var commandResult = new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1));

        var npmInstallSucceededEvent = NpmInstallSucceededEvent.From(commandResult);

        npmInstallSucceededEvent.ShouldNotBeNull();
        npmInstallSucceededEvent.Result.ShouldBeEquivalentTo(commandResult);
    }

    [Fact]
    public void CanInitialiseNpmUpgradeSucceededEventFromCommandResult()
    {
        var commandResult = new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1));

        var npmUpgradeSucceededEvent = NpmUpgradeSucceededEvent.From(commandResult);

        npmUpgradeSucceededEvent.ShouldNotBeNull();
        npmUpgradeSucceededEvent.Result.ShouldBeEquivalentTo(commandResult);
    }
}
